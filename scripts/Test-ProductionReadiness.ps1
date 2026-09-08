# Fungsi file: Memvalidasi rahasia, bootstrap akun, domain, TLS tunnel, dan konfigurasi Compose sebelum deployment production.
# Uraian baris: Menjadikan skrip sebagai advanced command PowerShell sehingga pengikatan parameter dan parameter umum mengikuti perilaku cmdlet.
[CmdletBinding()]
# Uraian baris: Mendeklarasikan parameter masukan untuk skrip/fungsi ini; nilai default digunakan jika pemanggil tidak memberikan argumen. Atribut Mandatory, bila ada, mewajibkan pemanggil mengisi parameter tersebut.
param(
    # Uraian baris: Mendeklarasikan parameter EnvironmentFile bertipe string untuk memilih file environment yang menjadi masukan validasi/Compose. Nilai di sebelah kanan menjadi default jika parameter tidak diberikan.
    [string]$EnvironmentFile = "config/env/.env.prod"
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
)

# Uraian baris: Meminta error PowerShell menghentikan alur agar pemeriksaan yang gagal tidak diteruskan sebagai hasil sukses.
$ErrorActionPreference = "Stop"
# Uraian baris: Mengambil direktori induk scripts sebagai root repositori; semua jalur proyek disusun dari lokasi skrip, bukan direktori terminal pemanggil.
$repositoryRoot = Split-Path -Parent $PSScriptRoot
# Uraian baris: Menyimpan hasil ekspresi Join-Path $repositoryRoot $EnvironmentFile ke variabel resolvedEnvironmentFile agar dapat digunakan pada tahap verifikasi selanjutnya.
$resolvedEnvironmentFile = Join-Path $repositoryRoot $EnvironmentFile

# Uraian baris: Mengevaluasi kondisi (-not (Test-Path -LiteralPath $resolvedEnvironmentFile -PathType Leaf)). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
if (-not (Test-Path -LiteralPath $resolvedEnvironmentFile -PathType Leaf)) {
    # Uraian baris: Menghentikan operasi dengan exception berisi pesan "File environment production tidak ditemukan: $resolvedEnvironmentFile". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
    throw "File environment production tidak ditemukan: $resolvedEnvironmentFile"
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}

# Membaca dotenv tanpa mengeksekusi isinya agar nilai rahasia tidak menjadi perintah shell.
# Uraian baris: Menampung konfigurasi dotenv dalam hashtable agar nilai dapat dicari menggunakan nama variabel. Nilai dihitung dari ekspresi di sebelah kanan assignment.
$settings = @{}
# Uraian baris: Mengulangi blok untuk setiap elemen pada ($line in Get-Content -LiteralPath $resolvedEnvironmentFile). Variabel iterasi menunjuk satu tool, entri konfigurasi, atau kontrak yang diperiksa pada putaran berjalan.
foreach ($line in Get-Content -LiteralPath $resolvedEnvironmentFile) {
    # Uraian baris: Membuang whitespace di awal/akhir baris sebelum mengenali komentar, baris kosong, atau pasangan nama dan nilai. Nilai dihitung dari ekspresi di sebelah kanan assignment.
    $trimmed = $line.Trim()
    # Uraian baris: Mengevaluasi kondisi ($trimmed.Length -eq 0 -or $trimmed.StartsWith("#") -or -not $trimmed.Contains("=")). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
    if ($trimmed.Length -eq 0 -or $trimmed.StartsWith("#") -or -not $trimmed.Contains("=")) {
        # Uraian baris: Melewati sisa blok iterasi untuk entri yang tidak perlu diproses, lalu mengambil entri berikutnya; ini mencegah baris kosong/komentar dotenv dianggap sebagai setting.
        continue
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    }

    # Uraian baris: Memisahkan baris dotenv pada tanda sama dengan pertama saja (maksimal dua bagian), sehingga tanda sama dengan di dalam nilai tidak ikut dipecah.
    $name, $value = $trimmed.Split("=", 2)
    # Uraian baris: Menampung konfigurasi dotenv dalam hashtable agar nilai dapat dicari menggunakan nama variabel. Nilai dihitung dari ekspresi di sebelah kanan assignment.
    $settings[$name.Trim()] = $value.Trim()
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}

