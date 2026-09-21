# Fungsi file: Menguji identitas proxy tepercaya, penolakan spoofing, dan isolasi rate limit Nginx.
param([string]$Image = 'nginxinc/nginx-unprivileged:1.31.3-alpine')
$ErrorActionPreference = 'Stop'
$repo = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$composeJson = & docker compose --env-file (Join-Path $repo 'config/env/.env.prod.example') `
    -f (Join-Path $repo 'infra/docker/docker-compose.yml') `
    -f (Join-Path $repo 'infra/docker/docker-compose.prod.yml') --profile tunnel config --format json
if ($LASTEXITCODE -ne 0) { throw 'Konfigurasi Compose produksi tidak dapat dibaca.' }
$uiEnvironment = ($composeJson | ConvertFrom-Json).services.ui.environment
if ($uiEnvironment.Security__RequireHttps -ne 'true' -or $uiEnvironment.ASPNETCORE_HTTPS_PORT -ne '443') {
    throw 'Redirect HTTPS UI produksi wajib aktif dengan port publik 443.'
}
$output = Join-Path $repo 'artifacts/fixes-round12/nginx'
New-Item -ItemType Directory -Force $output | Out-Null
$config = [IO.File]::ReadAllText((Join-Path $repo 'infra/nginx/default.conf'))
# Substitute only the trusted peer and upstream sockets for an isolated, networkless container.
$config = $config.Replace('172.30.254.2', '127.0.0.1').Replace('http://api:5041', 'http://127.0.0.1:5041').Replace('http://ui:5203', 'http://127.0.0.1:5203')
[IO.File]::WriteAllText((Join-Path $output 'production.conf'), $config)
$harness = @'
worker_processes 1;
pid /tmp/nginx-identity.pid;
error_log /tmp/nginx-identity-error.log warn;
events { worker_connections 128; }
http {
    access_log off;
    include /proof/production.conf;
    server {
        listen 5041;
        listen 5203;
        location / {
            default_type text/plain;
            return 200 "xff=$http_x_forwarded_for;real=$http_x_real_ip;proto=$http_x_forwarded_proto";
        }
    }
    server {
        listen 8081;
        location / {
            proxy_bind 127.0.0.2;
            proxy_pass http://127.0.0.1:8080;
        }
    }
}
'@
[IO.File]::WriteAllText((Join-Path $output 'nginx.conf'), $harness)
$probe = @'
#!/bin/sh
set -eu
nginx -c /proof/nginx.conf -t
nginx -c /proof/nginx.conf
trap 'nginx -c /proof/nginx.conf -s quit >/dev/null 2>&1 || true' EXIT
trusted=$(wget -qO- --header 'CF-Connecting-IP: 203.0.113.9' --header 'X-Forwarded-For: 192.0.2.66' --header 'X-Forwarded-Proto: https' http://127.0.0.1:8080/api/v1/identity)
test "$trusted" = 'xff=203.0.113.9;real=203.0.113.9;proto=https'
untrusted=$(wget -qO- --header 'CF-Connecting-IP: 203.0.113.9' --header 'X-Forwarded-For: 192.0.2.66' --header 'X-Forwarded-Proto: https' http://127.0.0.1:8081/api/v1/identity)
test "$untrusted" = 'xff=127.0.0.2;real=127.0.0.2;proto=http'
trusted_asset=$(wget -qO- --header 'CF-Connecting-IP: 203.0.113.9' --header 'X-Forwarded-For: 192.0.2.66' --header 'X-Forwarded-Proto: https' http://127.0.0.1:8080/css/site.css)
test "$trusted_asset" = 'xff=203.0.113.9;real=203.0.113.9;proto=https'
untrusted_asset=$(wget -qO- --header 'CF-Connecting-IP: 203.0.113.9' --header 'X-Forwarded-For: 192.0.2.66' --header 'X-Forwarded-Proto: https' http://127.0.0.1:8081/css/site.css)
test "$untrusted_asset" = 'xff=127.0.0.2;real=127.0.0.2;proto=http'
status() {
    wget -S -O /dev/null --header "CF-Connecting-IP: $1" http://127.0.0.1:8080/api/v1/auth/login 2>&1 | awk '/HTTP\// { print $2; exit }'
}
for suffix in 1 2 3 4 5 6; do
    test "$(status 203.0.113.$suffix)" = 200
done
# One initial request plus the configured ten-request burst must pass immediately.
for attempt in 1 2 3 4 5 6 7 8 9 10 11; do
    test "$(status 198.51.100.10)" = 200
done
test "$(status 198.51.100.10)" = 429
test "$(status 198.51.100.10)" = 429
echo 'PASS: trusted visitor IP and scheme for API/static assets, spoof rejection, six independent clients, per-client HTTP 429.'
'@
[IO.File]::WriteAllText((Join-Path $output 'probe.sh'), $probe.Replace("`r`n", "`n"))
& docker run --rm --network none --entrypoint sh --mount "type=bind,source=$output,target=/proof,readonly" $Image /proof/probe.sh
if ($LASTEXITCODE -ne 0) { throw "Nginx client identity regression failed ($LASTEXITCODE)." }
