#!/usr/bin/env bash
# Fungsi file: Menjalankan deployment production terkunci, bermigrasi maju, dan rollback image jika health check gagal.
# Uraian baris: Mengaktifkan errtrace, penghentian saat error, penolakan variabel tak terdefinisi, dan kegagalan pipeline bila salah satu tahap gagal. Kebijakan ini membuat kegagalan deployment diteruskan ke penanganan error.
set -Eeuo pipefail

# Uraian baris: Menguji kondisi if [[ ${EUID} -ne 0 ]]. Cabang atau operator ||/&& pada baris ini menentukan apakah proses boleh lanjut, harus menolak prasyarat, atau melewati item yang masih dipakai.
if [[ ${EUID} -ne 0 ]]; then
  # Uraian baris: Menampilkan pesan "Deployment wajib dijalankan sebagai root." >&2. Pengalihan >&2 mengirim penjelasan kegagalan ke stderr agar terpisah dari keluaran sukses.
  echo "Deployment wajib dijalankan sebagai root." >&2
  # Uraian baris: Menghentikan deployment dengan kode gagal karena prasyarat pada cabang ini tidak terpenuhi; operasi setelah cabang tidak dijalankan.
  exit 1
# Uraian baris: Menutup percabangan if/elif; eksekusi kembali pada urutan instruksi setelah keputusan tersebut.
fi

# Uraian baris: Menetapkan APP_ROOT sebagai direktori dasar aplikasi dengan default /opt/cashflowpoly. Ekspansi :- memakai default hanya ketika environment belum menyediakan nilai.
APP_ROOT=${APP_ROOT:-/opt/cashflowpoly}
# Uraian baris: Menetapkan REPOSITORY_DIR sebagai checkout Git induk yang menyediakan objek commit dan worktree rilis. Ekspansi :- memakai default hanya ketika environment belum menyediakan nilai.
REPOSITORY_DIR=${REPOSITORY_DIR:-${APP_ROOT}/repository}
# Uraian baris: Menetapkan RELEASES_DIR sebagai direktori penampung checkout per commit. Ekspansi :- memakai default hanya ketika environment belum menyediakan nilai.
RELEASES_DIR=${RELEASES_DIR:-${APP_ROOT}/releases}
# Uraian baris: Menetapkan ENV_FILE sebagai jalur dotenv produksi bersama di luar checkout per rilis. Ekspansi :- memakai default hanya ketika environment belum menyediakan nilai.
ENV_FILE=${ENV_FILE:-${APP_ROOT}/shared/.env.prod}
# Uraian baris: Menetapkan LOCK_FILE sebagai jalur pengunci yang mencegah deployment paralel. Ekspansi :- memakai default hanya ketika environment belum menyediakan nilai.
LOCK_FILE=${LOCK_FILE:-/var/lock/cashflowpoly-deploy.lock}
# Uraian baris: Menetapkan BRANCH sebagai branch remote yang dirilis, default prod. Ekspansi :- memakai default hanya ketika environment belum menyediakan nilai.
BRANCH=${BRANCH:-prod}
# Uraian baris: Menetapkan COMPOSE_PROJECT_NAME sebagai nama proyek Compose yang stabil antar-rilis. Ekspansi :- memakai default hanya ketika environment belum menyediakan nilai.
COMPOSE_PROJECT_NAME=${COMPOSE_PROJECT_NAME:-cashflowpoly-analytics-platform}