# Menolak nilai kosong dan placeholder yang aman untuk development tetapi berbahaya di production.
# Uraian baris: Mendaftar variabel wajib produksi yang harus tersedia dan tidak kosong. Nilai dihitung dari ekspresi di sebelah kanan assignment.
$required = @("POSTGRES_PASSWORD", "JWT_SIGNING_KEY", "DOMAIN", "CLOUDFLARE_TUNNEL_TOKEN")
# Uraian baris: Mengulangi blok untuk setiap elemen pada ($name in $required). Variabel iterasi menunjuk satu tool, entri konfigurasi, atau kontrak yang diperiksa pada putaran berjalan.
foreach ($name in $required) {
    # Uraian baris: Mengevaluasi kondisi (-not $settings.ContainsKey($name) -or [string]::IsNullOrWhiteSpace($settings[$name])). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
    if (-not $settings.ContainsKey($name) -or [string]::IsNullOrWhiteSpace($settings[$name])) {
        # Uraian baris: Menghentikan operasi dengan exception berisi pesan "$name wajib diisi untuk deployment production.". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
        throw "$name wajib diisi untuk deployment production."
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    }
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}

# Uraian baris: Menyimpan pola regex case-insensitive untuk menolak kata sandi/kunci yang masih berupa contoh. Nilai dihitung dari ekspresi di sebelah kanan assignment.
$placeholderPattern = "(?i)GANTI_|change-me|dev-local|example|placeholder"
# Uraian baris: Mengevaluasi kondisi ($settings["POSTGRES_PASSWORD"] -eq "cashflowpoly" -or $settings["POSTGRES_PASSWORD"] -match $placeholderPattern). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
if ($settings["POSTGRES_PASSWORD"] -eq "cashflowpoly" -or $settings["POSTGRES_PASSWORD"] -match $placeholderPattern) {
    # Uraian baris: Menghentikan operasi dengan exception berisi pesan "POSTGRES_PASSWORD masih menggunakan nilai development/placeholder.". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
    throw "POSTGRES_PASSWORD masih menggunakan nilai development/placeholder."
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}
# Uraian baris: Mengevaluasi kondisi ($settings["POSTGRES_PASSWORD"].Length -lt 16). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
if ($settings["POSTGRES_PASSWORD"].Length -lt 16) {
    # Uraian baris: Menghentikan operasi dengan exception berisi pesan "POSTGRES_PASSWORD minimal 16 karakter untuk production.". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
    throw "POSTGRES_PASSWORD minimal 16 karakter untuk production."
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}
# Uraian baris: Mengevaluasi kondisi ($settings["JWT_SIGNING_KEY"] -match $placeholderPattern -or $settings["JWT_SIGNING_KEY"].Length -lt 32). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
if ($settings["JWT_SIGNING_KEY"] -match $placeholderPattern -or $settings["JWT_SIGNING_KEY"].Length -lt 32) {
    # Uraian baris: Menghentikan operasi dengan exception berisi pesan "JWT_SIGNING_KEY wajib berupa rahasia non-placeholder minimal 32 karakter.". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
    throw "JWT_SIGNING_KEY wajib berupa rahasia non-placeholder minimal 32 karakter."
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}
# Uraian baris: Mengevaluasi kondisi ($settings["DOMAIN"] -match "(?i)^(localhost|127\.0\.0\.1)$"). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
if ($settings["DOMAIN"] -match "(?i)^(localhost|127\.0\.0\.1)$") {
    # Uraian baris: Menghentikan operasi dengan exception berisi pesan "DOMAIN production tidak boleh localhost.". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
    throw "DOMAIN production tidak boleh localhost."
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}

