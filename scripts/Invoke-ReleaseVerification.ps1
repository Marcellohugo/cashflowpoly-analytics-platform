# Fungsi file: Menjalankan seluruh gerbang verifikasi lokal sebelum perubahan boleh dirilis.
# Uraian baris: Menjadikan skrip sebagai advanced command PowerShell sehingga pengikatan parameter dan parameter umum mengikuti perilaku cmdlet.
[CmdletBinding()]
# Uraian baris: Mendeklarasikan parameter masukan untuk skrip/fungsi ini; nilai default digunakan jika pemanggil tidak memberikan argumen. Atribut Mandatory, bila ada, mewajibkan pemanggil mengisi parameter tersebut.
param(
    # Uraian baris: Mendeklarasikan parameter SkipBrowser bertipe switch untuk melewati instalasi dan pengujian browser saat switch diaktifkan. Nilai diperoleh dari pemanggil fungsi atau skrip.
    [switch]$SkipBrowser,
    # Uraian baris: Mendeklarasikan parameter SkipDockerBuild bertipe switch untuk melewati build image Docker saat switch diaktifkan. Nilai diperoleh dari pemanggil fungsi atau skrip.
    [switch]$SkipDockerBuild,
    # Uraian baris: Mendeklarasikan parameter SkipPerformance bertipe switch untuk melewati benchmark performa saat switch diaktifkan. Nilai diperoleh dari pemanggil fungsi atau skrip.
    [switch]$SkipPerformance,
    # Uraian baris: Mendeklarasikan parameter EnvironmentFile bertipe string untuk memilih file environment yang menjadi masukan validasi/Compose. Nilai di sebelah kanan menjadi default jika parameter tidak diberikan.
    [string]$EnvironmentFile = "config/env/.env.dev"
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
)

# Uraian baris: Meminta error PowerShell menghentikan alur agar pemeriksaan yang gagal tidak diteruskan sebagai hasil sukses.
$ErrorActionPreference = "Stop"
# Uraian baris: Mengambil direktori induk scripts sebagai root repositori; semua jalur proyek disusun dari lokasi skrip, bukan direktori terminal pemanggil.
$repositoryRoot = Split-Path -Parent $PSScriptRoot
# Uraian baris: Menyimpan hasil ekspresi Join-Path $repositoryRoot "tests/e2e" ke variabel e2eRoot agar dapat digunakan pada tahap verifikasi selanjutnya.
$e2eRoot = Join-Path $repositoryRoot "tests/e2e"
# Uraian baris: Menyimpan hasil ekspresi Join-Path $repositoryRoot $EnvironmentFile ke variabel resolvedEnvironmentFile agar dapat digunakan pada tahap verifikasi selanjutnya.
$resolvedEnvironmentFile = Join-Path $repositoryRoot $EnvironmentFile
# Uraian baris: Menyusun array argumen -f untuk menggabungkan Compose dasar dan override development. Nilai dihitung dari ekspresi di sebelah kanan assignment.
$composeFiles = @(
    # Uraian baris: Menambahkan nilai "-f", (Join-Path $repositoryRoot "infra/docker/docker-compose.yml") ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "-f", (Join-Path $repositoryRoot "infra/docker/docker-compose.yml"),
    # Uraian baris: Menambahkan nilai "-f", (Join-Path $repositoryRoot "infra/docker/docker-compose.watch.yml") ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "-f", (Join-Path $repositoryRoot "infra/docker/docker-compose.watch.yml")
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
)