# Uraian baris: Membuat direktori "${APP_ROOT}" "${RELEASES_DIR}" "$(dirname "${ENV_FILE}")" beserta induknya yang belum ada. Direktori tersebut menampung rilis/environment atau konfigurasi journald sesuai konteks fungsi.
mkdir -p "${APP_ROOT}" "${RELEASES_DIR}" "$(dirname "${ENV_FILE}")"
# Uraian baris: Menetapkan APP_ROOT_RESOLVED sebagai jalur absolut nyata root aplikasi hasil realpath. Command substitution mengambil keluaran perintah yang disebut di sisi kanan.
APP_ROOT_RESOLVED=$(realpath "${APP_ROOT}")
# Uraian baris: Menetapkan RELEASES_DIR_RESOLVED sebagai jalur absolut nyata folder rilis untuk pemeriksaan containment. Command substitution mengambil keluaran perintah yang disebut di sisi kanan.
RELEASES_DIR_RESOLVED=$(realpath "${RELEASES_DIR}")
# Uraian baris: Membandingkan jalur "${RELEASES_DIR_RESOLVED}" dengan pola pada cabang berikutnya untuk memeriksa apakah lokasinya tetap berada pada direktori yang diharapkan.
case "${RELEASES_DIR_RESOLVED}" in
  # Uraian baris: Menerima jalur yang berada di bawah direktori absolut yang telah diverifikasi. Cabang kosong dengan ;; menandai bahwa pemeriksaan lokasi berhasil.
  "${APP_ROOT_RESOLVED}"/*) ;;
  # Uraian baris: Cabang default untuk jalur yang tidak cocok dengan pola direktori yang diizinkan; menampilkan alasan penolakan ke stderr dan menghentikan deployment.
  *) echo "RELEASES_DIR wajib berada di dalam APP_ROOT." >&2; exit 1 ;;
# Uraian baris: Menutup pemilihan pola case setelah jalur absolut dinilai termasuk direktori rilis yang diizinkan atau ditolak.
esac

# Uraian baris: Membuka lock file untuk ditulis melalui file descriptor 9. Descriptor tetap hidup dalam proses deployment agar flock dapat mempertahankan kunci sampai proses selesai.
exec 9>"${LOCK_FILE}"
# Uraian baris: Mencoba mengambil kunci eksklusif descriptor 9 tanpa menunggu. Jika deployment lain sudah memegangnya, cabang ini menolak menjalankan deployment bersamaan.
if ! flock -n 9; then
  # Uraian baris: Menampilkan pesan "Deployment lain masih berjalan." >&2. Pengalihan >&2 mengirim penjelasan kegagalan ke stderr agar terpisah dari keluaran sukses.
  echo "Deployment lain masih berjalan." >&2
  # Uraian baris: Menghentikan deployment dengan kode gagal karena prasyarat pada cabang ini tidak terpenuhi; operasi setelah cabang tidak dijalankan.
  exit 1
# Uraian baris: Menutup percabangan if/elif; eksekusi kembali pada urutan instruksi setelah keputusan tersebut.
fi

# Uraian baris: Memulai perulangan for command_name in git docker curl logger realpath. Setiap item diproses satu kali untuk memeriksa tool atau membersihkan rilis/image yang tidak lagi dipakai.
for command_name in git docker curl logger realpath; do
  # Uraian baris: Memastikan executable tool pada iterasi ini ditemukan dalam PATH; kegagalan membuka blok penolakan agar deployment tidak berhenti di tengah akibat tool hilang.
  command -v "${command_name}" >/dev/null || {
    # Uraian baris: Menampilkan pesan "Perintah ${command_name} tidak tersedia." >&2. Pengalihan >&2 mengirim penjelasan kegagalan ke stderr agar terpisah dari keluaran sukses.
    echo "Perintah ${command_name} tidak tersedia." >&2
    # Uraian baris: Menghentikan deployment dengan kode gagal karena prasyarat pada cabang ini tidak terpenuhi; operasi setelah cabang tidak dijalankan.
    exit 1
  # Uraian baris: Menutup definisi fungsi/blok atau array argumen; batas ini menentukan scope sintaks sebelum instruksi induk diteruskan.
  }
# Uraian baris: Menutup badan perulangan dan kembali ke pemeriksaan/elemen berikutnya sampai seluruh item atau batas percobaan selesai.
done

# Uraian baris: Menguji kondisi [[ -d "${REPOSITORY_DIR}/.git" ]] || {. Cabang atau operator ||/&& pada baris ini menentukan apakah proses boleh lanjut, harus menolak prasyarat, atau melewati item yang masih dipakai.
[[ -d "${REPOSITORY_DIR}/.git" ]] || {
  # Uraian baris: Menampilkan pesan "Repository deployment tidak ditemukan: ${REPOSITORY_DIR}" >&2. Pengalihan >&2 mengirim penjelasan kegagalan ke stderr agar terpisah dari keluaran sukses.
  echo "Repository deployment tidak ditemukan: ${REPOSITORY_DIR}" >&2
  # Uraian baris: Menghentikan deployment dengan kode gagal karena prasyarat pada cabang ini tidak terpenuhi; operasi setelah cabang tidak dijalankan.
  exit 1
# Uraian baris: Menutup definisi fungsi/blok atau array argumen; batas ini menentukan scope sintaks sebelum instruksi induk diteruskan.
}
# Uraian baris: Menguji kondisi [[ -f "${ENV_FILE}" ]] || {. Cabang atau operator ||/&& pada baris ini menentukan apakah proses boleh lanjut, harus menolak prasyarat, atau melewati item yang masih dipakai.
[[ -f "${ENV_FILE}" ]] || {
  # Uraian baris: Menampilkan pesan "Environment production tidak ditemukan: ${ENV_FILE}" >&2. Pengalihan >&2 mengirim penjelasan kegagalan ke stderr agar terpisah dari keluaran sukses.
  echo "Environment production tidak ditemukan: ${ENV_FILE}" >&2
  # Uraian baris: Menghentikan deployment dengan kode gagal karena prasyarat pada cabang ini tidak terpenuhi; operasi setelah cabang tidak dijalankan.
  exit 1
# Uraian baris: Menutup definisi fungsi/blok atau array argumen; batas ini menentukan scope sintaks sebelum instruksi induk diteruskan.
}

# Uraian baris: Menetapkan DOMAIN_HOST sebagai hostname yang nanti diisi dari dotenv. Nilai disimpan untuk dipakai pada langkah deployment berikutnya.
DOMAIN_HOST=""
# Uraian baris: Memulai perulangan while IFS='=' read -r env_name env_value. read -r membaca dotenv sebagai data tanpa mengeksekusinya; pemisah = memisahkan nama dari nilai.
while IFS='=' read -r env_name env_value; do
  # Uraian baris: Menguji kondisi if [[ "${env_name}" == "DOMAIN" ]]. Cabang atau operator ||/&& pada baris ini menentukan apakah proses boleh lanjut, harus menolak prasyarat, atau melewati item yang masih dipakai.
  if [[ "${env_name}" == "DOMAIN" ]]; then
    # Uraian baris: Membuang karakter carriage return terakhir dari nilai DOMAIN yang dibaca, sehingga dotenv dengan line ending Windows dapat dipakai sebagai hostname.
    DOMAIN_HOST=${env_value%$'\r'}
    # Uraian baris: Menghapus satu tanda kutip ganda di awal nilai DOMAIN jika ada; operator # membuang prefiks yang cocok.
    DOMAIN_HOST=${DOMAIN_HOST#\"}
    # Uraian baris: Menghapus satu tanda kutip ganda di akhir nilai DOMAIN jika ada; operator % membuang sufiks yang cocok.
    DOMAIN_HOST=${DOMAIN_HOST%\"}
    # Uraian baris: Menghapus tanda kutip tunggal pembuka dari nilai DOMAIN bila dotenv menggunakan nilai berpetik tunggal.
    DOMAIN_HOST=${DOMAIN_HOST#\'}
    # Uraian baris: Menghapus tanda kutip tunggal penutup agar hostname yang dipakai header HTTP tidak mengandung pembungkus kutip.
    DOMAIN_HOST=${DOMAIN_HOST%\'}
  # Uraian baris: Menutup percabangan if/elif; eksekusi kembali pada urutan instruksi setelah keputusan tersebut.
  fi
# Uraian baris: Menutup loop dan memasok input dari <"${ENV_FILE}". Redirection/process substitution mengalirkan baris file atau daftar image ke perulangan read.
done <"${ENV_FILE}"
# Uraian baris: Menguji kondisi [[ "${DOMAIN_HOST}" =~ ^[A-Za-z0-9.-]+(:[0-9]+)?$ ]] || {. Cabang atau operator ||/&& pada baris ini menentukan apakah proses boleh lanjut, harus menolak prasyarat, atau melewati item yang masih dipakai.
[[ "${DOMAIN_HOST}" =~ ^[A-Za-z0-9.-]+(:[0-9]+)?$ ]] || {
  # Uraian baris: Menampilkan pesan "DOMAIN pada environment production wajib berupa hostname yang valid." >&2. Pengalihan >&2 mengirim penjelasan kegagalan ke stderr agar terpisah dari keluaran sukses.
  echo "DOMAIN pada environment production wajib berupa hostname yang valid." >&2
  # Uraian baris: Menghentikan deployment dengan kode gagal karena prasyarat pada cabang ini tidak terpenuhi; operasi setelah cabang tidak dijalankan.
  exit 1
# Uraian baris: Menutup definisi fungsi/blok atau array argumen; batas ini menentukan scope sintaks sebelum instruksi induk diteruskan.
}

# Uraian baris: Mengambil referensi branch deployment dari origin dan membersihkan referensi remote yang sudah hilang; checkout rilis nantinya menggunakan commit hasil fetch ini.
git -C "${REPOSITORY_DIR}" fetch --prune origin "${BRANCH}"
# Uraian baris: Menetapkan RELEASE_SHA sebagai SHA commit origin/branch yang telah diverifikasi sebagai objek commit. Command substitution mengambil keluaran perintah yang disebut di sisi kanan.
RELEASE_SHA=$(git -C "${REPOSITORY_DIR}" rev-parse "origin/${BRANCH}^{commit}")
# Uraian baris: Menetapkan RELEASE_DIR sebagai direktori rilis unik berdasarkan SHA commit. Nilai disimpan untuk dipakai pada langkah deployment berikutnya.
RELEASE_DIR="${RELEASES_DIR}/${RELEASE_SHA}"
# Uraian baris: Menetapkan CURRENT_LINK sebagai jalur symlink current yang menunjuk rilis aktif. Nilai disimpan untuk dipakai pada langkah deployment berikutnya.
CURRENT_LINK="${APP_ROOT}/current"
# Uraian baris: Menetapkan PREVIOUS_DIR sebagai direktori rilis sebelumnya, kosong bila belum ada rilis aktif. Nilai disimpan untuk dipakai pada langkah deployment berikutnya.
PREVIOUS_DIR=""
# Uraian baris: Menetapkan PREVIOUS_SHA sebagai commit rilis sebelumnya, kosong sebelum symlink aktif diperiksa. Nilai disimpan untuk dipakai pada langkah deployment berikutnya.
PREVIOUS_SHA=""

# Uraian baris: Menguji kondisi if [[ -L "${CURRENT_LINK}" ]]. Cabang atau operator ||/&& pada baris ini menentukan apakah proses boleh lanjut, harus menolak prasyarat, atau melewati item yang masih dipakai.
if [[ -L "${CURRENT_LINK}" ]]; then
  # Uraian baris: Menetapkan PREVIOUS_DIR sebagai direktori rilis sebelumnya, kosong bila belum ada rilis aktif. Command substitution mengambil keluaran perintah yang disebut di sisi kanan.
  PREVIOUS_DIR=$(readlink -f "${CURRENT_LINK}")
  # Uraian baris: Membandingkan jalur "${PREVIOUS_DIR}" dengan pola pada cabang berikutnya untuk memeriksa apakah lokasinya tetap berada pada direktori yang diharapkan.
  case "${PREVIOUS_DIR}" in
    # Uraian baris: Menerima jalur yang berada di bawah direktori absolut yang telah diverifikasi. Nama direktori dipakai sebagai identitas commit rilis sebelumnya.
    "${RELEASES_DIR_RESOLVED}"/*) PREVIOUS_SHA=$(basename "${PREVIOUS_DIR}") ;;
    # Uraian baris: Cabang default untuk jalur yang tidak cocok dengan pola direktori yang diizinkan; menampilkan alasan penolakan ke stderr dan menghentikan deployment.
    *) echo "Symlink current berada di luar direktori rilis." >&2; exit 1 ;;
  # Uraian baris: Menutup pemilihan pola case setelah jalur absolut dinilai termasuk direktori rilis yang diizinkan atau ditolak.
  esac
# Uraian baris: Menutup percabangan if/elif; eksekusi kembali pada urutan instruksi setelah keputusan tersebut.
fi

# Uraian baris: Menguji kondisi if [[ ! -d "${RELEASE_DIR}" ]]. Cabang atau operator ||/&& pada baris ini menentukan apakah proses boleh lanjut, harus menolak prasyarat, atau melewati item yang masih dipakai.
if [[ ! -d "${RELEASE_DIR}" ]]; then
  # Uraian baris: Membuat checkout Git terpisah pada direktori rilis dengan HEAD detached ke commit RELEASE_SHA, sehingga setiap rilis memiliki sumber tetapnya sendiri.
  git -C "${REPOSITORY_DIR}" worktree add --detach "${RELEASE_DIR}" "${RELEASE_SHA}"
# Uraian baris: Menguji kondisi elif [[ $(git -C "${RELEASE_DIR}" rev-parse HEAD 2>/dev/null || true) != "${RELEASE_SHA}" ]]. Cabang atau operator ||/&& pada baris ini menentukan apakah proses boleh lanjut, harus menolak prasyarat, atau melewati item yang masih dipakai.
elif [[ $(git -C "${RELEASE_DIR}" rev-parse HEAD 2>/dev/null || true) != "${RELEASE_SHA}" ]]; then
  # Uraian baris: Menampilkan pesan "Direktori rilis sudah ada tetapi tidak berisi commit ${RELEASE_SHA}." >&2. Pengalihan >&2 mengirim penjelasan kegagalan ke stderr agar terpisah dari keluaran sukses.
  echo "Direktori rilis sudah ada tetapi tidak berisi commit ${RELEASE_SHA}." >&2
  # Uraian baris: Menghentikan deployment dengan kode gagal karena prasyarat pada cabang ini tidak terpenuhi; operasi setelah cabang tidak dijalankan.
  exit 1
# Uraian baris: Menutup percabangan if/elif; eksekusi kembali pada urutan instruksi setelah keputusan tersebut.
fi

# Uraian baris: Meneruskan commit rilis sebagai environment ke Docker Compose sehingga tag image hasil build menunjuk versi sumber yang dipilih.
export RELEASE_SHA
# Uraian baris: Mengaktifkan seed simulasi untuk proses migrasi deployment melalui environment yang diwarisi Compose.
export DATABASE_MIGRATIONS_SEED_SIMULATION=true
# Uraian baris: Memulai array argumen Bash untuk perintah Compose rilis baru; array mempertahankan batas tiap argumen ketika jalur mengandung spasi.
COMPOSE=(
  # Uraian baris: Menambahkan executable docker dan subperintah compose ke array perintah; pemanggilan array akan mempertahankan tiap batas argumen.
  docker compose
  # Uraian baris: Memakai nama proyek Compose yang konsisten agar jaringan, volume, dan layanan tetap dikenali lintas rilis.
  --project-name "${COMPOSE_PROJECT_NAME}"
  # Uraian baris: Memuat dotenv produksi bersama yang sama untuk semua rilis agar konfigurasi tidak bergantung pada direktori checkout.
  --env-file "${ENV_FILE}"
  # Uraian baris: Menambahkan file Compose "${RELEASE_DIR}/infra/docker/docker-compose.yml" ke array argumen; file dasar dibaca sebelum override produksi.
  -f "${RELEASE_DIR}/infra/docker/docker-compose.yml"
  # Uraian baris: Menambahkan file Compose "${RELEASE_DIR}/infra/docker/docker-compose.prod.yml" ke array argumen; file dasar dibaca sebelum override produksi.
  -f "${RELEASE_DIR}/infra/docker/docker-compose.prod.yml"
  # Uraian baris: Mengaktifkan profil tunnel sehingga cloudflared menjadi bagian layanan yang dikelola perintah Compose.
  --profile tunnel
# Uraian baris: Menutup definisi fungsi/blok atau array argumen; batas ini menentukan scope sintaks sebelum instruksi induk diteruskan.
)

# Uraian baris: Memvalidasi konfigurasi Compose gabungan dan substitusi environment sebelum image atau layanan diubah; -q menekan keluaran konfigurasi.
"${COMPOSE[@]}" config -q

# Uraian baris: Mendefinisikan fungsi pemasangan batas retensi journald. Fungsi hanya mengganti konfigurasi sistem bila isinya berbeda lalu me-restart layanan log.
install_journald_retention() {
  # Uraian baris: Menetapkan target sebagai path konfigurasi retensi journald milik aplikasi. Nilai disimpan untuk dipakai pada langkah deployment berikutnya.
  local target=/etc/systemd/journald.conf.d/cashflowpoly.conf
  # Uraian baris: Membuat direktori "$(dirname "${target}")" beserta induknya yang belum ada. Direktori tersebut menampung rilis/environment atau konfigurasi journald sesuai konteks fungsi.
  mkdir -p "$(dirname "${target}")"
  # Uraian baris: Mendeklarasikan jalur file sementara dalam scope fungsi pemasangan konfigurasi journald.
  local temporary
  # Uraian baris: Meminta sistem membuat file sementara yang unik dan menyimpan path-nya untuk menulis konfigurasi journald sebelum dibandingkan/dipasang.
  temporary=$(mktemp)
  # Uraian baris: Bagian 1 dari 6 baris instruksi berikut. Menulis heredoc berpetik ke file sementara. Delimiter berpetik mencegah ekspansi variabel/perintah di dalam konfigurasi journald yang ditulis.
  # Uraian baris: Bagian 2 dari 6 baris instruksi berikut. Isi literal heredoc untuk header bagian Journal dalam konfigurasi systemd-journald. Teks ini ditulis ke file sementara, bukan dieksekusi sebagai perintah Bash.
  # Uraian baris: Bagian 3 dari 6 baris instruksi berikut. Isi konfigurasi journald yang membatasi usia entri log hingga 30 hari; ini adalah data heredoc yang akan dipasang sebagai file konfigurasi.
  # Uraian baris: Bagian 4 dari 6 baris instruksi berikut. Isi konfigurasi yang membatasi pemakaian disk log persisten journald menjadi 512 MiB sesuai satuan systemd.
  # Uraian baris: Bagian 5 dari 6 baris instruksi berikut. Isi konfigurasi yang membatasi log runtime journald menjadi 128 MiB pada penyimpanan sementara.
  # Uraian baris: Bagian 6 dari 6 baris instruksi berikut. Menutup heredoc dengan delimiter persis EOF. Semua baris di antara pembuka dan delimiter merupakan isi file, sehingga komentarnya sengaja diletakkan di luar heredoc.
  cat >"${temporary}" <<'EOF'
[Journal]
MaxRetentionSec=30day
SystemMaxUse=512M
RuntimeMaxUse=128M
EOF
  # Uraian baris: Menguji kondisi if [[ ! -f "${target}" ]] || ! cmp -s "${temporary}" "${target}". Cabang atau operator ||/&& pada baris ini menentukan apakah proses boleh lanjut, harus menolak prasyarat, atau melewati item yang masih dipakai.
  if [[ ! -f "${target}" ]] || ! cmp -s "${temporary}" "${target}"; then
    # Uraian baris: Menyalin konfigurasi sementara ke lokasi journald dengan mode 0644: pemilik dapat menulis, pengguna lain hanya membaca.
    install -m 0644 "${temporary}" "${target}"
    # Uraian baris: Me-restart layanan systemd-journald setelah konfigurasi baru dipasang agar batas retensi dan kapasitas log diterapkan.
    systemctl restart systemd-journald
  # Uraian baris: Menutup percabangan if/elif; eksekusi kembali pada urutan instruksi setelah keputusan tersebut.
  fi
  # Uraian baris: Menghapus "${temporary}" bila tersedia. File sementara tidak diperlukan lagi setelah konfigurasi dipasang/dibandingkan.
  rm -f "${temporary}"
# Uraian baris: Menutup definisi fungsi/blok atau array argumen; batas ini menentukan scope sintaks sebelum instruksi induk diteruskan.
}

# Uraian baris: Mendefinisikan fungsi polling status container dengan nama dan batas percobaan sebagai argumen; hasil 0 berarti siap, 1 berarti batas tunggu habis.
wait_for_container_health() {
  # Uraian baris: Menyimpan argumen pertama sebagai nama container lokal fungsi; docker inspect menggunakan nama ini untuk membaca status layanan yang sedang ditunggu.
  local container_name=$1
  # Uraian baris: Menyimpan argumen kedua sebagai batas percobaan; operator :- memakai nilai 30 jika pemanggil tidak memberikan batas.
  local attempts=${2:-30}
  # Uraian baris: Memulai perulangan for ((attempt = 1; attempt <= attempts; attempt++)). Penghitung dibatasi oleh jumlah percobaan yang diberikan pemanggil.
  for ((attempt = 1; attempt <= attempts; attempt++)); do
    # Uraian baris: Mendeklarasikan variabel lokal untuk status health/running agar hasil polling tidak menimpa variabel milik fungsi lain.
    local status
    # Uraian baris: Menetapkan status sebagai status health container dari docker inspect, atau status proses bila Health tidak tersedia. Command substitution mengambil keluaran perintah yang disebut di sisi kanan.
    status=$(docker inspect --format '{{if .State.Health}}{{.State.Health.Status}}{{else}}{{.State.Status}}{{end}}' "${container_name}" 2>/dev/null || true)
    # Uraian baris: Mengakhiri fungsi dengan sukses bila status container healthy atau running. Untuk container tanpa health check, nilai running berasal dari status proses container.
    [[ "${status}" == "healthy" || "${status}" == "running" ]] && return 0
    # Uraian baris: Memberi jarak dua detik antarpemeriksaan status agar layanan memiliki waktu startup dan Docker tidak dipoll tanpa jeda.
    sleep 2
  # Uraian baris: Menutup badan perulangan dan kembali ke pemeriksaan/elemen berikutnya sampai seluruh item atau batas percobaan selesai.
  done
  # Uraian baris: Mengembalikan kegagalan setelah seluruh percobaan habis, sehingga deployment masuk penanganan error alih-alih dianggap siap.
  return 1
# Uraian baris: Menutup definisi fungsi/blok atau array argumen; batas ini menentukan scope sintaks sebelum instruksi induk diteruskan.
}

# Uraian baris: Mendefinisikan penanganan kegagalan deployment yang mencatat error dan mencoba menjalankan image rilis sebelumnya. Migrasi database yang sudah dilakukan tidak dibalik oleh fungsi ini.
rollback_images() {
  # Uraian baris: Menyimpan exit status perintah yang memicu handler sebelum perintah lain menimpanya, agar status kegagalan asli dapat dikembalikan di akhir rollback.
  local failed_status=$?
  # Uraian baris: Melepas handler ERR ketika rollback sedang berjalan agar error di dalam rollback tidak memanggil rollback berulang secara rekursif.
  trap - ERR
  # Uraian baris: Mencatat "deployment_failed sha=${RELEASE_SHA} status=${failed_status}" pada log sistem dengan tag cashflowpoly-deploy; identitas commit dan status mendukung penelusuran hasil deployment.
  logger -t cashflowpoly-deploy "deployment_failed sha=${RELEASE_SHA} status=${failed_status}"

  # Uraian baris: Menguji kondisi if [[ -n "${PREVIOUS_DIR}" && -n "${PREVIOUS_SHA}" ]]. Cabang atau operator ||/&& pada baris ini menentukan apakah proses boleh lanjut, harus menolak prasyarat, atau melewati item yang masih dipakai.
  if [[ -n "${PREVIOUS_DIR}" && -n "${PREVIOUS_SHA}" ]]; then
    # Uraian baris: Menghapus "${PREVIOUS_DIR}/infra/nginx/maintenance/enabled" bila tersedia. Flag maintenance dilepas agar rilis sebelumnya kembali melayani trafik pada rollback atau setelah pengalihan sukses.
    rm -f "${PREVIOUS_DIR}/infra/nginx/maintenance/enabled"
    # Uraian baris: Mengganti tag image environment menjadi commit sebelumnya sehingga perintah Compose rollback memilih image rilis lama yang sudah dibangun.
    export RELEASE_SHA=${PREVIOUS_SHA}
    # Uraian baris: Menyusun array Compose khusus rilis sebelumnya untuk rollback dengan file konfigurasi dari checkout sebelumnya.
    local previous_compose=(
      # Uraian baris: Menambahkan executable docker dan subperintah compose ke array perintah; pemanggilan array akan mempertahankan tiap batas argumen.
      docker compose
      # Uraian baris: Memakai nama proyek Compose yang konsisten agar jaringan, volume, dan layanan tetap dikenali lintas rilis.
      --project-name "${COMPOSE_PROJECT_NAME}"
      # Uraian baris: Memuat dotenv produksi bersama yang sama untuk semua rilis agar konfigurasi tidak bergantung pada direktori checkout.
      --env-file "${ENV_FILE}"
      # Uraian baris: Menambahkan file Compose "${PREVIOUS_DIR}/infra/docker/docker-compose.yml" ke array argumen; file dasar dibaca sebelum override produksi.
      -f "${PREVIOUS_DIR}/infra/docker/docker-compose.yml"
      # Uraian baris: Menambahkan file Compose "${PREVIOUS_DIR}/infra/docker/docker-compose.prod.yml" ke array argumen; file dasar dibaca sebelum override produksi.
      -f "${PREVIOUS_DIR}/infra/docker/docker-compose.prod.yml"
      # Uraian baris: Mengaktifkan profil tunnel sehingga cloudflared menjadi bagian layanan yang dikelola perintah Compose.
      --profile tunnel
    # Uraian baris: Menutup definisi fungsi/blok atau array argumen; batas ini menentukan scope sintaks sebelum instruksi induk diteruskan.
    )
    # Uraian baris: Menjalankan array Compose up -d --no-build --force-recreate db api ui nginx cloudflared || true. Membangun image aplikasi dari sumber commit rilis yang dipilih.
    "${previous_compose[@]}" up -d --no-build --force-recreate db api ui nginx cloudflared || true
  # Uraian baris: Menutup percabangan if/elif; eksekusi kembali pada urutan instruksi setelah keputusan tersebut.
  fi

  # Uraian baris: Mengakhiri skrip dengan kode kegagalan asli meskipun rollback berhasil; sistem pemanggil tetap mengetahui rilis baru gagal.
  exit "${failed_status}"
# Uraian baris: Menutup definisi fungsi/blok atau array argumen; batas ini menentukan scope sintaks sebelum instruksi induk diteruskan.
}
# Uraian baris: Mendaftarkan rollback_images sebagai handler kegagalan perintah setelah tahap persiapan; strict mode membuat error yang tidak ditangani masuk jalur ini.
trap rollback_images ERR

# Uraian baris: Memanggil pemasangan retensi log sebelum menjalankan container rilis baru agar kebijakan penyimpanan log sistem sudah tersedia.
install_journald_retention
# Uraian baris: Menjalankan array Compose pull db nginx cloudflared. Mengambil image layanan pendukung sesuai versi konfigurasi.
"${COMPOSE[@]}" pull db nginx cloudflared
# Uraian baris: Menjalankan array Compose build api. Membangun image aplikasi dari sumber commit rilis yang dipilih.
"${COMPOSE[@]}" build api
# Uraian baris: Menjalankan array Compose build ui. Membangun image aplikasi dari sumber commit rilis yang dipilih.
"${COMPOSE[@]}" build ui

# Uraian baris: Menguji kondisi if [[ -n "${PREVIOUS_DIR}" ]]. Cabang atau operator ||/&& pada baris ini menentukan apakah proses boleh lanjut, harus menolak prasyarat, atau melewati item yang masih dipakai.
if [[ -n "${PREVIOUS_DIR}" ]]; then
  # Uraian baris: Membuat flag maintenance pada rilis sebelumnya agar reverse proxy dapat menyajikan halaman pemeliharaan selama pergantian layanan.
  touch "${PREVIOUS_DIR}/infra/nginx/maintenance/enabled"
# Uraian baris: Menutup percabangan if/elif; eksekusi kembali pada urutan instruksi setelah keputusan tersebut.
fi

# Uraian baris: Menjalankan array Compose up -d --no-recreate db. Memastikan database berjalan tanpa membuat ulang container yang sudah ada.
"${COMPOSE[@]}" up -d --no-recreate db
# Uraian baris: Menunggu kesiapan cashflowpoly-db dengan maksimal 45 percobaan. Kegagalan fungsi menghentikan rilis dan memicu handler rollback yang sudah terpasang.
wait_for_container_health cashflowpoly-db 45
# Uraian baris: Menjalankan array Compose run --rm --no-deps api --migrate-only. Menjalankan migrasi database satu kali sebelum aplikasi rilis baru dinyalakan.
"${COMPOSE[@]}" run --rm --no-deps api --migrate-only
# Uraian baris: Menjalankan array Compose run --rm --no-deps api --recalculate-analytics. Menghitung ulang proyeksi analitik setelah migrasi/seed selesai.
"${COMPOSE[@]}" run --rm --no-deps api --recalculate-analytics
# Uraian baris: Menjalankan array Compose up -d --no-build --force-recreate api ui nginx cloudflared. Membangun image aplikasi dari sumber commit rilis yang dipilih.
"${COMPOSE[@]}" up -d --no-build --force-recreate api ui nginx cloudflared

# Uraian baris: Menunggu kesiapan cashflowpoly-api dengan maksimal 45 percobaan. Kegagalan fungsi menghentikan rilis dan memicu handler rollback yang sudah terpasang.
wait_for_container_health cashflowpoly-api 45
# Uraian baris: Menunggu kesiapan cashflowpoly-ui dengan maksimal 45 percobaan. Kegagalan fungsi menghentikan rilis dan memicu handler rollback yang sudah terpasang.
wait_for_container_health cashflowpoly-ui 45
# Uraian baris: Menunggu kesiapan cashflowpoly-nginx dengan maksimal 30 percobaan. Kegagalan fungsi menghentikan rilis dan memicu handler rollback yang sudah terpasang.
wait_for_container_health cashflowpoly-nginx 30
# Uraian baris: Menunggu kesiapan cashflowpoly-tunnel dengan maksimal 30 percobaan. Kegagalan fungsi menghentikan rilis dan memicu handler rollback yang sudah terpasang.
wait_for_container_health cashflowpoly-tunnel 30
# Uraian baris: Memeriksa endpoint publik melalui Nginx lokal menggunakan header Host domain deployment. --fail menolak status HTTP gagal dan --max-time 15 membatasi penantian; body dibuang karena hanya keberhasilan akses yang dibutuhkan.
curl --fail --silent --show-error --max-time 15 --header "Host: ${DOMAIN_HOST}" http://127.0.0.1/health >/dev/null
# Uraian baris: Memeriksa endpoint publik melalui Nginx lokal menggunakan header Host domain deployment. --fail menolak status HTTP gagal dan --max-time 15 membatasi penantian; body dibuang karena hanya keberhasilan akses yang dibutuhkan.
curl --fail --silent --show-error --max-time 15 --header "Host: ${DOMAIN_HOST}" http://127.0.0.1/privacy >/dev/null

# Uraian baris: Memperbarui symlink current ke direktori rilis baru setelah health check berhasil. -s membuat symlink, -f mengganti entri lama, dan -n tidak menelusuri symlink tujuan.
ln -sfn "${RELEASE_DIR}" "${CURRENT_LINK}"
# Uraian baris: Menguji kondisi if [[ -n "${PREVIOUS_DIR}" ]]. Cabang atau operator ||/&& pada baris ini menentukan apakah proses boleh lanjut, harus menolak prasyarat, atau melewati item yang masih dipakai.
if [[ -n "${PREVIOUS_DIR}" ]]; then
  # Uraian baris: Menghapus "${PREVIOUS_DIR}/infra/nginx/maintenance/enabled" bila tersedia. Flag maintenance dilepas agar rilis sebelumnya kembali melayani trafik pada rollback atau setelah pengalihan sukses.
  rm -f "${PREVIOUS_DIR}/infra/nginx/maintenance/enabled"
# Uraian baris: Menutup percabangan if/elif; eksekusi kembali pada urutan instruksi setelah keputusan tersebut.
fi

# Uraian baris: Mengumpulkan direktori rilis yang diurutkan menurut waktu modifikasi, melewati dua yang terbaru, lalu menyimpan sisanya sebagai kandidat pembersihan; tiap jalur tetap diverifikasi sebelum Git worktree menghapusnya.
mapfile -t old_releases < <(find "${RELEASES_DIR_RESOLVED}" -mindepth 1 -maxdepth 1 -type d -printf '%T@ %p\n' | sort -rn | tail -n +3 | cut -d' ' -f2-)
# Uraian baris: Memulai perulangan for old_release in "${old_releases[@]}". Setiap item diproses satu kali untuk memeriksa tool atau membersihkan rilis/image yang tidak lagi dipakai.
for old_release in "${old_releases[@]}"; do
  # Uraian baris: Menetapkan old_release_resolved sebagai jalur absolut kandidat rilis lama sebelum dibatasi pada direktori rilis. Command substitution mengambil keluaran perintah yang disebut di sisi kanan.
  old_release_resolved=$(realpath "${old_release}")
  # Uraian baris: Membandingkan jalur "${old_release_resolved}" dengan pola pada cabang berikutnya untuk memeriksa apakah lokasinya tetap berada pada direktori yang diharapkan.
  case "${old_release_resolved}" in
    # Uraian baris: Menerima jalur yang berada di bawah direktori absolut yang telah diverifikasi. Menghapus checkout rilis lama melalui Git worktree hanya pada cabang direktori rilis ini.
    "${RELEASES_DIR_RESOLVED}"/*) git -C "${REPOSITORY_DIR}" worktree remove --force "${old_release_resolved}" ;;
    # Uraian baris: Cabang default untuk jalur yang tidak cocok dengan pola direktori yang diizinkan; menampilkan alasan penolakan ke stderr dan menghentikan deployment.
    *) echo "Rilis lama di luar RELEASES_DIR ditolak: ${old_release_resolved}" >&2; exit 1 ;;
  # Uraian baris: Menutup pemilihan pola case setelah jalur absolut dinilai termasuk direktori rilis yang diizinkan atau ditolak.
  esac
# Uraian baris: Menutup badan perulangan dan kembali ke pemeriksaan/elemen berikutnya sampai seluruh item atau batas percobaan selesai.
done

# Uraian baris: Memulai perulangan for repository in cashflowpoly-api cashflowpoly-ui. Setiap item diproses satu kali untuk memeriksa tool atau membersihkan rilis/image yang tidak lagi dipakai.
for repository in cashflowpoly-api cashflowpoly-ui; do
  # Uraian baris: Memulai perulangan while read -r image_tag. Setiap item diproses satu kali untuk memeriksa tool atau membersihkan rilis/image yang tidak lagi dipakai.
  while read -r image_tag; do
    # Uraian baris: Menguji kondisi [[ -z "${image_tag}" || "${image_tag}" == "${RELEASE_SHA}" || "${image_tag}" == "${PREVIOUS_SHA}" ]] && continue. Cabang atau operator ||/&& pada baris ini menentukan apakah proses boleh lanjut, harus menolak prasyarat, atau melewati item yang masih dipakai.
    [[ -z "${image_tag}" || "${image_tag}" == "${RELEASE_SHA}" || "${image_tag}" == "${PREVIOUS_SHA}" ]] && continue
    # Uraian baris: Menghapus tag image lama yang telah disaring loop agar bukan rilis aktif/sebelumnya. Kegagalan penghapusan diabaikan karena image mungkin masih digunakan container.
    docker image rm "${repository}:${image_tag}" >/dev/null 2>&1 || true
  # Uraian baris: Menutup loop dan memasok input dari < <(docker image ls "${repository}" --format '{{.Tag}}'). Redirection/process substitution mengalirkan baris file atau daftar image ke perulangan read.
  done < <(docker image ls "${repository}" --format '{{.Tag}}')
# Uraian baris: Menutup badan perulangan dan kembali ke pemeriksaan/elemen berikutnya sampai seluruh item atau batas percobaan selesai.
done

# Uraian baris: Mencatat "deployment_success sha=${RELEASE_SHA} previous_sha=${PREVIOUS_SHA:-none}" pada log sistem dengan tag cashflowpoly-deploy; identitas commit dan status mendukung penelusuran hasil deployment.
logger -t cashflowpoly-deploy "deployment_success sha=${RELEASE_SHA} previous_sha=${PREVIOUS_SHA:-none}"
# Uraian baris: Menampilkan pesan "Deployment berhasil: ${RELEASE_SHA}". Pesan akhir menunjukkan commit yang berhasil melewati seluruh proses deployment.
echo "Deployment berhasil: ${RELEASE_SHA}"
