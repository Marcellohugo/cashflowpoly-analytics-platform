# Fungsi file: Memastikan dokumentasi, Postman, dan kontrak publik tetap selaras dengan implementasi saat ini.
# Uraian baris: Menjadikan skrip sebagai advanced command PowerShell sehingga pengikatan parameter dan parameter umum mengikuti perilaku cmdlet.
[CmdletBinding()]
# Uraian baris: Mendeklarasikan parameter masukan untuk skrip/fungsi ini; nilai default digunakan jika pemanggil tidak memberikan argumen. Atribut Mandatory, bila ada, mewajibkan pemanggil mengisi parameter tersebut.
param()

# Uraian baris: Meminta error PowerShell menghentikan alur agar pemeriksaan yang gagal tidak diteruskan sebagai hasil sukses.
$ErrorActionPreference = "Stop"
# Uraian baris: Mengambil direktori induk scripts sebagai root repositori; semua jalur proyek disusun dari lokasi skrip, bukan direktori terminal pemanggil.
$repositoryRoot = Split-Path -Parent $PSScriptRoot

# Uraian baris: Mendefinisikan fungsi Read-RepositoryFile. Isi blok disimpan untuk dipanggil kembali oleh langkah verifikasi yang membutuhkannya.
function Read-RepositoryFile {
    # Uraian baris: Mendeklarasikan parameter masukan untuk skrip/fungsi ini; nilai default digunakan jika pemanggil tidak memberikan argumen. Atribut Mandatory, bila ada, mewajibkan pemanggil mengisi parameter tersebut.
    param([Parameter(Mandatory)][string]$Path)

    # Uraian baris: Menyimpan hasil ekspresi Join-Path $repositoryRoot $Path ke variabel absolutePath agar dapat digunakan pada tahap verifikasi selanjutnya.
    $absolutePath = Join-Path $repositoryRoot $Path
    # Uraian baris: Mengevaluasi kondisi (-not (Test-Path -LiteralPath $absolutePath -PathType Leaf)). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
    if (-not (Test-Path -LiteralPath $absolutePath -PathType Leaf)) {
        # Uraian baris: Menghentikan operasi dengan exception berisi pesan "Dokumen wajib tidak ditemukan: $Path". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
        throw "Dokumen wajib tidak ditemukan: $Path"
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    }

    # Uraian baris: Mengakhiri fungsi dan mengembalikan Get-Content -LiteralPath $absolutePath -Raw. Get-Content -Raw membaca seluruh berkas sebagai satu string agar pencarian kontrak tidak terpecah per baris.
    return Get-Content -LiteralPath $absolutePath -Raw
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}

# Uraian baris: Mendefinisikan fungsi Assert-ContainsText. Isi blok disimpan untuk dipanggil kembali oleh langkah verifikasi yang membutuhkannya.
function Assert-ContainsText {
    # Uraian baris: Mendeklarasikan parameter masukan untuk skrip/fungsi ini; nilai default digunakan jika pemanggil tidak memberikan argumen. Atribut Mandatory, bila ada, mewajibkan pemanggil mengisi parameter tersebut.
    param(
        # Uraian baris: Menandai parameter Content sebagai wajib. Tipe string membawa label/teks/jalur, sedangkan scriptblock membawa perintah yang akan dieksekusi helper.
        [Parameter(Mandatory)][string]$Content,
        # Uraian baris: Menandai parameter Expected sebagai wajib. Tipe string membawa label/teks/jalur, sedangkan scriptblock membawa perintah yang akan dieksekusi helper.
        [Parameter(Mandatory)][string]$Expected,
        # Uraian baris: Menandai parameter Source sebagai wajib. Tipe string membawa label/teks/jalur, sedangkan scriptblock membawa perintah yang akan dieksekusi helper.
        [Parameter(Mandatory)][string]$Source
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    )

    # Uraian baris: Mengevaluasi kondisi ($Content.IndexOf($Expected, [StringComparison]::Ordinal) -lt 0). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
    if ($Content.IndexOf($Expected, [StringComparison]::Ordinal) -lt 0) {
        # Uraian baris: Menghentikan operasi dengan exception berisi pesan "$Source belum memuat kontrak wajib: $Expected". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
        throw "$Source belum memuat kontrak wajib: $Expected"
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    }
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}

