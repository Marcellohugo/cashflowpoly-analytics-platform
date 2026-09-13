#!/usr/bin/env bash
# Fungsi file: Menguji alur deployment di container terisolasi dengan Git, Docker, dan HTTP palsu.
set -Eeuo pipefail

# Jalankan dalam container disposable, dengan source terpasang read-only dan --network none.
[[ -f /.dockerenv && ${EUID} -eq 0 ]] || { echo "Uji ini hanya boleh dijalankan sebagai root di container disposable." >&2; exit 1; }
SCRIPT=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/../.." && pwd)/scripts/deploy-production.sh
TEST_ROOT=$(mktemp -d)
trap 'rm -rf -- "${TEST_ROOT}"' EXIT
mkdir -p "${TEST_ROOT}/bin"

cat >"${TEST_ROOT}/bin/git" <<'EOF'
#!/usr/bin/env bash
set -euo pipefail
[[ $1 == -C ]]
repository=$2
shift 2
case $1 in
  fetch) ;;
  rev-parse)
    if [[ $2 == HEAD ]]; then cat "${repository}/.test-sha"; else echo "${TEST_RELEASE_SHA}"; fi
    ;;
  worktree)
    if [[ $2 == add ]]; then
      mkdir -p "$4/infra/nginx/maintenance"
      echo "$5" >"$4/.test-sha"
    else
      [[ ${FAIL_CLEANUP:-false} != true ]] || exit 42
      case $4 in "${APP_ROOT}/releases/"*) rm -rf -- "$4" ;; *) exit 1 ;; esac
    fi
    ;;
  *) exit 1 ;;
esac
EOF

cat >"${TEST_ROOT}/bin/docker" <<'EOF'
#!/usr/bin/env bash
set -euo pipefail
echo "docker ${RELEASE_SHA:-none} $*" >>"${TEST_LOG}"
case $1 in
  compose)
    if [[ " $* " == *' up '* ]]; then
      if [[ " $* " == *' api '* ]]; then
        echo "${RELEASE_SHA}" >"${TEST_ACTIVE}"
      fi
      # Compose waits for nginx:service_healthy before starting cloudflared; the
      # command cannot return while this release still serves maintenance (503).
      if [[ " $* " == *' cloudflared '* && " $* " != *' --no-deps '* ]]; then
        active=$(cat "${TEST_ACTIVE}")
        if [[ -f "${APP_ROOT}/releases/${active}/infra/nginx/maintenance/enabled" ]]; then
          echo "Compose: dependency nginx is unhealthy" >&2
          exit 43
        fi
      fi
    elif [[ " $* " == *' run '* && -n ${TEST_PREVIOUS_SHA} ]]; then
      [[ -f "${APP_ROOT}/releases/${TEST_PREVIOUS_SHA}/infra/nginx/maintenance/enabled" ]]
      echo "maintenance_during_migration" >>"${TEST_LOG}"
    fi
    ;;
  inspect)
    active=$(cat "${TEST_ACTIVE}" 2>/dev/null || true)
    if [[ ${*: -1} == cashflowpoly-nginx && -f "${APP_ROOT}/releases/${active}/infra/nginx/maintenance/enabled" ]]; then
      echo unhealthy
    else
      echo healthy
    fi
    ;;
  image)
    if [[ $2 == ls ]]; then
      printf '%s\n' "${TEST_RELEASE_SHA}" "${TEST_PREVIOUS_SHA}" old-image
    elif [[ ${FAIL_CLEANUP:-false} == true ]]; then
      exit 42
    fi
    ;;
  *) exit 1 ;;
esac
EOF

cat >"${TEST_ROOT}/bin/curl" <<'EOF'
#!/usr/bin/env bash
set -euo pipefail
active=$(cat "${TEST_ACTIVE}")
[[ ! -f "${APP_ROOT}/releases/${active}/infra/nginx/maintenance/enabled" ]]
[[ ${FAIL_SMOKE_SHA:-none} != "${active}" ]] || exit 42
echo "curl $*" >>"${TEST_LOG}"
EOF

cat >"${TEST_ROOT}/bin/logger" <<'EOF'
#!/usr/bin/env bash
echo "$*" >>"${TEST_LOG}"
EOF
for executable in install systemctl sleep; do
  printf '#!/usr/bin/env bash\nexit 0\n' >"${TEST_ROOT}/bin/${executable}"
done
chmod +x "${TEST_ROOT}/bin/"*
export PATH="${TEST_ROOT}/bin:${PATH}"