# Uraian baris: Mendefinisikan fungsi Test-BootstrapCredentialPair. Isi blok disimpan untuk dipanggil kembali oleh langkah verifikasi yang membutuhkannya.
function Test-BootstrapCredentialPair {
    # Uraian baris: Mendeklarasikan parameter masukan untuk skrip/fungsi ini; nilai default digunakan jika pemanggil tidak memberikan argumen. Atribut Mandatory, bila ada, mewajibkan pemanggil mengisi parameter tersebut.
    param(
        # Uraian baris: Mendeklarasikan parameter Role bertipe string untuk menyebutkan peran akun pada pesan validasi bootstrap. Nilai diperoleh dari pemanggil fungsi atau skrip.
        [string]$Role,
        # Uraian baris: Mendeklarasikan parameter UsernameKey bertipe string untuk menentukan nama variabel username yang dicari pada dotenv. Nilai diperoleh dari pemanggil fungsi atau skrip.
        [string]$UsernameKey,
        # Uraian baris: Mendeklarasikan parameter PasswordKey bertipe string untuk menentukan nama variabel password yang dicari pada dotenv. Nilai diperoleh dari pemanggil fungsi atau skrip.
        [string]$PasswordKey
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    )

    # Uraian baris: Membaca username dari hashtable jika key tersedia; key yang tidak ada diwakili string kosong. Nilai dihitung dari ekspresi di sebelah kanan assignment.
    $username = if ($settings.ContainsKey($UsernameKey)) { $settings[$UsernameKey] } else { "" }
    # Uraian baris: Membaca password bootstrap jika key tersedia, dengan string kosong sebagai nilai untuk key yang tidak ada. Nilai dihitung dari ekspresi di sebelah kanan assignment.
    $password = if ($settings.ContainsKey($PasswordKey)) { $settings[$PasswordKey] } else { "" }
    # Uraian baris: Menghasilkan boolean apakah username mengandung teks non-whitespace. Nilai dihitung dari ekspresi di sebelah kanan assignment.
    $hasUsername = -not [string]::IsNullOrWhiteSpace($username)
    # Uraian baris: Menghasilkan boolean apakah password mengandung teks non-whitespace. Nilai dihitung dari ekspresi di sebelah kanan assignment.
    $hasPassword = -not [string]::IsNullOrWhiteSpace($password)

    # Uraian baris: Mengevaluasi kondisi ($hasUsername -xor $hasPassword). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
    if ($hasUsername -xor $hasPassword) {
        # Uraian baris: Menghentikan operasi dengan exception berisi pesan "Bootstrap $Role harus mengisi $UsernameKey dan $PasswordKey sebagai pasangan.". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
        throw "Bootstrap $Role harus mengisi $UsernameKey dan $PasswordKey sebagai pasangan."
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    }
    # Uraian baris: Mengevaluasi kondisi (-not $hasUsername). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
    if (-not $hasUsername) {
        # Uraian baris: Mengakhiri fungsi dan mengembalikan $false. false menunjukkan pasangan kredensial tidak diminta karena username/password kosong.
        return $false
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    }
    # Uraian baris: Mengevaluasi kondisi ($password.Length -lt 12 -or [Text.Encoding]::UTF8.GetByteCount($password) -gt 72). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
    if ($password.Length -lt 12 -or [Text.Encoding]::UTF8.GetByteCount($password) -gt 72) {
        # Uraian baris: Menghentikan operasi dengan exception berisi pesan "Password bootstrap $Role harus minimal 12 karakter dan maksimal 72 byte UTF-8.". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
        throw "Password bootstrap $Role harus minimal 12 karakter dan maksimal 72 byte UTF-8."
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    }

    # Uraian baris: Mengakhiri fungsi dan mengembalikan $true. true menandai pasangan kredensial bootstrap yang lengkap dan lolos batas panjang password.
    return $true
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}

# Uraian baris: Menyimpan flag bootstrap sebagai teks; validasi berikutnya hanya menerima true atau false. Nilai dihitung dari ekspresi di sebelah kanan assignment.
$bootstrapEnabled = "false"
# Uraian baris: Mengevaluasi kondisi ($settings.ContainsKey("AUTH_BOOTSTRAP_SEED_DEFAULT_USERS")). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
if ($settings.ContainsKey("AUTH_BOOTSTRAP_SEED_DEFAULT_USERS")) {
    # Uraian baris: Menyimpan flag bootstrap sebagai teks; validasi berikutnya hanya menerima true atau false. Nilai dihitung dari ekspresi di sebelah kanan assignment.
    $bootstrapEnabled = $settings["AUTH_BOOTSTRAP_SEED_DEFAULT_USERS"]
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}
# Uraian baris: Mengevaluasi kondisi ($bootstrapEnabled -notmatch "(?i)^(true|false)$"). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
if ($bootstrapEnabled -notmatch "(?i)^(true|false)$") {
    # Uraian baris: Menghentikan operasi dengan exception berisi pesan "AUTH_BOOTSTRAP_SEED_DEFAULT_USERS harus bernilai true atau false.". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
    throw "AUTH_BOOTSTRAP_SEED_DEFAULT_USERS harus bernilai true atau false."
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}

