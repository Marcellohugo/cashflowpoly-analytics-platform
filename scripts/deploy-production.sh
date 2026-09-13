#!/usr/bin/env bash
# Fungsi file: Menjalankan deployment production terkunci, bermigrasi maju, dan rollback image jika health check gagal.
set -Eeuo pipefail

if [[ ${EUID} -ne 0 ]]; then
  echo "Deployment wajib dijalankan sebagai root." >&2
  exit 1
fi

APP_ROOT=${APP_ROOT:-/opt/cashflowpoly}
REPOSITORY_DIR=${REPOSITORY_DIR:-$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.." && pwd)}
RELEASES_DIR=${RELEASES_DIR:-${APP_ROOT}/releases}
ENV_FILE=${ENV_FILE:-${REPOSITORY_DIR}/config/env/.env.prod}
LOCK_FILE=${LOCK_FILE:-/var/lock/cashflowpoly-deploy.lock}
BRANCH=${BRANCH:-prod}
COMPOSE_PROJECT_NAME=${COMPOSE_PROJECT_NAME:-cashflowpoly-analytics-platform}

mkdir -p "${APP_ROOT}" "${RELEASES_DIR}" "$(dirname "${ENV_FILE}")"
APP_ROOT_RESOLVED=$(realpath "${APP_ROOT}")
RELEASES_DIR_RESOLVED=$(realpath "${RELEASES_DIR}")
case "${RELEASES_DIR_RESOLVED}" in
  "${APP_ROOT_RESOLVED}"/*) ;;
  *) echo "RELEASES_DIR wajib berada di dalam APP_ROOT." >&2; exit 1 ;;
esac

exec 9>"${LOCK_FILE}"
if ! flock -n 9; then
  echo "Deployment lain masih berjalan." >&2
  exit 1
fi

for command_name in git docker curl logger realpath; do
  command -v "${command_name}" >/dev/null || {
    echo "Perintah ${command_name} tidak tersedia." >&2
    exit 1
  }
done

[[ -d "${REPOSITORY_DIR}/.git" ]] || {
  echo "Repository deployment tidak ditemukan: ${REPOSITORY_DIR}" >&2
  exit 1
}
[[ -f "${ENV_FILE}" ]] || {
  echo "Environment production tidak ditemukan: ${ENV_FILE}" >&2
  exit 1
}

DOMAIN_HOST=""
while IFS='=' read -r env_name env_value; do
  if [[ "${env_name}" == "DOMAIN" ]]; then
    DOMAIN_HOST=${env_value%$'\r'}
    DOMAIN_HOST=${DOMAIN_HOST#\"}
    DOMAIN_HOST=${DOMAIN_HOST%\"}
    DOMAIN_HOST=${DOMAIN_HOST#\'}
    DOMAIN_HOST=${DOMAIN_HOST%\'}
  fi
done <"${ENV_FILE}"
[[ "${DOMAIN_HOST}" =~ ^[A-Za-z0-9.-]+(:[0-9]+)?$ ]] || {
  echo "DOMAIN pada environment production wajib berupa hostname yang valid." >&2
  exit 1
}

git -C "${REPOSITORY_DIR}" fetch --prune origin "${BRANCH}"
RELEASE_SHA=$(git -C "${REPOSITORY_DIR}" rev-parse "origin/${BRANCH}^{commit}")
RELEASE_DIR="${RELEASES_DIR_RESOLVED}/${RELEASE_SHA}"
CURRENT_LINK="${APP_ROOT}/current"
PREVIOUS_DIR=""
PREVIOUS_SHA=""

if [[ -L "${CURRENT_LINK}" ]]; then
  PREVIOUS_DIR=$(readlink -f "${CURRENT_LINK}")
  case "${PREVIOUS_DIR}" in
    "${RELEASES_DIR_RESOLVED}"/*) PREVIOUS_SHA=$(basename "${PREVIOUS_DIR}") ;;
    *) echo "Symlink current berada di luar direktori rilis." >&2; exit 1 ;;
  esac
fi

if [[ ! -d "${RELEASE_DIR}" ]]; then
  git -C "${REPOSITORY_DIR}" worktree add --detach "${RELEASE_DIR}" "${RELEASE_SHA}"
elif [[ $(git -C "${RELEASE_DIR}" rev-parse HEAD 2>/dev/null || true) != "${RELEASE_SHA}" ]]; then
  echo "Direktori rilis sudah ada tetapi tidak berisi commit ${RELEASE_SHA}." >&2
  exit 1
fi

export RELEASE_SHA
# Seed 2 mengikuti konfigurasi environment produksi (default aktif pada Compose produksi).
COMPOSE=(
  docker compose
  --project-name "${COMPOSE_PROJECT_NAME}"
  --env-file "${ENV_FILE}"
  -f "${RELEASE_DIR}/infra/docker/docker-compose.yml"
  -f "${RELEASE_DIR}/infra/docker/docker-compose.prod.yml"
  --profile tunnel
)

"${COMPOSE[@]}" config -q

install_journald_retention() {
  local target=/etc/systemd/journald.conf.d/cashflowpoly.conf
  mkdir -p "$(dirname "${target}")"
  local temporary
  temporary=$(mktemp)
  cat >"${temporary}" <<'EOF'
[Journal]
MaxRetentionSec=30day
SystemMaxUse=512M
RuntimeMaxUse=128M
EOF
  if [[ ! -f "${target}" ]] || ! cmp -s "${temporary}" "${target}"; then
    install -m 0644 "${temporary}" "${target}"
    systemctl restart systemd-journald
  fi
  rm -f "${temporary}"
}

wait_for_container_health() {
  local container_name=$1
  local attempts=${2:-30}
  for ((attempt = 1; attempt <= attempts; attempt++)); do
    local status
    status=$(docker inspect --format '{{if .State.Health}}{{.State.Health.Status}}{{else}}{{.State.Status}}{{end}}' "${container_name}" 2>/dev/null || true)
    [[ "${status}" == "healthy" || "${status}" == "running" ]] && return 0
    sleep 2
  done
  return 1
}

rollback_images() {
  local failed_status=$?
  trap - ERR
  logger -t cashflowpoly-deploy "deployment_failed sha=${RELEASE_SHA} status=${failed_status}" || true

  if [[ -n "${PREVIOUS_DIR}" && -n "${PREVIOUS_SHA}" ]]; then
    rm -f "${PREVIOUS_DIR}/infra/nginx/maintenance/enabled"
    export RELEASE_SHA=${PREVIOUS_SHA}
    local previous_compose=(
      docker compose
      --project-name "${COMPOSE_PROJECT_NAME}"
      --env-file "${ENV_FILE}"
      -f "${PREVIOUS_DIR}/infra/docker/docker-compose.yml"
      -f "${PREVIOUS_DIR}/infra/docker/docker-compose.prod.yml"
      --profile tunnel
    )
    if "${previous_compose[@]}" up -d --no-build --force-recreate db api ui nginx cloudflared; then
      ln -sfn "${PREVIOUS_DIR}" "${CURRENT_LINK}"
    else
      echo "Rollback image gagal; periksa status container sebelum mencoba lagi." >&2
    fi
  fi

  exit "${failed_status}"
}
trap rollback_images ERR

install_journald_retention
"${COMPOSE[@]}" pull db nginx cloudflared
"${COMPOSE[@]}" build api
"${COMPOSE[@]}" build ui

if [[ -n "${PREVIOUS_DIR}" ]]; then
  touch "${PREVIOUS_DIR}/infra/nginx/maintenance/enabled"
fi

"${COMPOSE[@]}" up -d --no-recreate db
wait_for_container_health cashflowpoly-db 45
"${COMPOSE[@]}" run --rm --no-deps api --migrate-only
"${COMPOSE[@]}" run --rm --no-deps api --recalculate-analytics
"${COMPOSE[@]}" up -d --no-build --force-recreate api ui nginx

wait_for_container_health cashflowpoly-api 45
wait_for_container_health cashflowpoly-ui 45
# Rilis ulang SHA yang sama memakai direktori maintenance yang sama dengan rilis aktif.
# Buka trafik hanya sesudah API/UI siap, sebelum Nginx menguji /health.
rm -f "${RELEASE_DIR}/infra/nginx/maintenance/enabled"
# Compose menunggu Nginx sehat sebelum menyalakan tunnel; maintenance harus dilepas dahulu.
wait_for_container_health cashflowpoly-nginx 30
"${COMPOSE[@]}" up -d --no-build --no-deps --force-recreate cloudflared
wait_for_container_health cashflowpoly-tunnel 30
curl --fail --silent --show-error --max-time 15 --header "Host: ${DOMAIN_HOST}" http://127.0.0.1/health >/dev/null
curl --fail --silent --show-error --max-time 15 --header "Host: ${DOMAIN_HOST}" --header "X-Forwarded-Proto: https" http://127.0.0.1/privacy >/dev/null

ln -sfn "${RELEASE_DIR}" "${CURRENT_LINK}"
# Setelah aktivasi, kegagalan retensi tidak boleh membatalkan rilis yang sudah sehat.
trap - ERR
if [[ -n "${PREVIOUS_DIR}" ]]; then
  rm -f "${PREVIOUS_DIR}/infra/nginx/maintenance/enabled" || echo "Peringatan: penanda maintenance rilis lama belum dibersihkan." >&2
fi

# Rilis ulang SHA aktif harus mempertahankan rilis rollback yang sudah tersimpan.
if [[ "${RELEASE_DIR}" != "${PREVIOUS_DIR}" ]]; then
  for old_release in "${RELEASES_DIR_RESOLVED}"/*; do
    [[ -d "${old_release}" ]] || continue
    old_release_resolved=$(realpath "${old_release}") || { echo "Peringatan: rilis lama tidak dapat diperiksa: ${old_release}" >&2; continue; }
    [[ "${old_release_resolved}" == "${RELEASE_DIR}" || "${old_release_resolved}" == "${PREVIOUS_DIR}" ]] && continue
    case "${old_release_resolved}" in
      "${RELEASES_DIR_RESOLVED}"/*)
        git -C "${REPOSITORY_DIR}" worktree remove --force "${old_release_resolved}" || echo "Peringatan: rilis lama belum dibersihkan: ${old_release_resolved}" >&2
        ;;
      *) echo "Peringatan: rilis lama di luar RELEASES_DIR dilewati: ${old_release_resolved}" >&2 ;;
    esac
  done

  for repository in cashflowpoly-api cashflowpoly-ui; do
    image_tags=$(docker image ls "${repository}" --format '{{.Tag}}') || { echo "Peringatan: daftar image ${repository} tidak dapat diperiksa." >&2; continue; }
    while read -r image_tag; do
      [[ -z "${image_tag}" || "${image_tag}" == "${RELEASE_SHA}" || "${image_tag}" == "${PREVIOUS_SHA}" ]] && continue
      docker image rm "${repository}:${image_tag}" >/dev/null 2>&1 || echo "Peringatan: image lama belum dibersihkan: ${repository}:${image_tag}" >&2
    done <<<"${image_tags}"
  done
fi

logger -t cashflowpoly-deploy "deployment_success sha=${RELEASE_SHA} previous_sha=${PREVIOUS_SHA:-none}" || true
echo "Deployment berhasil: ${RELEASE_SHA}"
