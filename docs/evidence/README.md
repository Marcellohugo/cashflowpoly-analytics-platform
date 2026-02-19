# Evidence Folder

Folder ini menyimpan artefak verifikasi formal (build/test/compose/load/security/observability) per tanggal eksekusi.

Struktur yang direkomendasikan:

```text
docs/evidence/
  YYYY-MM-DD/
    build.log
    test.log
    compose.log
    load-test-summary.md
    observability.json
    security-audit.json
```

Gunakan script baseline beban:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/perf/run-load-test.ps1 -BaseUrl http://localhost:5041
```
