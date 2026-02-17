# Evidence Pack - 17 Februari 2026

Folder ini berisi artefak bukti formal untuk verifikasi baseline implementasi.

## Daftar Artefak

1. `build-output.txt`  
   Hasil `dotnet build Cashflowpoly.sln --nologo`.

2. `test-output.txt`  
   Hasil `dotnet test Cashflowpoly.sln --nologo`.

3. `compose-status.txt`  
   Status service docker compose (`db`, `api`, `ui`) saat pengambilan evidence.

4. `load-test-summary.md`  
   Ringkasan hasil uji beban dari `scripts/perf/run-load-test.ps1`.

5. `sql-security-audit-count.txt`  
   Bukti jumlah data pada tabel `security_audit_logs`.

6. `sql-security-audit-sample.txt`  
   Sampel 10 baris terbaru audit log keamanan.

7. `api-observability-metrics.json`  
   Output endpoint `GET /api/v1/observability/metrics`.

8. `api-security-audit-logs.json`  
   Output endpoint `GET /api/v1/security/audit-logs`.