# Uraian baris: Mendefinisikan fungsi Assert-ExcludesText. Isi blok disimpan untuk dipanggil kembali oleh langkah verifikasi yang membutuhkannya.
function Assert-ExcludesText {
    # Uraian baris: Mendeklarasikan parameter masukan untuk skrip/fungsi ini; nilai default digunakan jika pemanggil tidak memberikan argumen. Atribut Mandatory, bila ada, mewajibkan pemanggil mengisi parameter tersebut.
    param(
        # Uraian baris: Menandai parameter Content sebagai wajib. Tipe string membawa label/teks/jalur, sedangkan scriptblock membawa perintah yang akan dieksekusi helper.
        [Parameter(Mandatory)][string]$Content,
        # Uraian baris: Menandai parameter Forbidden sebagai wajib. Tipe string membawa label/teks/jalur, sedangkan scriptblock membawa perintah yang akan dieksekusi helper.
        [Parameter(Mandatory)][string]$Forbidden,
        # Uraian baris: Menandai parameter Source sebagai wajib. Tipe string membawa label/teks/jalur, sedangkan scriptblock membawa perintah yang akan dieksekusi helper.
        [Parameter(Mandatory)][string]$Source
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    )

    # Uraian baris: Mengevaluasi kondisi ($Content.IndexOf($Forbidden, [StringComparison]::OrdinalIgnoreCase) -ge 0). Blok berikutnya hanya dijalankan saat kondisi bernilai benar; cabang ini menentukan apakah validasi diterima, ditolak, atau dilanjutkan.
    if ($Content.IndexOf($Forbidden, [StringComparison]::OrdinalIgnoreCase) -ge 0) {
        # Uraian baris: Menghentikan operasi dengan exception berisi pesan "$Source masih memuat kontrak lama yang dilarang: $Forbidden". Pesan ini menjelaskan prasyarat atau pemeriksaan yang gagal kepada pemanggil.
        throw "$Source masih memuat kontrak lama yang dilarang: $Forbidden"
    # Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
    }
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}

# Uraian baris: Menyimpan hasil ekspresi Read-RepositoryFile "README.md" ke variabel readme agar dapat digunakan pada tahap verifikasi selanjutnya.
$readme = Read-RepositoryFile "README.md"
# Uraian baris: Menyimpan hasil ekspresi Read-RepositoryFile "docs/02-Perancangan/02-02-kontrak-rest-api-dan-event-permainan.md" ke variabel apiReadme agar dapat digunakan pada tahap verifikasi selanjutnya.
$apiReadme = Read-RepositoryFile "docs/02-Perancangan/02-02-kontrak-rest-api-dan-event-permainan.md"
# Uraian baris: Menyimpan hasil ekspresi Read-RepositoryFile "docs/02-Perancangan/02-01-arsitektur-database-dan-model-data.md" ke variabel databaseReadme agar dapat digunakan pada tahap verifikasi selanjutnya.
$databaseReadme = Read-RepositoryFile "docs/02-Perancangan/02-01-arsitektur-database-dan-model-data.md"
# Uraian baris: Menyimpan hasil ekspresi Read-RepositoryFile "docs/02-Perancangan/02-03-rancangan-definisi-dan-agregasi-metrik.md" ke variabel metricDesign agar dapat digunakan pada tahap verifikasi selanjutnya.
$metricDesign = Read-RepositoryFile "docs/02-Perancangan/02-03-rancangan-definisi-dan-agregasi-metrik.md"
# Uraian baris: Menyimpan hasil ekspresi Join-Path $repositoryRoot "postman/Cashflowpoly.postman_collection.json" ke variabel postmanPath agar dapat digunakan pada tahap verifikasi selanjutnya.
$postmanPath = Join-Path $repositoryRoot "postman/Cashflowpoly.postman_collection.json"
# Uraian baris: Menyimpan hasil ekspresi Join-Path $repositoryRoot "postman/Cashflowpoly.local.postman_environment.json" ke variabel environmentPath agar dapat digunakan pada tahap verifikasi selanjutnya.
$environmentPath = Join-Path $repositoryRoot "postman/Cashflowpoly.local.postman_environment.json"
# Uraian baris: Menyimpan hasil ekspresi Get-Content -LiteralPath $postmanPath -Raw ke variabel postmanText agar dapat digunakan pada tahap verifikasi selanjutnya.
$postmanText = Get-Content -LiteralPath $postmanPath -Raw

# JSON harus dapat diparsing agar collection tidak rusak hanya karena kesalahan koma atau kurung.
# Uraian baris: Mem-parsing JSON Postman dan membuang objek hasilnya melalui $null. Tujuannya memverifikasi sintaks; JSON yang rusak tetap menimbulkan error.
$null = $postmanText | ConvertFrom-Json
# Uraian baris: Mem-parsing JSON Postman dan membuang objek hasilnya melalui $null. Tujuannya memverifikasi sintaks; JSON yang rusak tetap menimbulkan error.
$null = (Get-Content -LiteralPath $environmentPath -Raw) | ConvertFrom-Json

