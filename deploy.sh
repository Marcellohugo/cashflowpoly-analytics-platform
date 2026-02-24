# Fungsi file: Menjalankan otomasi deployment aplikasi melalui skrip shell Linux.
#!/usr/bin/env bash
# ============================================================
# deploy.sh - Script deployment Cashflowpoly Analytics Platform
# ============================================================
# Penggunaan:
#   chmod +x deploy.sh
#   ./deploy.sh            -> deploy production
#   ./deploy.sh --dev      -> deploy development (port langsung)
#   ./deploy.sh --dev-watch -> deploy development + auto-redeploy
#   ./deploy.sh --env-file .env.prod -> pakai env file spesifik
#   ./deploy.sh --status   -> cek status semua container
#   ./deploy.sh --logs     -> tail logs semua service
#   ./deploy.sh --down     -> stop semua service
#   ./deploy.sh --restart  -> restart semua service
# ============================================================

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

log()   { echo -e "${GREEN}[OK]${NC} $1"; }
warn()  { echo -e "${YELLOW}[!]${NC} $1"; }
error() { echo -e "${RED}[X]${NC} $1"; }
info()  { echo -e "${BLUE}[->]${NC} $1"; }

default_env_file_for_mode() {
    local mode="${1:-prod}"
    if [ "$mode" = "dev" ] || [ "$mode" = "dev-watch" ]; then
        echo ".env.dev"
        return
    fi

    echo ".env.prod"
}

resolve_env_file() {
    local mode="${1:-prod}"
    local requested_env_file="${2:-}"

    if [ -n "$requested_env_file" ]; then
        echo "$requested_env_file"
        return
    fi

    local mode_default
    mode_default="$(default_env_file_for_mode "$mode")"
    if [ -f "$mode_default" ]; then
        echo "$mode_default"
        return
    fi

    if [ -f ".env" ]; then
        echo ".env"
        return
    fi

    echo "$mode_default"
}

get_env_template_for_target() {
    local target_env_file="$1"
    if [[ "$target_env_file" == *.env.dev ]] && [ -f ".env.dev.example" ]; then
        echo ".env.dev.example"
        return
    fi

    if [[ "$target_env_file" == *.env.prod ]] && [ -f ".env.prod.example" ]; then
        echo ".env.prod.example"
        return
    fi

    if [ -f ".env.example" ]; then
        echo ".env.example"
        return
    fi
}

compose() {
    docker compose --env-file "$ENV_FILE" "$@"
}

check_dependencies() {
    local missing=0
    for cmd in docker; do
        if ! command -v "$cmd" &>/dev/null; then
            error "$cmd tidak ditemukan. Silakan install terlebih dahulu."
            missing=1
        fi
    done

    if ! docker compose version &>/dev/null; then
        error "Docker Compose V2 tidak ditemukan."
        missing=1
    fi

    if [ $missing -eq 1 ]; then
        exit 1
    fi

    log "Semua dependensi tersedia."
}

check_env_file() {
    if [ ! -f "$ENV_FILE" ]; then
        warn "File $ENV_FILE belum ada."
        local env_template
        env_template="$(get_env_template_for_target "$ENV_FILE")"
        if [ -n "${env_template:-}" ]; then
            info "Membuat $ENV_FILE dari $env_template..."
            cp "$env_template" "$ENV_FILE"
            warn "PENTING: Edit file $ENV_FILE dan ganti nilai default sebelum deploy production."
            warn "  nano $ENV_FILE"
            exit 1
        fi

        error "Template env file tidak ditemukan."
        exit 1
    fi

    log "File $ENV_FILE ditemukan."

    # shellcheck disable=SC1091
    source "$ENV_FILE" 2>/dev/null || true
    if [[ "${POSTGRES_PASSWORD:-}" == "GANTI_DENGAN_PASSWORD_KUAT" ]] || \
       [[ "${JWT_SIGNING_KEY:-}" == *"GANTI"* ]]; then
        error "Variabel $ENV_FILE masih menggunakan nilai default."
        error "Edit $ENV_FILE dan ganti POSTGRES_PASSWORD, JWT_SIGNING_KEY, dll."
        exit 1
    fi

    log "Variabel $ENV_FILE tervalidasi."
}

get_env_file_value() {
    local key="$1"
    if [ ! -f "$ENV_FILE" ]; then
        return 0
    fi

    local line
    line="$(grep -E "^[[:space:]]*${key}[[:space:]]*=" "$ENV_FILE" | tail -n 1 || true)"
    if [ -z "$line" ]; then
        return 0
    fi

    echo "${line#*=}" | sed -e "s/^[[:space:]]*//" -e "s/[[:space:]]*$//" -e "s/^['\"]//" -e "s/['\"]$//"
}

