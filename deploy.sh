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
    if [ ! -f .env ]; then
        warn "File .env belum ada."
        if [ -f .env.example ]; then
            info "Membuat .env dari .env.example..."
            cp .env.example .env
            warn "PENTING: Edit file .env dan ganti nilai default sebelum deploy production."
            warn "  nano .env"
            exit 1
        fi

        error "File .env.example tidak ditemukan."
        exit 1
    fi

    log "File .env ditemukan."

    # shellcheck disable=SC1091
    source .env 2>/dev/null || true
    if [[ "${POSTGRES_PASSWORD:-}" == "GANTI_DENGAN_PASSWORD_KUAT" ]] || \
       [[ "${JWT_SIGNING_KEY:-}" == *"GANTI"* ]]; then
        error "Variabel .env masih menggunakan nilai default."
        error "Edit .env dan ganti POSTGRES_PASSWORD, JWT_SIGNING_KEY, dll."
        exit 1
    fi

    log "Variabel .env tervalidasi."
}

get_env_file_value() {
    local key="$1"
    if [ ! -f .env ]; then
        return 0
    fi

    local line
    line="$(grep -E "^[[:space:]]*${key}[[:space:]]*=" .env | tail -n 1 || true)"
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

    if ! docker compose "${compose_args[@]}" config >/dev/null; then
        error "Validasi compose production gagal."
        exit 1
    fi

    log "Compose production tervalidasi."
}

validate_compose_dev() {
    if ! docker compose config >/dev/null; then
        error "Validasi compose development gagal."
        exit 1
    fi

    log "Compose development tervalidasi."
}

validate_compose_dev_watch() {
    if ! docker compose -f docker-compose.yml -f docker-compose.watch.yml config >/dev/null; then
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
    local ghcr_registry="${GHCR_REGISTRY:-ghcr.io}"
    local compose_args=(-f docker-compose.yml -f docker-compose.prod.yml)

    if is_tunnel_profile_enabled; then
        compose_args+=(--profile tunnel)
        info "CLOUDFLARE_TUNNEL_TOKEN ditemukan. Tunnel profile akan dijalankan."
    else
        warn "CLOUDFLARE_TUNNEL_TOKEN tidak ditemukan. Deploy tetap berjalan tanpa profile tunnel."
    fi

    if [ -n "${GHCR_USERNAME:-}" ] && [ -n "${GHCR_TOKEN:-}" ]; then
        info "Login ke GHCR (${ghcr_registry})..."
        echo "${GHCR_TOKEN}" | docker login "${ghcr_registry}" -u "${GHCR_USERNAME}" --password-stdin
    else
        warn "GHCR_USERNAME/GHCR_TOKEN tidak diset. Asumsi image publik atau host sudah login."
    fi

    info "Pull image production..."
    docker compose "${compose_args[@]}" pull api ui

    info "Starting semua service tanpa build ulang..."
    docker compose "${compose_args[@]}" up -d --no-build

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

    docker compose up -d --build

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

    docker compose "${compose_args[@]}" up -d --build

    info "Menunggu health check..."
    sleep 15

    run_smoke_checks dev
    show_status
    echo ""
    log "Mode development watch aktif."
    info "Akses UI:      http://localhost:5203"
    info "Akses API:     http://localhost:5041"
    info "Tekan Ctrl+C untuk berhenti mode watch."

    docker compose "${compose_args[@]}" watch
}

show_status() {
    local compose_args=(-f docker-compose.yml -f docker-compose.prod.yml)
    if is_tunnel_profile_enabled; then
        compose_args+=(--profile tunnel)
    fi

    echo ""
    info "Status Container:"
    echo "----------------------------------------------------"
    docker compose "${compose_args[@]}" ps 2>/dev/null \
        || docker compose ps
    echo "----------------------------------------------------"
}

show_logs() {
    local compose_args=(-f docker-compose.yml -f docker-compose.prod.yml)
    if is_tunnel_profile_enabled; then
        compose_args+=(--profile tunnel)
    fi

    docker compose "${compose_args[@]}" logs -f --tail=100 2>/dev/null \
        || docker compose logs -f --tail=100
}

stop_all() {
    local compose_args=(-f docker-compose.yml -f docker-compose.prod.yml)
    if is_tunnel_profile_enabled; then
        compose_args+=(--profile tunnel)
    fi

    warn "Menghentikan semua service..."
    docker compose "${compose_args[@]}" down 2>/dev/null \
        || docker compose down
    log "Semua service dihentikan."
}

restart_all() {
    local compose_args=(-f docker-compose.yml -f docker-compose.prod.yml)
    if is_tunnel_profile_enabled; then
        compose_args+=(--profile tunnel)
    fi

    info "Restart semua service..."
    docker compose "${compose_args[@]}" restart 2>/dev/null \
        || docker compose restart
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
  --status       Tampilkan status container
  --logs         Tail logs semua service
  --down         Stop semua service
  --restart      Restart semua service
  --help         Tampilkan bantuan ini
EOF
}

case "${1:-}" in
    --dev)      deploy_dev ;;
    --dev-watch) deploy_dev_watch ;;
    --status)   show_status ;;
    --logs)     show_logs ;;
    --down)     stop_all ;;
    --restart)  restart_all ;;
    --help|-h)  show_help ;;
    "")         deploy_production ;;
    *)          error "Opsi tidak dikenal: $1"; show_help; exit 1 ;;
esac