# Uraian baris: Mengulangi blok untuk setiap elemen pada ($contract in @(. Variabel iterasi menunjuk satu tool, entri konfigurasi, atau kontrak yang diperiksa pada putaran berjalan.
foreach ($contract in @(
    # Uraian baris: Menambahkan nilai "/api/v1/sessions/{sessionId}/setup/validate" ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "/api/v1/sessions/{sessionId}/setup/validate",
    # Uraian baris: Menambahkan nilai "/api/v1/sessions/{sessionId}/setup" ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "/api/v1/sessions/{sessionId}/setup",
    # Uraian baris: Menambahkan nilai "next_cursor" ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "next_cursor",
    # Uraian baris: Menambahkan nilai "has_more" ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "has_more",
    # Uraian baris: Menambahkan nilai "transaction_id" ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "transaction_id"
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
)) {
    # Uraian baris: Memeriksa teks kontrak wajib melalui helper Assert-ContainsText dengan masukan -Content $apiReadme -Expected $contract -Source "dokumen kontrak REST API". Ketidakcocokan menghentikan pemeriksaan konsistensi dokumentasi.
    Assert-ContainsText -Content $apiReadme -Expected $contract -Source "dokumen kontrak REST API"
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}

# Uraian baris: Mengulangi blok untuk setiap elemen pada ($metric in @(. Variabel iterasi menunjuk satu tool, entri konfigurasi, atau kontrak yang diperiksa pada putaran berjalan.
foreach ($metric in @(
    # Uraian baris: Menambahkan nilai "Pertumbuhan Kas" ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "Perubahan Koin dari Awal",
    # Uraian baris: Menambahkan nilai "Diversifikasi Pendapatan" ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "Diversifikasi Pendapatan",
    # Uraian baris: Menambahkan nilai "Porsi Biaya Usaha" ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "Porsi Biaya Usaha",
    # Uraian baris: Menambahkan nilai "Margin Usaha Pesanan" ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "Margin Usaha Pesanan",
    # Uraian baris: Menambahkan nilai "Kesiapan Menghadapi Risiko" ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "Kesiapan Menghadapi Risiko",
    # Uraian baris: Menambahkan nilai "Beban Pinjaman" ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "Beban Pinjaman",
    # Memastikan metrik pembelian memakai seluruh katalog, bukan setoran yang dianggap memesan kartu.
    "Porsi Biaya Pembelian Tujuan",
    "financial_goals_completed ÷ financial_goals_available_total × 100%",
    "Saldo tabungan tidak dihitung sebagai pembelian",
    # Uraian baris: Menambahkan nilai "Fokus Aksi Penghasil Uang" ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "Fokus Aksi Penghasil Uang",
    # Uraian baris: Menambahkan nilai "Pemanfaatan Bahan" ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "Pemanfaatan Bahan",
    # Uraian baris: Menambahkan nilai "Porsi Aksi Jangka Panjang" ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "Porsi Aksi Jangka Panjang",
    # Uraian baris: Menambahkan nilai "Keragaman Pemenuhan Kebutuhan" ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "Keragaman Pemenuhan Kebutuhan",
    # Uraian baris: Menambahkan nilai "Komitmen Donasi" ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "Komitmen Donasi",
    # Uraian baris: Menambahkan nilai "Komposisi Poin Kebahagiaan" ke daftar yang sedang dibentuk. Urutan nilai mengikuti urutan pemeriksaan kontrak/metrik atau argumen command line di bawahnya.
    "Komposisi Poin Kebahagiaan",
    "Pemerataan Sumber Poin Kebahagiaan",
    "Target Finansial Berhasil Dibeli",
    "saving_and_goal_actions"
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
)) {
    # Uraian baris: Memeriksa teks kontrak wajib melalui helper Assert-ContainsText dengan masukan -Content $metricDesign -Expected $metric -Source "dokumen definisi metrik". Ketidakcocokan menghentikan pemeriksaan konsistensi dokumentasi.
    Assert-ContainsText -Content $metricDesign -Expected $metric -Source "dokumen definisi metrik"
# Uraian baris: Menutup blok, daftar parameter, atau koleksi yang sedang dibentuk; batas sintaks ini mengembalikan eksekusi/struktur ke tingkat induknya atau membuka badan perulangan setelah daftar selesai.
}

# Menjaga kontrak tabungan tanpa reservasi dan pembelian atomik tetap terdokumentasi.
foreach ($savingContract in @(
    'satu saldo tabungan per pemain',
    '`goal_id` opsional untuk kompatibilitas klien lama',
    'pemain yang lebih dulu berhasil membeli',
    'transaksi dengan kunci sesi'
)) {
    Assert-ContainsText -Content $apiReadme -Expected $savingContract -Source "dokumen kontrak tabungan dan pembelian tujuan"
}
Assert-ExcludesText -Content $apiReadme -Forbidden "setoran pada tujuan lain tetap teralokasi" -Source "dokumen kontrak tabungan"