prepare_case() {
  local name=$1
  export APP_ROOT="${TEST_ROOT}/${name}/app"
  export REPOSITORY_DIR="${TEST_ROOT}/${name}/repository"
  export ENV_FILE="${REPOSITORY_DIR}/config/env/.env.prod"
  export LOCK_FILE="${TEST_ROOT}/${name}/deploy.lock"
  export TEST_LOG="${TEST_ROOT}/${name}/commands.log"
  export TEST_ACTIVE="${TEST_ROOT}/${name}/active-sha"
  export TEST_PREVIOUS_SHA=previous TEST_RELEASE_SHA=candidate
  unset FAIL_CLEANUP FAIL_SMOKE_SHA
  mkdir -p "${REPOSITORY_DIR}/.git" "${REPOSITORY_DIR}/scripts" "$(dirname "${ENV_FILE}")" "${APP_ROOT}/releases/previous/infra/nginx/maintenance" "${APP_ROOT}/releases/obsolete"
  cp "${SCRIPT}" "${REPOSITORY_DIR}/scripts/deploy-production.sh"
  printf 'DOMAIN=example.invalid\nDATABASE_MIGRATIONS_SEED_SIMULATION=true\n' >"${ENV_FILE}"
  echo previous >"${APP_ROOT}/releases/previous/.test-sha"
  ln -s "${APP_ROOT}/releases/previous" "${APP_ROOT}/current"
  echo previous >"${TEST_ACTIVE}"
}

run_deployment() {
  bash "${REPOSITORY_DIR}/scripts/deploy-production.sh" >"${APP_ROOT}/test-output.log" 2>&1
}

assert_active() {
  [[ $(readlink -f "${APP_ROOT}/current") == "${APP_ROOT}/releases/$1" ]]
  [[ $(cat "${TEST_ACTIVE}") == "$1" ]]
  [[ ! -f "${APP_ROOT}/releases/$1/infra/nginx/maintenance/enabled" ]]
}

prepare_case same-sha
export TEST_RELEASE_SHA=previous
run_deployment
assert_active previous
[[ $(grep -c maintenance_during_migration "${TEST_LOG}") == 2 ]]
! grep -q deployment_failed "${TEST_LOG}"
[[ -d "${APP_ROOT}/releases/obsolete" ]]
! grep -q 'image rm' "${TEST_LOG}"
echo 'PASS: SHA aktif dapat di-deploy ulang tanpa deadlock health dependency Nginx/Cloudflared dan tetap menyimpan rilis rollback.'

prepare_case new-sha
# Rilis kandidat yang pernah gagal boleh masih menyimpan penanda maintenance.
mkdir -p "${APP_ROOT}/releases/candidate/infra/nginx/maintenance"
echo candidate >"${APP_ROOT}/releases/candidate/.test-sha"
touch "${APP_ROOT}/releases/candidate/infra/nginx/maintenance/enabled"
# Default script harus mengikuti checkout dan config/env/.env.prod, bukan path VPS lama.
bash_script="${REPOSITORY_DIR}/scripts/deploy-production.sh"
unset REPOSITORY_DIR ENV_FILE
bash "${bash_script}" >"${APP_ROOT}/test-output.log" 2>&1
assert_active candidate
[[ -d "${APP_ROOT}/releases/previous" && ! -d "${APP_ROOT}/releases/obsolete" ]]
[[ ! -f "${APP_ROOT}/releases/previous/infra/nginx/maintenance/enabled" ]]
echo 'PASS: rilis baru memakai checkout/env aktual, menghapus marker usang, dan mempertahankan rilis sebelumnya.'

prepare_case cleanup-failure
export FAIL_CLEANUP=true
run_deployment
assert_active candidate
[[ -d "${APP_ROOT}/releases/obsolete" ]]
grep -q 'Peringatan: rilis lama belum dibersihkan' "${APP_ROOT}/test-output.log"
grep -q 'Peringatan: image lama belum dibersihkan' "${APP_ROOT}/test-output.log"
! grep -q deployment_failed "${TEST_LOG}"
echo 'PASS: kegagalan cleanup tidak rollback atau mengubah rilis sehat.'

prepare_case smoke-failure
export FAIL_SMOKE_SHA=candidate
status=0
run_deployment || status=$?
[[ ${status} -eq 42 ]]
assert_active previous
grep -q deployment_failed "${TEST_LOG}"
echo 'PASS: kegagalan smoke test mengembalikan image dan penunjuk current ke rilis sebelumnya.'