# Uraian baris: Mendefinisikan fungsi Invoke-Checked. Isi blok disimpan untuk dipanggil kembali oleh langkah verifikasi yang membutuhkannya.
function Invoke-Checked {
    # Uraian baris: Mendeklarasikan parameter masukan untuk skrip/fungsi ini; nilai default digunakan jika pemanggil tidak memberikan argumen. Atribut Mandatory, bila ada, mewajibkan pemanggil mengisi parameter tersebut.
    param(
        # Uraian baris: Menandai parameter Label sebagai wajib. Tipe string membawa label/teks/jalur, sedangkan scriptblock membawa perintah yang akan dieksekusi helper.
        [Parameter(Mandatory)][string]$Label,
        # Uraian baris: Menandai parameter Command sebagai wajib. Tipe string membawa label/teks/jalur, sedangkan scriptblock membawa perintah yang akan dieksekusi helper.
        [Parameter(Mandatory)][scriptblock]$Command
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    )

    # Uraian baris: Menampilkan "`n=== $Label ===" -ForegroundColor Cyan. Keluaran ini memberi hasil/progres verifikasi kepada operator tanpa mengubah berkas konfigurasi.
    Write-Host "`n=== $Label ===" -ForegroundColor Cyan
    # Uraian baris: Memanggil scriptblock yang diterima Invoke-Checked. Operator panggil & menjalankan perintah dalam blok sehingga exit code tool bisa diperiksa sesudahnya.
    & $Command
    # Uraian baris: Mengevaluasi kondisi ($LASTEXITCODE -ne 0). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
    if ($LASTEXITCODE -ne 0) {
        # Uraian baris: Menghentikan operasi dengan exception berisi pesan "$Label gagal dengan exit code $LASTEXITCODE.". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
        throw "$Label gagal dengan exit code $LASTEXITCODE."
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    }
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}

# Uraian baris: Mengulangi blok untuk setiap elemen pada ($tool in @("dotnet", "node", "npm", "docker")). Variabel iterasi menunjuk satu tool, entri konfigurasi, atau kontrak yang diperiksa pada putaran berjalan.
foreach ($tool in @("dotnet", "node", "npm", "docker")) {
    # Uraian baris: Mengevaluasi kondisi (-not (Get-Command $tool -ErrorAction SilentlyContinue)). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
    if (-not (Get-Command $tool -ErrorAction SilentlyContinue)) {
        # Uraian baris: Menghentikan operasi dengan exception berisi pesan "Tool wajib tidak tersedia: $tool". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
        throw "Tool wajib tidak tersedia: $tool"
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    }
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}

# Uraian baris: Mengevaluasi kondisi (-not (Test-Path -LiteralPath $resolvedEnvironmentFile -PathType Leaf)). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
if (-not (Test-Path -LiteralPath $resolvedEnvironmentFile -PathType Leaf)) {
    # Uraian baris: Menghentikan operasi dengan exception berisi pesan "Environment development tidak ditemukan: $resolvedEnvironmentFile". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
    throw "Environment development tidak ditemukan: $resolvedEnvironmentFile"
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}