is_tunnel_profile_enabled() {
    local token="${CLOUDFLARE_TUNNEL_TOKEN:-}"
    if [ -z "$token" ]; then
        token="$(get_env_file_value "CLOUDFLARE_TUNNEL_TOKEN")"
    fi

    if [ -z "$token" ]; then
        return 1
    fi

    if [[ "$token" =~ ^GANTI_ ]]; then
        return 1
    fi

    return 0
}

http_status() {
    local url="$1"
    curl -sS -o /dev/null -w "%{http_code}" --max-redirs 0 "$url" || echo "000"
}

assert_status() {
    local name="$1"
    local url="$2"
    local expected="$3"
    local status
    status="$(http_status "$url")"
    if [ "$status" != "$expected" ]; then
        error "$name gagal. URL: $url, status: $status (expected: $expected)"
        return 1
    fi

    log "$name: $status"
    return 0
}

run_smoke_checks() {
    local mode="${1:-prod}"
    local failed=0

    info "Menjalankan smoke checks..."
    if [ "$mode" = "prod" ]; then
        assert_status "Health" "http://localhost/health" "200" || failed=1
        assert_status "Health Live" "http://localhost/health/live" "200" || failed=1
        assert_status "Health Ready" "http://localhost/health/ready" "200" || failed=1
        assert_status "UI CSS Site" "http://localhost/css/site.css" "200" || failed=1
        assert_status "UI CSS Tailwind" "http://localhost/css/tailwind.css" "200" || failed=1
        assert_status "Swagger" "http://localhost/swagger/index.html" "200" || failed=1
    else
        assert_status "UI Health Ready" "http://localhost:5203/health/ready" "200" || failed=1
        assert_status "API Health Ready" "http://localhost:5041/health/ready" "200" || failed=1
        assert_status "UI CSS Site" "http://localhost:5203/css/site.css" "200" || failed=1
        assert_status "UI CSS Tailwind" "http://localhost:5203/css/tailwind.css" "200" || failed=1
        assert_status "Swagger" "http://localhost:5041/swagger/index.html" "200" || failed=1
    fi

    if [ "$failed" -ne 0 ]; then
        exit 1
    fi
}

validate_compose_prod() {
    local compose_args=(-f docker-compose.yml -f docker-compose.prod.yml)
    if is_tunnel_profile_enabled; then
        compose_args+=(--profile tunnel)
    fi

    if ! compose "${compose_args[@]}" config >/dev/null; then
        error "Validasi compose production gagal."
        exit 1
    fi

    log "Compose production tervalidasi."
}

validate_compose_dev() {
    if ! compose config >/dev/null; then
        error "Validasi compose development gagal."
        exit 1
    fi

    log "Compose development tervalidasi."
}

validate_compose_dev_watch() {
    if ! compose -f docker-compose.yml -f docker-compose.watch.yml config >/dev/null; then
        error "Validasi compose development watch gagal."
        exit 1
    fi

    log "Compose development watch tervalidasi."
}

check_compose_watch_support() {
    if ! docker compose watch --help >/dev/null 2>&1; then
        error "Docker Compose watch belum tersedia. Perbarui Docker Compose ke versi terbaru."
        exit 1
    fi
}

deploy_production() {
    info "Deploying Cashflowpoly (PRODUCTION)..."
    check_dependencies
    check_env_file
    validate_compose_prod
    local ghcr_registry="${GHCR_REGISTRY:-$(get_env_file_value GHCR_REGISTRY)}"
    local ghcr_username="${GHCR_USERNAME:-$(get_env_file_value GHCR_USERNAME)}"
    local ghcr_token="${GHCR_TOKEN:-$(get_env_file_value GHCR_TOKEN)}"
    local compose_args=(-f docker-compose.yml -f docker-compose.prod.yml)

    if [ -z "$ghcr_registry" ]; then
        ghcr_registry="ghcr.io"
    fi

    if is_tunnel_profile_enabled; then
        compose_args+=(--profile tunnel)
        info "CLOUDFLARE_TUNNEL_TOKEN ditemukan. Tunnel profile akan dijalankan."
    else
        warn "CLOUDFLARE_TUNNEL_TOKEN tidak ditemukan. Deploy tetap berjalan tanpa profile tunnel."
    fi

    if [ -n "$ghcr_username" ] && [ -n "$ghcr_token" ]; then
        info "Login ke GHCR (${ghcr_registry})..."
        echo "${ghcr_token}" | docker login "${ghcr_registry}" -u "${ghcr_username}" --password-stdin
    else
        warn "GHCR_USERNAME/GHCR_TOKEN tidak diset. Asumsi image publik atau host sudah login."
    fi

    info "Pull image production..."
    compose "${compose_args[@]}" pull api ui

    info "Memastikan seluruh service production berjalan..."
    compose "${compose_args[@]}" up -d --no-build

    info "Redeploy service production (api + ui) dengan force recreate..."
    compose "${compose_args[@]}" up -d --no-build --force-recreate --no-deps api ui

    info "Menunggu health check..."
    sleep 15

    run_smoke_checks prod
    show_status
    echo ""
    log "Deploy production selesai."
    info "Akses dashboard: http://localhost"
    info "Akses API:       http://localhost/api/"
}

