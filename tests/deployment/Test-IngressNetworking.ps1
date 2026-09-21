# Fungsi file: Menguji alokasi IP produksi saat Nginx menyala sebelum tunnel pada jaringan baru.
param([string]$Image = 'alpine:3.22', [string]$TestSubnet = '172.29.253.0/29')
$ErrorActionPreference = 'Stop'
$repo = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$productionJson = & docker compose --env-file (Join-Path $repo 'config/env/.env.prod.example') `
    -f (Join-Path $repo 'infra/docker/docker-compose.yml') `
    -f (Join-Path $repo 'infra/docker/docker-compose.prod.yml') --profile tunnel config --format json
if ($LASTEXITCODE -ne 0) { throw 'Konfigurasi Compose produksi tidak dapat dibaca.' }
$production = $productionJson | ConvertFrom-Json
$productionPrefix = $production.networks.'cashflowpoly-ingress'.ipam.config[0].subnet -replace '\.\d+/\d+$', '.'
$testPrefix = $TestSubnet -replace '\.\d+/\d+$', '.'
$services = @{}
foreach ($service in @('nginx', 'cloudflared')) {
    $address = $production.services.$service.networks.'cashflowpoly-ingress'.ipv4_address
    $endpoint = @{}
    if ($address) { $endpoint.ipv4_address = $address.Replace($productionPrefix, $testPrefix) }
    $services[$service] = @{ image = $Image; command = @('sleep', '60'); networks = @{ ingress = $endpoint } }
}
$project = 'cashflowpoly-ingress-test-' + [Guid]::NewGuid().ToString('N').Substring(0, 8)
$probeFile = Join-Path ([IO.Path]::GetTempPath()) "$project.json"
@{ services = $services; networks = @{ ingress = @{ internal = $true; ipam = @{ config = @(@{ subnet = $TestSubnet }) } } } } |
    ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $probeFile
$compose = @('--project-name', $project, '-f', $probeFile)
try {
    # Urutan ini sama dengan deploy-production.sh; tidak menjalankan tunnel atau aplikasi nyata.
    & docker compose @compose up -d nginx
    if ($LASTEXITCODE -ne 0) { throw 'Startup Nginx probe gagal.' }
    & docker compose @compose up -d --no-deps cloudflared
    if ($LASTEXITCODE -ne 0) { throw 'Startup tunnel probe gagal; periksa benturan alamat ingress.' }
    $addresses = foreach ($service in @('nginx', 'cloudflared')) {
        $id = & docker compose @compose ps -q $service
        if ($LASTEXITCODE -ne 0 -or -not $id) { throw "Container probe $service tidak berjalan." }
        & docker inspect --format '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' $id
        if ($LASTEXITCODE -ne 0) { throw "Alamat probe $service tidak dapat dibaca." }
    }
    if (-not $addresses[0] -or -not $addresses[1] -or $addresses[0] -eq $addresses[1]) {
        throw 'Nginx dan tunnel wajib memiliki alamat ingress berbeda.'
    }
    Write-Output "PASS: Nginx $($addresses[0]) menyala sebelum tunnel $($addresses[1]) pada jaringan baru."
} finally {
    & docker compose @compose down --remove-orphans
    Remove-Item -LiteralPath $probeFile -ErrorAction SilentlyContinue
}