# Uraian baris: Menyimpan direktori kerja saat ini lalu berpindah ke $repositoryRoot. Perintah relatif berikutnya berjalan dari direktori proyek yang sesuai.
Push-Location $repositoryRoot
# Uraian baris: Memulai blok operasi yang dipasangkan dengan catch/finally. Error dapat ditangani dan pembersihan tetap dijalankan setelah alur keluar.
try {
    # Uraian baris: Menjalankan tahap Restore .NET melalui helper pemeriksa exit code. Jika perintah tahap ini mengembalikan kode bukan nol, proses verifikasi dilempar sebagai error.
    Invoke-Checked "Restore .NET" { dotnet restore Cashflowpoly.sln }
    # Uraian baris: Menjalankan tahap Build Release tanpa warning melalui helper pemeriksa exit code. Jika perintah tahap ini mengembalikan kode bukan nol, proses verifikasi dilempar sebagai error.
    Invoke-Checked "Build Release tanpa warning" { dotnet build Cashflowpoly.sln -c Release --no-restore /warnaserror }
    # Uraian baris: Menjalankan tahap Unit, integration PostgreSQL, dan contract test melalui helper pemeriksa exit code. Jika perintah tahap ini mengembalikan kode bukan nol, proses verifikasi dilempar sebagai error.
    Invoke-Checked "Unit, integration PostgreSQL, dan contract test" { dotnet test Cashflowpoly.sln -c Release --no-build --no-restore --filter "Category!=Performance" }

    # Uraian baris: Menjalankan tahap Audit kerentanan NuGet melalui helper pemeriksa exit code. Jika perintah tahap ini mengembalikan kode bukan nol, proses verifikasi dilempar sebagai error.
    Invoke-Checked "Audit kerentanan NuGet" {
        # Uraian baris: Menjalankan CLI .NET dengan argumen list Cashflowpoly.sln package --vulnerable --include-transitive --format json | Out-File -LiteralPath (Join-Path $env:TEMP "cashflowpoly-nuget-audit.json") -Encoding utf8. Output JSON audit seluruh dependensi disimpan pada file sementara untuk dianalisis.
        dotnet list Cashflowpoly.sln package --vulnerable --include-transitive --format json | Out-File -LiteralPath (Join-Path $env:TEMP "cashflowpoly-nuget-audit.json") -Encoding utf8
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    }
    # Uraian baris: Membaca laporan audit NuGet JSON sementara dan mengubahnya menjadi objek PowerShell. Nilai dihitung dari ekspresi di sebelah kanan assignment.
    $nugetAudit = Get-Content -LiteralPath (Join-Path $env:TEMP "cashflowpoly-nuget-audit.json") -Raw | ConvertFrom-Json
    # Uraian baris: Menggabungkan paket langsung dan transitif dari laporan audit sebelum menyaring kerentanan. Nilai dihitung dari ekspresi di sebelah kanan assignment.
    $vulnerablePackages = @($nugetAudit.projects.frameworks.topLevelPackages + $nugetAudit.projects.frameworks.transitivePackages) |
        # Uraian baris: Menyaring hasil pipeline agar hanya paket dengan daftar vulnerabilities yang tidak kosong dipertahankan; jumlah hasil menentukan kegagalan audit NuGet.
        Where-Object { $_.vulnerabilities -and $_.vulnerabilities.Count -gt 0 }
    # Uraian baris: Mengevaluasi kondisi ($vulnerablePackages.Count -gt 0). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
    if ($vulnerablePackages.Count -gt 0) {
        # Uraian baris: Menghentikan operasi dengan exception berisi pesan "Audit NuGet menemukan $($vulnerablePackages.Count) package rentan.". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
        throw "Audit NuGet menemukan $($vulnerablePackages.Count) package rentan."
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    }

    # Uraian baris: Menjalankan tahap Install dependensi UI melalui helper pemeriksa exit code. Jika perintah tahap ini mengembalikan kode bukan nol, proses verifikasi dilempar sebagai error.
    Invoke-Checked "Install dependensi UI" {
        # Uraian baris: Menyimpan direktori kerja saat ini lalu berpindah ke (Join-Path $repositoryRoot "src/Cashflowpoly.Ui"). Perintah relatif berikutnya berjalan dari direktori proyek yang sesuai.
        Push-Location (Join-Path $repositoryRoot "src/Cashflowpoly.Ui")
        # Uraian baris: Memulai blok operasi yang dipasangkan dengan catch/finally. npm menjalankan tahap dependensi/pengujian pada direktori yang telah dipilih; finally mengembalikan direktori walau perintah gagal.
        try { npm ci --no-audit --no-fund } finally { Pop-Location }
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    }
    # Uraian baris: Menjalankan tahap Audit kerentanan npm UI melalui helper pemeriksa exit code. Jika perintah tahap ini mengembalikan kode bukan nol, proses verifikasi dilempar sebagai error.
    Invoke-Checked "Audit kerentanan npm UI" {
        # Uraian baris: Menyimpan direktori kerja saat ini lalu berpindah ke (Join-Path $repositoryRoot "src/Cashflowpoly.Ui"). Perintah relatif berikutnya berjalan dari direktori proyek yang sesuai.
        Push-Location (Join-Path $repositoryRoot "src/Cashflowpoly.Ui")
        # Uraian baris: Memulai blok operasi yang dipasangkan dengan catch/finally. npm menjalankan tahap dependensi/pengujian pada direktori yang telah dipilih; finally mengembalikan direktori walau perintah gagal.
        try { npm audit --audit-level=high } finally { Pop-Location }
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    }
    # Uraian baris: Menjalankan tahap Konsistensi dokumentasi dan Postman melalui helper pemeriksa exit code. Jika perintah tahap ini mengembalikan kode bukan nol, proses verifikasi dilempar sebagai error.
    Invoke-Checked "Konsistensi dokumentasi dan Postman" { & (Join-Path $PSScriptRoot "Test-DocumentationConsistency.ps1") }

    # Uraian baris: Mengevaluasi kondisi (-not $SkipPerformance). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
    if (-not $SkipPerformance) {
        # Uraian baris: Menjalankan tahap Target performa 100 akun, 20 sesi aktif, 20 pengguna bersamaan melalui helper pemeriksa exit code. Jika perintah tahap ini mengembalikan kode bukan nol, proses verifikasi dilempar sebagai error.
        Invoke-Checked "Target performa 100 akun, 20 sesi aktif, 20 pengguna bersamaan" {
            # Uraian baris: Menjalankan CLI .NET dengan argumen test tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj -c Release --no-build --no-restore --filter "Category=Performance". Filter memilih kelompok pengujian yang sesuai; no-build/no-restore memakai hasil tahap kompilasi sebelumnya.
            dotnet test tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj -c Release --no-build --no-restore --filter "Category=Performance"
        # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
        }
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    }

    # Uraian baris: Mengevaluasi kondisi (-not $SkipBrowser). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
    if (-not $SkipBrowser) {
        # Uraian baris: Menjalankan tahap Validasi Docker Compose development melalui helper pemeriksa exit code. Jika perintah tahap ini mengembalikan kode bukan nol, proses verifikasi dilempar sebagai error.
        Invoke-Checked "Validasi Docker Compose development" {
            # Uraian baris: Memanggil Docker Compose dengan environment dan file compose yang telah disusun. Baris lanjutan menentukan file konfigurasi serta validasi config -q.
            docker compose --env-file $resolvedEnvironmentFile @composeFiles config -q
        # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
        }
        # Uraian baris: Mengaktifkan flag seed simulasi pada environment proses agar Docker Compose meneruskannya ke tahap migrasi verifikasi.
        Invoke-Checked "Build image development untuk pengujian kode terbaru" {
            docker compose --env-file $resolvedEnvironmentFile @composeFiles build api ui
        }
        $env:DATABASE_MIGRATIONS_SEED_SIMULATION = "true"
        # Uraian baris: Menjalankan tahap Migrasi dan Seed 2 idempoten melalui helper pemeriksa exit code. Jika perintah tahap ini mengembalikan kode bukan nol, proses verifikasi dilempar sebagai error.
        Invoke-Checked "Migrasi dan Seed 2 idempoten" {
            # Uraian baris: Bagian 1 dari 2 baris instruksi berikut. Memanggil Docker Compose dengan environment dan file compose yang telah disusun. run --rm menjalankan operasi satu kali dan membersihkan containernya setelah selesai.
            # Uraian baris: Bagian 2 dari 2 baris instruksi berikut. Menjalankan CLI .NET dengan argumen run --project src/Cashflowpoly.Api/Cashflowpoly.Api.csproj --no-launch-profile -- --migrate-only. Perintah ini menjalankan entry point aplikasi sesuai mode migrasi atau rekalkulasi yang diberikan.
            docker compose --env-file $resolvedEnvironmentFile @composeFiles run --rm api `
                dotnet run --project src/Cashflowpoly.Api/Cashflowpoly.Api.csproj --no-launch-profile -- --migrate-only
        # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
        }
        # Uraian baris: Menghapus variabel environment sementara untuk seed simulasi dari proses PowerShell; ini mencegah flag verifikasi memengaruhi perintah selanjutnya.
        Remove-Item Env:DATABASE_MIGRATIONS_SEED_SIMULATION -ErrorAction SilentlyContinue
        # Uraian baris: Menjalankan tahap Rekalkulasi snapshot analitik Seed 2 melalui helper pemeriksa exit code. Jika perintah tahap ini mengembalikan kode bukan nol, proses verifikasi dilempar sebagai error.
        Invoke-Checked "Rekalkulasi snapshot analitik Seed 2" {
            # Uraian baris: Bagian 1 dari 2 baris instruksi berikut. Memanggil Docker Compose dengan environment dan file compose yang telah disusun. run --rm menjalankan operasi satu kali dan membersihkan containernya setelah selesai.
            # Uraian baris: Bagian 2 dari 2 baris instruksi berikut. Menjalankan CLI .NET dengan argumen run --project src/Cashflowpoly.Api/Cashflowpoly.Api.csproj --no-launch-profile -- --recalculate-analytics. Perintah ini menjalankan entry point aplikasi sesuai mode migrasi atau rekalkulasi yang diberikan.
            docker compose --env-file $resolvedEnvironmentFile @composeFiles run --rm --no-deps api `
                dotnet run --project src/Cashflowpoly.Api/Cashflowpoly.Api.csproj --no-launch-profile -- --recalculate-analytics
        # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
        }
        # Uraian baris: Menjalankan tahap Jalankan aplikasi lokal melalui helper pemeriksa exit code. Jika perintah tahap ini mengembalikan kode bukan nol, proses verifikasi dilempar sebagai error.
        Invoke-Checked "Jalankan aplikasi lokal" {
            # Uraian baris: Memanggil Docker Compose dengan environment dan file compose yang telah disusun. up -d menyalakan layanan di latar belakang sebelum health check.
            docker compose --env-file $resolvedEnvironmentFile @composeFiles up -d db api ui
        # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
        }

        # Uraian baris: Menentukan endpoint readiness UI lokal yang diperiksa setelah layanan dinyalakan. Nilai dihitung dari ekspresi di sebelah kanan assignment.
        $healthUri = "http://localhost:5203/health/ready"
        # Uraian baris: Mencatat apakah respons readiness HTTP sudah berhasil; flag ini diperiksa setelah loop selesai. Nilai dihitung dari ekspresi di sebelah kanan assignment.
        $healthy = $false
        # Uraian baris: Mengulangi percobaan health check dengan penghitung attempt, mulai 1 sampai 30. Batas ini menghentikan penantian ketika UI tidak mencapai keadaan siap.
        for ($attempt = 1; $attempt -le 30; $attempt += 1) {
            # Uraian baris: Memulai blok operasi yang dipasangkan dengan catch/finally. Error dapat ditangani dan pembersihan tetap dijalankan setelah alur keluar.
            try {
                # Uraian baris: Mengirim request HTTP readiness dengan timeout 3 detik dan menyimpan respons untuk memeriksa status 200. Nilai dihitung dari ekspresi di sebelah kanan assignment.
                $response = Invoke-WebRequest -Uri $healthUri -UseBasicParsing -TimeoutSec 3
                # Uraian baris: Mengevaluasi kondisi ($response.StatusCode -eq 200). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
                if ($response.StatusCode -eq 200) {
                    # Uraian baris: Mencatat apakah respons readiness HTTP sudah berhasil; flag ini diperiksa setelah loop selesai. Nilai dihitung dari ekspresi di sebelah kanan assignment.
                    $healthy = $true
                    # Uraian baris: Keluar dari loop percobaan setelah kondisi siap terpenuhi sehingga tidak mengirim pemeriksaan HTTP tambahan.
                    break
                # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
                }
            # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
            }
            # Uraian baris: Menangani exception dari percobaan pemeriksaan sebelumnya; pada loop readiness, kegagalan sementara akan diberi jeda sebelum dicoba lagi.
            catch {
                # Uraian baris: Memberi jeda 2 detik setelah request readiness gagal sebelum mencoba lagi, sehingga startup UI tidak dibanjiri request berulang tanpa jeda.
                Start-Sleep -Seconds 2
            # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
            }
        # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
        }
        # Uraian baris: Mengevaluasi kondisi (-not $healthy). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
        if (-not $healthy) {
            # Uraian baris: Menghentikan operasi dengan exception berisi pesan "UI lokal tidak sehat setelah 60 detik: $healthUri". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
            throw "UI lokal tidak sehat setelah 60 detik: $healthUri"
        # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
        }

        # Uraian baris: Menjalankan tahap Install Playwright melalui helper pemeriksa exit code. Jika perintah tahap ini mengembalikan kode bukan nol, proses verifikasi dilempar sebagai error.
        Invoke-Checked "Install Playwright" {
            # Uraian baris: Menyimpan direktori kerja saat ini lalu berpindah ke $e2eRoot. Perintah relatif berikutnya berjalan dari direktori proyek yang sesuai.
            Push-Location $e2eRoot
            # Uraian baris: Memulai blok operasi yang dipasangkan dengan catch/finally. npm menjalankan tahap dependensi/pengujian pada direktori yang telah dipilih; finally mengembalikan direktori walau perintah gagal.
            try { npm ci --no-audit --no-fund } finally { Pop-Location }
        # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
        }
        # Uraian baris: Menjalankan tahap Audit kerentanan npm E2E melalui helper pemeriksa exit code. Jika perintah tahap ini mengembalikan kode bukan nol, proses verifikasi dilempar sebagai error.
        Invoke-Checked "Audit kerentanan npm E2E" {
            # Uraian baris: Menyimpan direktori kerja saat ini lalu berpindah ke $e2eRoot. Perintah relatif berikutnya berjalan dari direktori proyek yang sesuai.
            Push-Location $e2eRoot
            # Uraian baris: Memulai blok operasi yang dipasangkan dengan catch/finally. npm menjalankan tahap dependensi/pengujian pada direktori yang telah dipilih; finally mengembalikan direktori walau perintah gagal.
            try { npm audit --audit-level=high } finally { Pop-Location }
        # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
        }
        # Uraian baris: Menjalankan tahap Install Chromium Playwright melalui helper pemeriksa exit code. Jika perintah tahap ini mengembalikan kode bukan nol, proses verifikasi dilempar sebagai error.
        Invoke-Checked "Install Chromium Playwright" {
            # Uraian baris: Menyimpan direktori kerja saat ini lalu berpindah ke $e2eRoot. Perintah relatif berikutnya berjalan dari direktori proyek yang sesuai.
            Push-Location $e2eRoot
            # Uraian baris: Memulai blok operasi yang dipasangkan dengan catch/finally. Error dapat ditangani dan pembersihan tetap dijalankan setelah alur keluar.
            try { npx playwright install chromium } finally { Pop-Location }
        # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
        }
        # Uraian baris: Menjalankan tahap E2E Chromium desktop dan ponsel melalui helper pemeriksa exit code. Jika perintah tahap ini mengembalikan kode bukan nol, proses verifikasi dilempar sebagai error.
        Invoke-Checked "E2E Chromium desktop dan ponsel" {
            # Uraian baris: Menyimpan direktori kerja saat ini lalu berpindah ke $e2eRoot. Perintah relatif berikutnya berjalan dari direktori proyek yang sesuai.
            Push-Location $e2eRoot
            # Uraian baris: Memulai blok operasi yang dipasangkan dengan catch/finally. npm menjalankan tahap dependensi/pengujian pada direktori yang telah dipilih; finally mengembalikan direktori walau perintah gagal.
            try { npm test } finally { Pop-Location }
        # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
        }
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    }

    # Uraian baris: Mengevaluasi kondisi (-not $SkipDockerBuild). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
    if (-not $SkipDockerBuild) {
        # Uraian baris: Menjalankan tahap Build image API melalui helper pemeriksa exit code. Jika perintah tahap ini mengembalikan kode bukan nol, proses verifikasi dilempar sebagai error.
        Invoke-Checked "Build image API" { docker build -f src/Cashflowpoly.Api/Dockerfile -t cashflowpoly-api:release-gate . }
        # Uraian baris: Menjalankan tahap Build image UI melalui helper pemeriksa exit code. Jika perintah tahap ini mengembalikan kode bukan nol, proses verifikasi dilempar sebagai error.
        Invoke-Checked "Build image UI" { docker build -f src/Cashflowpoly.Ui/Dockerfile -t cashflowpoly-ui:release-gate . }
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    }

    # Uraian baris: Menampilkan "`nSeluruh gerbang verifikasi rilis lulus." -ForegroundColor Green. Keluaran ini memberi hasil/progres verifikasi kepada operator tanpa mengubah berkas konfigurasi.
    Write-Host "`nSeluruh gerbang verifikasi rilis lulus." -ForegroundColor Green
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}
# Uraian baris: Memulai pembersihan yang selalu dijalankan setelah try, termasuk ketika validasi, tool, atau request HTTP gagal.
finally {
    # Uraian baris: Menghapus variabel environment sementara untuk seed simulasi dari proses PowerShell; ini mencegah flag verifikasi memengaruhi perintah selanjutnya.
    Remove-Item Env:DATABASE_MIGRATIONS_SEED_SIMULATION -ErrorAction SilentlyContinue
    # Uraian baris: Memulihkan direktori kerja sebelumnya dari stack PowerShell agar eksekusi skrip tidak meninggalkan terminal pada direktori lain.
    Pop-Location
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}
