#!/usr/bin/env bash
# Fungsi file: Menjalankan deployment production terkunci, bermigrasi maju, dan rollback image jika health check gagal.
set -Eeuo pipefail

if [[ ${EUID} -ne 0 ]]; then
  echo "Deployment wajib dijalankan sebagai root." >&2
  exit 1
fi

APP_ROOT=${APP_ROOT:-/opt/cashflowpoly}
REPOSITORY_DIR=${REPOSITORY_DIR:-${APP_ROOT}/repository}
RELEASES_DIR=${RELEASES_DIR:-${APP_ROOT}/releases}
ENV_FILE=${ENV_FILE:-${APP_ROOT}/shared/.env.prod}
LOCK_FILE=${LOCK_FILE:-/var/lock/cashflowpoly-deploy.lock}
BRANCH=${BRANCH:-prod}

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
RELEASE_DIR="${RELEASES_DIR}/${RELEASE_SHA}"
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
export DATABASE_MIGRATIONS_SEED_SIMULATION=true
COMPOSE=(
  docker compose
  --project-name cashflowpoly
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
  logger -t cashflowpoly-deploy "deployment_failed sha=${RELEASE_SHA} status=${failed_status}"

  if [[ -n "${PREVIOUS_DIR}" && -n "${PREVIOUS_SHA}" ]]; then
    rm -f "${PREVIOUS_DIR}/infra/nginx/maintenance/enabled"
    export RELEASE_SHA=${PREVIOUS_SHA}
    local previous_compose=(
      docker compose
      --project-name cashflowpoly
      --env-file "${ENV_FILE}"
      -f "${PREVIOUS_DIR}/infra/docker/docker-compose.yml"
      -f "${PREVIOUS_DIR}/infra/docker/docker-compose.prod.yml"
      --profile tunnel
    )
    "${previous_compose[@]}" up -d --no-build db api ui nginx cloudflared || true
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

"${COMPOSE[@]}" up -d db
wait_for_container_health cashflowpoly-db 45
"${COMPOSE[@]}" run --rm --no-deps api --migrate-only
"${COMPOSE[@]}" run --rm --no-deps api --recalculate-analytics
"${COMPOSE[@]}" up -d --no-build api ui nginx cloudflared

wait_for_container_health cashflowpoly-api 45
wait_for_container_health cashflowpoly-ui 45
wait_for_container_health cashflowpoly-nginx 30
wait_for_container_health cashflowpoly-tunnel 30
curl --fail --silent --show-error --max-time 15 --header "Host: ${DOMAIN_HOST}" http://127.0.0.1/health >/dev/null
curl --fail --silent --show-error --max-time 15 --header "Host: ${DOMAIN_HOST}" http://127.0.0.1/privacy >/dev/null

ln -sfn "${RELEASE_DIR}" "${CURRENT_LINK}"
if [[ -n "${PREVIOUS_DIR}" ]]; then
  rm -f "${PREVIOUS_DIR}/infra/nginx/maintenance/enabled"
fi

mapfile -t old_releases < <(find "${RELEASES_DIR_RESOLVED}" -mindepth 1 -maxdepth 1 -type d -printf '%T@ %p\n' | sort -rn | tail -n +3 | cut -d' ' -f2-)
for old_release in "${old_releases[@]}"; do
  old_release_resolved=$(realpath "${old_release}")
  case "${old_release_resolved}" in
    "${RELEASES_DIR_RESOLVED}"/*) git -C "${REPOSITORY_DIR}" worktree remove --force "${old_release_resolved}" ;;
    *) echo "Rilis lama di luar RELEASES_DIR ditolak: ${old_release_resolved}" >&2; exit 1 ;;
  esac
done

for repository in cashflowpoly-api cashflowpoly-ui; do
  while read -r image_tag; do
    [[ -z "${image_tag}" || "${image_tag}" == "${RELEASE_SHA}" || "${image_tag}" == "${PREVIOUS_SHA}" ]] && continue
    docker image rm "${repository}:${image_tag}" >/dev/null 2>&1 || true
  done < <(docker image ls "${repository}" --format '{{.Tag}}')
done

logger -t cashflowpoly-deploy "deployment_success sha=${RELEASE_SHA} previous_sha=${PREVIOUS_SHA:-none}"
echo "Deployment berhasil: ${RELEASE_SHA}"
