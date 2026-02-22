#!/usr/bin/env sh
set -eu

cd /src/src/Cashflowpoly.Ui
npm ci --no-audit --no-fund

npm run tailwind:watch &
tailwind_pid=$!

cleanup() {
  if kill -0 "$tailwind_pid" 2>/dev/null; then
    kill "$tailwind_pid" 2>/dev/null || true
    wait "$tailwind_pid" 2>/dev/null || true
  fi
}

trap cleanup INT TERM EXIT

cd /src
dotnet watch --project src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj run --no-launch-profile
