# Fungsi file: Menjalankan otomasi deployment aplikasi melalui skrip shell Linux.
#!/usr/bin/env bash
# ============================================================
# deploy.sh - Script deployment Cashflowpoly Analytics Platform
# ============================================================
# Penggunaan:
#   chmod +x deploy.sh
#   ./deploy.sh            -> deploy production
#   ./deploy.sh --dev      -> deploy development (port langsung)
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

validate_compose_prod() {
    if ! docker compose -f docker-compose.yml -f docker-compose.prod.yml config >/dev/null; then
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

deploy_production() {
    info "Deploying Cashflowpoly (PRODUCTION)..."
    check_dependencies
    check_env_file
    validate_compose_prod

    info "Building & starting semua service..."
    docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build

    info "Menunggu health check..."
    sleep 15

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

    show_status
    echo ""
    log "Deploy development selesai."
    info "Akses UI:      http://localhost:5203"
    info "Akses API:     http://localhost:5041"
    info "Swagger:       http://localhost:5041/swagger"
}

show_status() {
    echo ""
    info "Status Container:"
    echo "----------------------------------------------------"
    docker compose -f docker-compose.yml -f docker-compose.prod.yml ps 2>/dev/null \
        || docker compose ps
    echo "----------------------------------------------------"
}

show_logs() {
    docker compose -f docker-compose.yml -f docker-compose.prod.yml logs -f --tail=100 2>/dev/null \
        || docker compose logs -f --tail=100
}

stop_all() {
    warn "Menghentikan semua service..."
    docker compose -f docker-compose.yml -f docker-compose.prod.yml down 2>/dev/null \
        || docker compose down
    log "Semua service dihentikan."
}

restart_all() {
    info "Restart semua service..."
    docker compose -f docker-compose.yml -f docker-compose.prod.yml restart 2>/dev/null \
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
  --status       Tampilkan status container
  --logs         Tail logs semua service
  --down         Stop semua service
  --restart      Restart semua service
  --help         Tampilkan bantuan ini
EOF
}

case "${1:-}" in
    --dev)      deploy_dev ;;
    --status)   show_status ;;
    --logs)     show_logs ;;
    --down)     stop_all ;;
    --restart)  restart_all ;;
    --help|-h)  show_help ;;
    "")         deploy_production ;;
    *)          error "Opsi tidak dikenal: $1"; show_help; exit 1 ;;
esac
