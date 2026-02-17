# Evidence Directory

Folder ini dipakai untuk kurasi artefak bukti teknis per tanggal eksekusi.

Struktur yang direkomendasikan:

```
docs/evidence/
  YYYY-MM-DD/
    README.md
    build-output.txt
    test-output.txt
    compose-status.txt
    load-test-summary.md
    ...
```

Setiap paket tanggal minimal memuat:
1. bukti build/test,
2. bukti status runtime (compose/health),
3. bukti verifikasi fungsional/non-fungsional yang relevan.
