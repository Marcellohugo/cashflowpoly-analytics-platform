-- Fungsi file: Membatasi isi dan retensi log operasional tanpa menghapus jejak migrasi lama.
alter table validation_logs
  add column if not exists status_code integer not null default 422,
  add column if not exists trace_id varchar(64) not null default 'legacy';

alter table validation_logs
  alter column raw_payload_json set default '{}'::jsonb,
  alter column details_json set default '{}'::jsonb;

update validation_logs
set raw_payload_json = '{}'::jsonb,
    details_json = '{}'::jsonb
where raw_payload_json <> '{}'::jsonb
   or coalesce(details_json, '{}'::jsonb) <> '{}'::jsonb;

insert into log_retention_policies (table_name, retention_days)
values
  ('validation_logs', 30),
  ('security_audit_logs', 30)
on conflict (table_name) do update
set retention_days = excluded.retention_days,
    updated_at = now();