# Uraian baris: Mengevaluasi kondisi ($bootstrapEnabled -match "(?i)^true$"). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
if ($bootstrapEnabled -match "(?i)^true$") {
    # Uraian baris: Bagian 1 dari 4 baris instruksi berikut. Menyimpan hasil validasi pasangan username/password untuk akun Instruktur awal. Nilai dihitung dari ekspresi di sebelah kanan assignment.
    # Uraian baris: Bagian 2 dari 4 baris instruksi berikut. Memberikan label peran kepada helper bootstrap agar pesan error menyebut akun yang benar.
    # Uraian baris: Bagian 3 dari 4 baris instruksi berikut. Menentukan key dotenv untuk username yang dibaca helper validasi pasangan kredensial.
    # Uraian baris: Bagian 4 dari 4 baris instruksi berikut. Menentukan key dotenv untuk password yang dibaca helper bersama username; baris ini menutup instruksi lanjutan pemanggilan helper.
    $hasInstructorBootstrap = Test-BootstrapCredentialPair `
        -Role "Instruktur" `
        -UsernameKey "AUTH_BOOTSTRAP_INSTRUCTOR_USERNAME" `
        -PasswordKey "AUTH_BOOTSTRAP_INSTRUCTOR_PASSWORD"
    # Uraian baris: Bagian 1 dari 4 baris instruksi berikut. Menyimpan hasil validasi pasangan username/password untuk akun Player awal. Nilai dihitung dari ekspresi di sebelah kanan assignment.
    # Uraian baris: Bagian 2 dari 4 baris instruksi berikut. Memberikan label peran kepada helper bootstrap agar pesan error menyebut akun yang benar.
    # Uraian baris: Bagian 3 dari 4 baris instruksi berikut. Menentukan key dotenv untuk username yang dibaca helper validasi pasangan kredensial.
    # Uraian baris: Bagian 4 dari 4 baris instruksi berikut. Menentukan key dotenv untuk password yang dibaca helper bersama username; baris ini menutup instruksi lanjutan pemanggilan helper.
    $hasPlayerBootstrap = Test-BootstrapCredentialPair `
        -Role "Player" `
        -UsernameKey "AUTH_BOOTSTRAP_PLAYER_USERNAME" `
        -PasswordKey "AUTH_BOOTSTRAP_PLAYER_PASSWORD"

    # Uraian baris: Mengevaluasi kondisi (-not $hasInstructorBootstrap -and -not $hasPlayerBootstrap). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
    if (-not $hasInstructorBootstrap -and -not $hasPlayerBootstrap) {
        # Uraian baris: Menghentikan operasi dengan exception berisi pesan "AUTH_BOOTSTRAP_SEED_DEFAULT_USERS=true membutuhkan minimal satu pasangan credential bootstrap.". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
        throw "AUTH_BOOTSTRAP_SEED_DEFAULT_USERS=true membutuhkan minimal satu pasangan credential bootstrap."
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    }
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}

# Uraian baris: Mengevaluasi kondisi (-not (Get-Command docker -ErrorAction SilentlyContinue)). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    # Uraian baris: Menghentikan operasi dengan exception berisi pesan "Docker CLI tidak tersedia untuk memvalidasi konfigurasi production.". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
    throw "Docker CLI tidak tersedia untuk memvalidasi konfigurasi production."
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}

# Meminta Docker Compose merender konfigurasi final; kegagalan interpolasi atau YAML menghentikan deployment.
# Uraian baris: Bagian 1 dari 6 baris instruksi berikut. Memanggil Docker Compose dengan environment dan file compose yang telah disusun. Baris lanjutan menentukan file konfigurasi serta validasi config -q.
# Uraian baris: Bagian 2 dari 6 baris instruksi berikut. Memberikan jalur dotenv yang sudah diresolusi kepada Docker Compose untuk interpolasi variabel konfigurasi.
# Uraian baris: Bagian 3 dari 6 baris instruksi berikut. Menambahkan file Compose melalui jalur yang dibangun dari root repositori; urutan file menentukan nilai override yang menang.
# Uraian baris: Bagian 4 dari 6 baris instruksi berikut. Menambahkan file Compose melalui jalur yang dibangun dari root repositori; urutan file menentukan nilai override yang menang.
# Uraian baris: Bagian 5 dari 6 baris instruksi berikut. Mengaktifkan profil tunnel sehingga definisi cloudflared turut disertakan dalam pemeriksaan konfigurasi produksi.
# Uraian baris: Bagian 6 dari 6 baris instruksi berikut. Meminta Compose memvalidasi konfigurasi gabungan tanpa mencetak isinya; exit code bukan nol menandakan interpolasi atau konfigurasi tidak valid.
& docker compose `
    --env-file $resolvedEnvironmentFile `
    -f (Join-Path $repositoryRoot "infra/docker/docker-compose.yml") `
    -f (Join-Path $repositoryRoot "infra/docker/docker-compose.prod.yml") `
    --profile tunnel `
    config -q
# Uraian baris: Mengevaluasi kondisi ($LASTEXITCODE -ne 0). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
if ($LASTEXITCODE -ne 0) {
    # Uraian baris: Menghentikan operasi dengan exception berisi pesan "Konfigurasi Docker Compose production tidak valid.". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
    throw "Konfigurasi Docker Compose production tidak valid."
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}

# Uraian baris: Menampilkan "Konfigurasi production valid: rahasia, bootstrap akun, domain, tunnel TLS, dan Docker Compose siap digunakan.". Keluaran ini memberi hasil/progres verifikasi kepada operator tanpa mengubah berkas konfigurasi.
Write-Output "Konfigurasi production valid: rahasia, bootstrap akun, domain, tunnel TLS, dan Docker Compose siap digunakan."
