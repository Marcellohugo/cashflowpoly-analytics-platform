# Fungsi file: Memverifikasi cakupan dokumentasi fungsi file tracked melalui komentar inline atau manifest pusat.
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Get-Mode {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    $ext = [System.IO.Path]::GetExtension($Path).ToLowerInvariant()
    $inlineExt = @(
        ".cs", ".cshtml", ".js", ".css", ".ps1", ".sh", ".sql", ".http",
        ".conf", ".yml", ".yaml", ".csproj", ".example", ".gitignore"
    )

    if ($inlineExt -contains $ext) {
        return "inline"
    }

    return "manifest-only"
}

function Test-InlineHeader {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Content
    )

    $ext = [System.IO.Path]::GetExtension($Path).ToLowerInvariant()
    switch ($ext) {
        ".cs" { return [regex]::IsMatch($Content, "(?m)^\s*//\s*Fungsi file:") }
        ".cshtml" { return [regex]::IsMatch($Content, "(?m)^\s*@\*\s*Fungsi file:") }
        ".js" { return [regex]::IsMatch($Content, "(?m)^\s*/\*\s*Fungsi file:") }
        ".css" { return [regex]::IsMatch($Content, "(?m)^\s*/\*\s*Fungsi file:") }
        ".sql" { return [regex]::IsMatch($Content, "(?m)^\s*--\s*Fungsi file:") }
        ".csproj" { return [regex]::IsMatch($Content, "(?m)^\s*<!--\s*Fungsi file:") }
        default { return [regex]::IsMatch($Content, "(?m)^\s*#\s*Fungsi file:") }
    }
}

function Test-HasSummaryBefore {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Lines,
        [Parameter(Mandatory = $true)]
        [int]$Index
    )

    if ($null -eq $Lines -or ($Lines -isnot [System.Collections.IList])) {
        return $false
    }

    $limit = [Math]::Max(0, $Index - 12)
    for ($k = $Index - 1; $k -ge $limit; $k--) {
        $line = $Lines[$k].Trim()
        if ($line -match "^///\s*<summary>") { return $true }
        if ($line -eq "") { continue }
        if ($line -match "^\[") { continue }
        if ($line -match "^///") { continue }
        if ($line -match "^//") { continue }
        break
    }

    return $false
}

function Test-CSharpSummaryCoverage {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Content
    )

    $lines = [System.Collections.Generic.List[string]]::new()
    foreach ($line in ($Content -split "`r?`n", -1)) {
        [void]$lines.Add($line)
    }

    $issues = [System.Collections.Generic.List[string]]::new()
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $trim = $lines[$i].Trim()
        if ($trim -eq "") { continue }

        $isType = $trim -match "^(public|internal|protected|private)\s+(?:(?:sealed|static|partial|abstract|readonly|ref|unsafe|new)\s+)*(class|record|interface|enum)\s+([A-Za-z_][A-Za-z0-9_]*)"
        $isMethodCandidate = $trim -match "^(public|internal|protected|private)\b" -and
            $trim -match "\(" -and
            $trim -notmatch "\b(class|record|interface|enum|struct)\b" -and
            $trim -notmatch "^\s*(if|for|foreach|while|switch|catch|lock|using)\b" -and
            $trim -notmatch "\bdelegate\b"

        if (-not ($isType -or $isMethodCandidate)) {
            continue
        }

        if (-not (Test-HasSummaryBefore -Lines $lines -Index $i)) {
            $lineNo = $i + 1
            $issues.Add("${Path}:$lineNo")
        }
    }

    return $issues
}

function Get-ManifestPaths {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ManifestPath
    )

    if (-not (Test-Path $ManifestPath -PathType Leaf)) {
        throw "Manifest tidak ditemukan: $ManifestPath"
    }

    $lines = [System.IO.File]::ReadAllLines($ManifestPath)
    $paths = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($line in $lines) {
        if (-not $line.StartsWith("|")) { continue }
        if ($line -match "^\|\s*Path\s*\|") { continue }
        if ($line -match "^\|\s*---") { continue }

        $cells = $line.Split("|")
        if ($cells.Length -lt 3) { continue }
        $pathCell = $cells[1].Trim()
        if ([string]::IsNullOrWhiteSpace($pathCell)) { continue }
        $normalizedPath = $pathCell.Trim([char]96).Replace("\\|", "|")
        [void]$paths.Add($normalizedPath)
    }

    return $paths
}

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
Push-Location $repoRoot
try {
    $tracked = git ls-files
    if (-not $tracked) {
        throw "Tidak ada file tracked dari git ls-files."
    }

    $manifestPath = Join-Path $repoRoot "docs/file-function-manifest.md"
    $manifestPaths = Get-ManifestPaths -ManifestPath $manifestPath

    $errors = [System.Collections.Generic.List[string]]::new()
    $covered = 0

    foreach ($path in $tracked) {
        if (-not (Test-Path $path -PathType Leaf)) {
            $errors.Add("File tracked tidak ditemukan di workspace: $path")
            continue
        }

        $mode = Get-Mode -Path $path
        $content = [System.IO.File]::ReadAllText($path)
        $hasManifestEntry = $manifestPaths.Contains($path)

        if ($mode -eq "inline") {
            $hasInline = Test-InlineHeader -Path $path -Content $content
            if (-not $hasInline -and -not $hasManifestEntry) {
                $errors.Add("Tidak tercakup inline/manifest: $path")
                continue
            }

            if ($hasInline) {
                $covered += 1
                if ([System.IO.Path]::GetExtension($path).ToLowerInvariant() -eq ".cs") {
                    $summaryIssues = Test-CSharpSummaryCoverage -Path $path -Content $content
                    foreach ($summaryIssue in $summaryIssues) {
                        $errors.Add("Summary method/class belum lengkap: $summaryIssue")
                    }
                }
            } else {
                $covered += 1
            }
        } else {
            if (-not $hasManifestEntry) {
                $errors.Add("File manifest-only belum masuk manifest: $path")
                continue
            }
            $covered += 1
        }
    }

    if ($errors.Count -gt 0) {
        Write-Host "Verifikasi gagal. Ditemukan $($errors.Count) masalah:" -ForegroundColor Red
        $errors | Sort-Object -Unique | ForEach-Object { Write-Host " - $_" -ForegroundColor Red }
        exit 1
    }

    Write-Host "Verifikasi sukses. Total file tercakup: $covered" -ForegroundColor Green
    exit 0
}
finally {
    Pop-Location
}
