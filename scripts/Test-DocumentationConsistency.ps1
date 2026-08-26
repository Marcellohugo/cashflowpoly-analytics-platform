# Fungsi file: Memastikan dokumentasi, Postman, dan kontrak publik tetap selaras dengan implementasi saat ini.
[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot

function Read-RepositoryFile {
    param([Parameter(Mandatory)][string]$Path)

    $absolutePath = Join-Path $repositoryRoot $Path
    if (-not (Test-Path -LiteralPath $absolutePath -PathType Leaf)) {
        throw "Dokumen wajib tidak ditemukan: $Path"
    }

    return Get-Content -LiteralPath $absolutePath -Raw
}

function Assert-ContainsText {
    param(
        [Parameter(Mandatory)][string]$Content,
        [Parameter(Mandatory)][string]$Expected,
        [Parameter(Mandatory)][string]$Source
    )

    if ($Content.IndexOf($Expected, [StringComparison]::Ordinal) -lt 0) {
        throw "$Source belum memuat kontrak wajib: $Expected"
    }
}

function Assert-ExcludesText {
    param(
        [Parameter(Mandatory)][string]$Content,
        [Parameter(Mandatory)][string]$Forbidden,
        [Parameter(Mandatory)][string]$Source
    )

    if ($Content.IndexOf($Forbidden, [StringComparison]::OrdinalIgnoreCase) -ge 0) {
        throw "$Source masih memuat kontrak lama yang dilarang: $Forbidden"
    }
}

$readme = Read-RepositoryFile "README.md"
$apiReadme = Read-RepositoryFile "README-API.md"
$databaseReadme = Read-RepositoryFile "README-DATABASE.md"
$metricDesign = Read-RepositoryFile "docs/02-Perancangan/02-03-rancangan-definisi-dan-agregasi-metrik.md"
$postmanPath = Join-Path $repositoryRoot "postman/Cashflowpoly.postman_collection.json"
$environmentPath = Join-Path $repositoryRoot "postman/Cashflowpoly.local.postman_environment.json"
$postmanText = Get-Content -LiteralPath $postmanPath -Raw

# JSON harus dapat diparsing agar collection tidak rusak hanya karena kesalahan koma atau kurung.
$null = $postmanText | ConvertFrom-Json
$null = (Get-Content -LiteralPath $environmentPath -Raw) | ConvertFrom-Json

foreach ($contract in @(
    "/api/v1/sessions/{sessionId}/setup/validate",
    "/api/v1/sessions/{sessionId}/setup",
    "next_cursor",
    "has_more",
    "transaction_id"
)) {
    Assert-ContainsText -Content $apiReadme -Expected $contract -Source "README-API.md"
}

foreach ($metric in @(
    "Pertumbuhan Kas",
    "Diversifikasi Pendapatan",
    "Porsi Biaya Usaha",
    "Margin Usaha Pesanan",
    "Kesiapan Menghadapi Risiko",
    "Beban Pinjaman",
    "Progres Target Finansial",
    "Fokus Aksi Penghasil Uang",
    "Pemanfaatan Bahan",
    "Porsi Aksi Jangka Panjang",
    "Keragaman Pemenuhan Kebutuhan",
    "Komitmen Donasi",
    "Komposisi Poin Kebahagiaan"
)) {
    Assert-ContainsText -Content $metricDesign -Expected $metric -Source "dokumen definisi metrik"
}

Assert-ContainsText -Content $readme -Expected "IDN" -Source "README.md"
Assert-ContainsText -Content $databaseReadme -Expected "schema_history" -Source "README-DATABASE.md"
Assert-ContainsText -Content $databaseReadme -Expected "session_setup_revisions" -Source "README-DATABASE.md"
Assert-ContainsText -Content $postmanText -Expected "/events?limit=50" -Source "Postman collection"
Assert-ContainsText -Content $postmanText -Expected "/transactions?userId={{playerUserId}}&limit=50" -Source "Postman collection"
Assert-ExcludesText -Content $postmanText -Forbidden "LewatiOrder" -Source "Postman collection"
Assert-ExcludesText -Content $postmanText -Forbidden "fromSeq" -Source "Postman collection"

Write-Output "Dokumentasi konsisten: README, API, database, definisi metrik, dan Postman valid."