# Uraian baris: Memeriksa teks kontrak wajib melalui helper Assert-ContainsText dengan masukan -Content $readme -Expected "IDN" -Source "README.md". Ketidakcocokan menghentikan pemeriksaan konsistensi dokumentasi.
Assert-ContainsText -Content $readme -Expected "IDN" -Source "README.md"
# Uraian baris: Memeriksa teks kontrak wajib melalui helper Assert-ContainsText dengan masukan -Content $databaseReadme -Expected "schema_history" -Source "dokumen arsitektur database". Ketidakcocokan menghentikan pemeriksaan konsistensi dokumentasi.
Assert-ContainsText -Content $databaseReadme -Expected "schema_history" -Source "dokumen arsitektur database"
# Uraian baris: Memeriksa teks kontrak wajib melalui helper Assert-ContainsText dengan masukan -Content $databaseReadme -Expected "session_setup_revisions" -Source "dokumen arsitektur database". Ketidakcocokan menghentikan pemeriksaan konsistensi dokumentasi.
Assert-ContainsText -Content $databaseReadme -Expected "session_setup_revisions" -Source "dokumen arsitektur database"
# Uraian baris: Memeriksa teks kontrak wajib melalui helper Assert-ContainsText dengan masukan -Content $postmanText -Expected "/events?limit=50" -Source "Postman collection". Ketidakcocokan menghentikan pemeriksaan konsistensi dokumentasi.
Assert-ContainsText -Content $postmanText -Expected "/events?limit=50" -Source "Postman collection"
# Uraian baris: Memeriksa teks kontrak wajib melalui helper Assert-ContainsText dengan masukan -Content $postmanText -Expected "/transactions?userId={{playerUserId}}&limit=50" -Source "Postman collection". Ketidakcocokan menghentikan pemeriksaan konsistensi dokumentasi.
Assert-ContainsText -Content $postmanText -Expected "/transactions?userId={{playerUserId}}&limit=50" -Source "Postman collection"
# Uraian baris: Memastikan kontrak lama yang disebut pada argumen Forbidden tidak lagi muncul pada sumber; helper memakai perbandingan tanpa membedakan kapital.
Assert-ExcludesText -Content $postmanText -Forbidden "LewatiOrder" -Source "Postman collection"
# Uraian baris: Memastikan kontrak lama yang disebut pada argumen Forbidden tidak lagi muncul pada sumber; helper memakai perbandingan tanpa membedakan kapital.
Assert-ExcludesText -Content $postmanText -Forbidden "fromSeq" -Source "Postman collection"

# Periksa seluruh dokumen aktif dan tautan berkas lokal, bukan hanya kata kunci kontrak API.
$documentationFiles = Get-ChildItem -LiteralPath (Join-Path $repositoryRoot "docs") -Filter *.md -Recurse -File
foreach ($document in $documentationFiles) {
    $content = Get-Content -LiteralPath $document.FullName -Raw
    foreach ($obsolete in @(
        "PlayerDirectoryController", "Chart.js", "UI menyimpan token di server-side session.",
        "UI menyimpan token di session.", "registrasi publik Instruktur ditolak",
        "pendaftaran Instructor publik tetap ditolak", "Ibu Rina Kartika"
    )) {
        Assert-ExcludesText -Content $content -Forbidden $obsolete -Source $document.FullName
    }
    foreach ($match in [regex]::Matches($content, '\]\(([^)\s]+)\)')) {
        $target = $match.Groups[1].Value.Trim('<', '>')
        if ($target -match '^(?:[a-z][a-z0-9+.-]*:|#)') { continue }
        $target = [Uri]::UnescapeDataString(($target -split '#', 2)[0])
        if (-not $target) { continue }
        $resolved = Join-Path $document.DirectoryName $target
        if (-not (Test-Path -LiteralPath $resolved)) {
            throw "Tautan dokumentasi tidak ditemukan: $($document.FullName) -> $target"
        }
    }
}
$docsIndex = Read-RepositoryFile "docs/README.md"
Assert-ContainsText -Content $docsIndex -Expected "03-05-kesiapan-produksi.md" -Source "Indeks docs"
Assert-ContainsText -Content $docsIndex -Expected "03-06-perbaikan-audit-ui-data-dan-dokumentasi.md" -Source "Indeks docs"
$userManual = Read-RepositoryFile "docs/00-Panduan/00-02-panduan-manual-pengguna-dashboard.md"
foreach ($required in @("/statistics", "5 baris", "hover", "120 karakter", "72 byte", "Data Protection")) {
    Assert-ContainsText -Content $userManual -Expected $required -Source "Manual pengguna"
}
foreach ($required in @("refreshSequences", "refreshed_items", "has_sealed_donations", "akun instruktur pemanggil")) {
    Assert-ContainsText -Content $apiReadme -Expected $required -Source "Kontrak API"
}
Write-Output "Pemeriksaan kontrak dokumentasi dan tautan lokal lulus pada $($documentationFiles.Count) dokumen Markdown."
