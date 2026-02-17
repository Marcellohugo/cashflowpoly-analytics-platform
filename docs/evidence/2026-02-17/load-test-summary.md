# Load Test Summary

- Generated at (UTC): 2026-02-17 03:34:48
- Base URL: http://localhost:5041/
- Session ID: 03a93e9d-4e91-4f63-9e34-77b5bf37c3cc
- Player ID: 2d60295a-b277-45b6-a1b8-46605f40758c
- Ruleset Version ID: 946ad713-80f6-4525-b0b1-20663cbda314

## Scenario Parameters

| Metric | Value |
|---|---|
| Seed events | 120 |
| Analytics iterations | 40 |

## Results

| Endpoint Group | Requests | Success | Failure | Error Rate (%) | Avg (ms) | P50 (ms) | P95 (ms) | Max (ms) | SLA |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---|
| POST /api/v1/events | 120 | 120 | 0 | 0 | 16.57 | 15.7 | 18.72 | 84.01 | PASS |
| GET /api/v1/analytics/sessions/{sessionId} | 40 | 40 | 0 | 0 | 216.54 | 101.19 | 867.26 | 906.14 | PASS |

## Notes

- Ingest SLA target: P95 <= 500 ms, error rate = 0%
- Analytics SLA target: P95 <= 1500 ms, error rate = 0%
- Script ini menyiapkan data uji secara otomatis (register -> ruleset -> session -> player -> ingest -> analytics).