deploy_dev() {
    info "Deploying Cashflowpoly (DEVELOPMENT)..."
    check_dependencies

    if [ ! -f .env ] && [ -f .env.example ]; then
        cp .env.example .env
        warn "File .env dibuat dari template. Edit jika perlu."
    fi

    validate_compose_dev

    compose up -d --build

    info "Menunggu health check..."
    sleep 15

    run_smoke_checks dev
    show_status
    echo ""
    log "Deploy development selesai."
    info "Akses UI:      http://localhost:5203"
    info "Akses API:     http://localhost:5041"
    info "Swagger:       http://localhost:5041/swagger"
}

deploy_dev_watch() {
    info "Deploying Cashflowpoly (DEVELOPMENT + WATCH)..."
    check_dependencies
    check_compose_watch_support
    local compose_args=(-f docker-compose.yml -f docker-compose.watch.yml)

    if [ ! -f .env ] && [ -f .env.example ]; then
        cp .env.example .env
        warn "File .env dibuat dari template. Edit jika perlu."
    fi

    validate_compose_dev_watch

    compose "${compose_args[@]}" up -d --build

    info "Menunggu health check..."
    sleep 15

    run_smoke_checks dev
    show_status
    echo ""
    log "Mode development watch aktif."
    info "Akses UI:      http://localhost:5203"
    info "Akses API:     http://localhost:5041"
    info "Tekan Ctrl+C untuk berhenti mode watch."

    compose "${compose_args[@]}" watch
}

show_status() {
    local compose_args=(-f docker-compose.yml -f docker-compose.prod.yml)
    if is_tunnel_profile_enabled; then
        compose_args+=(--profile tunnel)
    fi

    echo ""
    info "Status Container:"
    echo "----------------------------------------------------"
    compose "${compose_args[@]}" ps 2>/dev/null \
        || compose ps
    echo "----------------------------------------------------"
}

show_logs() {
    local compose_args=(-f docker-compose.yml -f docker-compose.prod.yml)
    if is_tunnel_profile_enabled; then
        compose_args+=(--profile tunnel)
    fi

    compose "${compose_args[@]}" logs -f --tail=100 2>/dev/null \
        || compose logs -f --tail=100
}

stop_all() {
    local compose_args=(-f docker-compose.yml -f docker-compose.prod.yml)
    if is_tunnel_profile_enabled; then
        compose_args+=(--profile tunnel)
    fi

    warn "Menghentikan semua service..."
    compose "${compose_args[@]}" down 2>/dev/null \
        || compose down
    log "Semua service dihentikan."
}

restart_all() {
    local compose_args=(-f docker-compose.yml -f docker-compose.prod.yml)
    if is_tunnel_profile_enabled; then
        compose_args+=(--profile tunnel)
    fi

    info "Restart semua service..."
    compose "${compose_args[@]}" restart 2>/dev/null \
        || compose restart
    sleep 10
    show_status
    log "Restart selesai."
}

show_help() {
    cat <<'EOF'
Penggunaan: ./deploy.sh [OPTION]

Options:
  (tanpa opsi)   Deploy production (dengan Nginx)
  --dev          Deploy development (port langsung)
  --dev-watch    Deploy development + auto-redeploy
  --env-file     Pilih file env custom (contoh: .env.prod)
  --status       Tampilkan status container
  --logs         Tail logs semua service
  --down         Stop semua service
  --restart      Restart semua service
  --help         Tampilkan bantuan ini
EOF
}

MODE="prod"
CUSTOM_ENV_FILE=""

while [ $# -gt 0 ]; do
    case "$1" in
        --dev) MODE="dev" ;;
        --dev-watch) MODE="dev-watch" ;;
        --status) MODE="status" ;;
        --logs) MODE="logs" ;;
        --down) MODE="down" ;;
        --restart) MODE="restart" ;;
        --env-file)
            if [ $# -lt 2 ]; then
                error "Nilai untuk --env-file belum diisi."
                show_help
                exit 1
            fi
            CUSTOM_ENV_FILE="$2"
            shift
            ;;
        --help|-h)
            show_help
            exit 0
            ;;
        *)
            error "Opsi tidak dikenal: $1"
            show_help
            exit 1
            ;;
    esac
    shift
done

ENV_FILE="$(resolve_env_file "$MODE" "$CUSTOM_ENV_FILE")"
info "Menggunakan env file: $ENV_FILE"

case "$MODE" in
    dev) deploy_dev ;;
    dev-watch) deploy_dev_watch ;;
    status) show_status ;;
    logs) show_logs ;;
    down) stop_all ;;
    restart) restart_all ;;
    prod) deploy_production ;;
    *)
        error "Mode tidak dikenal: $MODE"
        exit 1
        ;;
esac
