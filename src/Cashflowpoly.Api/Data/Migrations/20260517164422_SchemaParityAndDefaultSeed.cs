using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cashflowpoly.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class SchemaParityAndDefaultSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.AlterColumn<string>(
                name: "error_message",
                table: "validation_logs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "error_code",
                table: "validation_logs",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "validation_logs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "user_player_links",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "sessions",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "session_name",
                table: "sessions",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "mode",
                table: "sessions",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "activated_by",
                table: "session_ruleset_activations",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "activated_at",
                table: "session_ruleset_activations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "session_players",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "PLAYER",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "join_order",
                table: "session_players",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "session_players",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "security_audit_logs",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "user_agent",
                table: "security_audit_logs",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "trace_id",
                table: "security_audit_logs",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "security_audit_logs",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "path",
                table: "security_audit_logs",
                type: "character varying(240)",
                maxLength: 240,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "outcome",
                table: "security_audit_logs",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "occurred_at",
                table: "security_audit_logs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "method",
                table: "security_audit_logs",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ip_address",
                table: "security_audit_logs",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "event_type",
                table: "security_audit_logs",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "rulesets",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "rulesets",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "rulesets",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "ruleset_versions",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "ruleset_versions",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "ruleset_versions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "config_hash",
                table: "ruleset_versions",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "display_name",
                table: "players",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "players",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "metric_name",
                table: "metric_snapshots",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "computed_at",
                table: "metric_snapshots",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "weekday",
                table: "events",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "received_at",
                table: "events",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "client_request_id",
                table: "events",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "actor_type",
                table: "events",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "action_type",
                table: "events",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "reference",
                table: "event_cashflow_projections",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "note",
                table: "event_cashflow_projections",
                type: "character varying(160)",
                maxLength: 160,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "direction",
                table: "event_cashflow_projections",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "counterparty",
                table: "event_cashflow_projections",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "category",
                table: "event_cashflow_projections",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "app_users",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "app_users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "app_users",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "app_users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_validation_logs_event_pk",
                table: "validation_logs",
                column: "event_pk");

            migrationBuilder.CreateIndex(
                name: "ix_validation_session_time",
                table: "validation_logs",
                columns: new[] { "session_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_validation_valid",
                table: "validation_logs",
                column: "is_valid");

            migrationBuilder.CreateIndex(
                name: "validation_logs_session_id_event_id_key",
                table: "validation_logs",
                columns: new[] { "session_id", "event_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_player_links_player",
                table: "user_player_links",
                column: "player_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_player_links_user",
                table: "user_player_links",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sessions_created_at",
                table: "sessions",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_sessions_instructor_user",
                table: "sessions",
                columns: new[] { "instructor_user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_sessions_status",
                table: "sessions",
                column: "status");

            migrationBuilder.AddCheckConstraint(
                name: "sessions_mode_check",
                table: "sessions",
                sql: "mode in ('PEMULA','MAHIR')");

            migrationBuilder.AddCheckConstraint(
                name: "sessions_status_check",
                table: "sessions",
                sql: "status in ('CREATED','STARTED','ENDED')");

            migrationBuilder.CreateIndex(
                name: "ix_sra_ruleset_version",
                table: "session_ruleset_activations",
                column: "ruleset_version_id");

            migrationBuilder.CreateIndex(
                name: "ix_sra_session",
                table: "session_ruleset_activations",
                columns: new[] { "session_id", "activated_at" });

            migrationBuilder.CreateIndex(
                name: "ix_session_players_player",
                table: "session_players",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_session_players_session",
                table: "session_players",
                columns: new[] { "session_id", "join_order" });

            migrationBuilder.CreateIndex(
                name: "session_players_session_id_player_id_key",
                table: "session_players",
                columns: new[] { "session_id", "player_id" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_session_players_role",
                table: "session_players",
                sql: "role in ('PLAYER')");

            migrationBuilder.AddCheckConstraint(
                name: "session_players_join_order_check",
                table: "session_players",
                sql: "join_order >= 1");

            migrationBuilder.CreateIndex(
                name: "ix_security_audit_logs_event",
                table: "security_audit_logs",
                columns: new[] { "event_type", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_security_audit_logs_occurred",
                table: "security_audit_logs",
                column: "occurred_at");

            migrationBuilder.CreateIndex(
                name: "ix_security_audit_logs_user",
                table: "security_audit_logs",
                columns: new[] { "user_id", "occurred_at" });

            migrationBuilder.AddCheckConstraint(
                name: "security_audit_logs_outcome_check",
                table: "security_audit_logs",
                sql: "outcome in ('SUCCESS','FAILURE','DENIED')");

            migrationBuilder.AddCheckConstraint(
                name: "security_audit_logs_status_code_check",
                table: "security_audit_logs",
                sql: "status_code >= 100 and status_code <= 599");

            migrationBuilder.CreateIndex(
                name: "ix_rulesets_created_at",
                table: "rulesets",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_rulesets_instructor_user",
                table: "rulesets",
                columns: new[] { "instructor_user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_ruleset_versions_config_gin",
                table: "ruleset_versions",
                column: "config_json")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "ix_ruleset_versions_ruleset",
                table: "ruleset_versions",
                columns: new[] { "ruleset_id", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ruleset_versions_status",
                table: "ruleset_versions",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ruleset_versions_ruleset_id_config_hash_key",
                table: "ruleset_versions",
                columns: new[] { "ruleset_id", "config_hash" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "ruleset_versions_status_check",
                table: "ruleset_versions",
                sql: "status in ('DRAFT','ACTIVE','RETIRED')");

            migrationBuilder.AddCheckConstraint(
                name: "ruleset_versions_version_check",
                table: "ruleset_versions",
                sql: "version >= 1");

            migrationBuilder.CreateIndex(
                name: "ix_players_instructor_user",
                table: "players",
                columns: new[] { "instructor_user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_metric_snapshots_player_id",
                table: "metric_snapshots",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_metrics_ruleset_version",
                table: "metric_snapshots",
                column: "ruleset_version_id");

            migrationBuilder.CreateIndex(
                name: "ix_metrics_session_name_time",
                table: "metric_snapshots",
                columns: new[] { "session_id", "metric_name", "computed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_metrics_session_player_name_time",
                table: "metric_snapshots",
                columns: new[] { "session_id", "player_id", "metric_name", "computed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_metrics_session_player_time",
                table: "metric_snapshots",
                columns: new[] { "session_id", "player_id", "computed_at" });

            migrationBuilder.CreateIndex(
                name: "events_session_id_event_id_key",
                table: "events",
                columns: new[] { "session_id", "event_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_events_payload_gin",
                table: "events",
                column: "payload")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "ix_events_player_time",
                table: "events",
                columns: new[] { "player_id", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_events_ruleset_version_id",
                table: "events",
                column: "ruleset_version_id");

            migrationBuilder.CreateIndex(
                name: "ix_events_session_action",
                table: "events",
                columns: new[] { "session_id", "action_type" });

            migrationBuilder.CreateIndex(
                name: "ix_events_session_seq",
                table: "events",
                columns: new[] { "session_id", "sequence_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_events_session_time",
                table: "events",
                columns: new[] { "session_id", "timestamp" });

            migrationBuilder.AddCheckConstraint(
                name: "events_actor_type_check",
                table: "events",
                sql: "actor_type in ('PLAYER','SYSTEM')");

            migrationBuilder.AddCheckConstraint(
                name: "events_day_index_check",
                table: "events",
                sql: "day_index >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "events_sequence_number_check",
                table: "events",
                sql: "sequence_number >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "events_turn_number_check",
                table: "events",
                sql: "turn_number >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "events_weekday_check",
                table: "events",
                sql: "weekday in ('MON','TUE','WED','THU','FRI','SAT','SUN')");

            migrationBuilder.CreateIndex(
                name: "event_cashflow_projections_session_id_event_id_key",
                table: "event_cashflow_projections",
                columns: new[] { "session_id", "event_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ecp_category",
                table: "event_cashflow_projections",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "ix_ecp_session_player_time",
                table: "event_cashflow_projections",
                columns: new[] { "session_id", "player_id", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "ix_ecp_session_time",
                table: "event_cashflow_projections",
                columns: new[] { "session_id", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_event_cashflow_projections_event_pk",
                table: "event_cashflow_projections",
                column: "event_pk");

            migrationBuilder.CreateIndex(
                name: "IX_event_cashflow_projections_player_id",
                table: "event_cashflow_projections",
                column: "player_id");

            migrationBuilder.AddCheckConstraint(
                name: "event_cashflow_projections_amount_check",
                table: "event_cashflow_projections",
                sql: "amount > 0");

            migrationBuilder.AddCheckConstraint(
                name: "event_cashflow_projections_direction_check",
                table: "event_cashflow_projections",
                sql: "direction in ('IN','OUT')");

            migrationBuilder.CreateIndex(
                name: "app_users_username_key",
                table: "app_users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_app_users_role_active",
                table: "app_users",
                columns: new[] { "role", "is_active" });

            migrationBuilder.AddCheckConstraint(
                name: "app_users_role_check",
                table: "app_users",
                sql: "role in ('INSTRUCTOR','PLAYER')");

            migrationBuilder.AddForeignKey(
                name: "event_cashflow_projections_event_pk_fkey",
                table: "event_cashflow_projections",
                column: "event_pk",
                principalTable: "events",
                principalColumn: "event_pk",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "event_cashflow_projections_player_id_fkey",
                table: "event_cashflow_projections",
                column: "player_id",
                principalTable: "players",
                principalColumn: "player_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "event_cashflow_projections_session_id_fkey",
                table: "event_cashflow_projections",
                column: "session_id",
                principalTable: "sessions",
                principalColumn: "session_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "events_player_id_fkey",
                table: "events",
                column: "player_id",
                principalTable: "players",
                principalColumn: "player_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "events_ruleset_version_id_fkey",
                table: "events",
                column: "ruleset_version_id",
                principalTable: "ruleset_versions",
                principalColumn: "ruleset_version_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "events_session_id_fkey",
                table: "events",
                column: "session_id",
                principalTable: "sessions",
                principalColumn: "session_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "metric_snapshots_player_id_fkey",
                table: "metric_snapshots",
                column: "player_id",
                principalTable: "players",
                principalColumn: "player_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "metric_snapshots_ruleset_version_id_fkey",
                table: "metric_snapshots",
                column: "ruleset_version_id",
                principalTable: "ruleset_versions",
                principalColumn: "ruleset_version_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "metric_snapshots_session_id_fkey",
                table: "metric_snapshots",
                column: "session_id",
                principalTable: "sessions",
                principalColumn: "session_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "players_instructor_user_id_fkey",
                table: "players",
                column: "instructor_user_id",
                principalTable: "app_users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "ruleset_versions_ruleset_id_fkey",
                table: "ruleset_versions",
                column: "ruleset_id",
                principalTable: "rulesets",
                principalColumn: "ruleset_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "rulesets_instructor_user_id_fkey",
                table: "rulesets",
                column: "instructor_user_id",
                principalTable: "app_users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "security_audit_logs_user_id_fkey",
                table: "security_audit_logs",
                column: "user_id",
                principalTable: "app_users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "session_players_player_id_fkey",
                table: "session_players",
                column: "player_id",
                principalTable: "players",
                principalColumn: "player_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "session_players_session_id_fkey",
                table: "session_players",
                column: "session_id",
                principalTable: "sessions",
                principalColumn: "session_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "session_ruleset_activations_ruleset_version_id_fkey",
                table: "session_ruleset_activations",
                column: "ruleset_version_id",
                principalTable: "ruleset_versions",
                principalColumn: "ruleset_version_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "session_ruleset_activations_session_id_fkey",
                table: "session_ruleset_activations",
                column: "session_id",
                principalTable: "sessions",
                principalColumn: "session_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "sessions_instructor_user_id_fkey",
                table: "sessions",
                column: "instructor_user_id",
                principalTable: "app_users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "user_player_links_player_id_fkey",
                table: "user_player_links",
                column: "player_id",
                principalTable: "players",
                principalColumn: "player_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "user_player_links_user_id_fkey",
                table: "user_player_links",
                column: "user_id",
                principalTable: "app_users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "validation_logs_event_pk_fkey",
                table: "validation_logs",
                column: "event_pk",
                principalTable: "events",
                principalColumn: "event_pk",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "validation_logs_session_id_fkey",
                table: "validation_logs",
                column: "session_id",
                principalTable: "sessions",
                principalColumn: "session_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "event_cashflow_projections_event_pk_fkey",
                table: "event_cashflow_projections");

            migrationBuilder.DropForeignKey(
                name: "event_cashflow_projections_player_id_fkey",
                table: "event_cashflow_projections");

            migrationBuilder.DropForeignKey(
                name: "event_cashflow_projections_session_id_fkey",
                table: "event_cashflow_projections");

            migrationBuilder.DropForeignKey(
                name: "events_player_id_fkey",
                table: "events");

            migrationBuilder.DropForeignKey(
                name: "events_ruleset_version_id_fkey",
                table: "events");

            migrationBuilder.DropForeignKey(
                name: "events_session_id_fkey",
                table: "events");

            migrationBuilder.DropForeignKey(
                name: "metric_snapshots_player_id_fkey",
                table: "metric_snapshots");

            migrationBuilder.DropForeignKey(
                name: "metric_snapshots_ruleset_version_id_fkey",
                table: "metric_snapshots");

            migrationBuilder.DropForeignKey(
                name: "metric_snapshots_session_id_fkey",
                table: "metric_snapshots");

            migrationBuilder.DropForeignKey(
                name: "players_instructor_user_id_fkey",
                table: "players");

            migrationBuilder.DropForeignKey(
                name: "ruleset_versions_ruleset_id_fkey",
                table: "ruleset_versions");

            migrationBuilder.DropForeignKey(
                name: "rulesets_instructor_user_id_fkey",
                table: "rulesets");

            migrationBuilder.DropForeignKey(
                name: "security_audit_logs_user_id_fkey",
                table: "security_audit_logs");

            migrationBuilder.DropForeignKey(
                name: "session_players_player_id_fkey",
                table: "session_players");

            migrationBuilder.DropForeignKey(
                name: "session_players_session_id_fkey",
                table: "session_players");

            migrationBuilder.DropForeignKey(
                name: "session_ruleset_activations_ruleset_version_id_fkey",
                table: "session_ruleset_activations");

            migrationBuilder.DropForeignKey(
                name: "session_ruleset_activations_session_id_fkey",
                table: "session_ruleset_activations");

            migrationBuilder.DropForeignKey(
                name: "sessions_instructor_user_id_fkey",
                table: "sessions");

            migrationBuilder.DropForeignKey(
                name: "user_player_links_player_id_fkey",
                table: "user_player_links");

            migrationBuilder.DropForeignKey(
                name: "user_player_links_user_id_fkey",
                table: "user_player_links");

            migrationBuilder.DropForeignKey(
                name: "validation_logs_event_pk_fkey",
                table: "validation_logs");

            migrationBuilder.DropForeignKey(
                name: "validation_logs_session_id_fkey",
                table: "validation_logs");

            migrationBuilder.DropIndex(
                name: "IX_validation_logs_event_pk",
                table: "validation_logs");

            migrationBuilder.DropIndex(
                name: "ix_validation_session_time",
                table: "validation_logs");

            migrationBuilder.DropIndex(
                name: "ix_validation_valid",
                table: "validation_logs");

            migrationBuilder.DropIndex(
                name: "validation_logs_session_id_event_id_key",
                table: "validation_logs");

            migrationBuilder.DropIndex(
                name: "ix_user_player_links_player",
                table: "user_player_links");

            migrationBuilder.DropIndex(
                name: "ix_user_player_links_user",
                table: "user_player_links");

            migrationBuilder.DropIndex(
                name: "ix_sessions_created_at",
                table: "sessions");

            migrationBuilder.DropIndex(
                name: "ix_sessions_instructor_user",
                table: "sessions");

            migrationBuilder.DropIndex(
                name: "ix_sessions_status",
                table: "sessions");

            migrationBuilder.DropCheckConstraint(
                name: "sessions_mode_check",
                table: "sessions");

            migrationBuilder.DropCheckConstraint(
                name: "sessions_status_check",
                table: "sessions");

            migrationBuilder.DropIndex(
                name: "ix_sra_ruleset_version",
                table: "session_ruleset_activations");

            migrationBuilder.DropIndex(
                name: "ix_sra_session",
                table: "session_ruleset_activations");

            migrationBuilder.DropIndex(
                name: "ix_session_players_player",
                table: "session_players");

            migrationBuilder.DropIndex(
                name: "ix_session_players_session",
                table: "session_players");

            migrationBuilder.DropIndex(
                name: "session_players_session_id_player_id_key",
                table: "session_players");

            migrationBuilder.DropCheckConstraint(
                name: "ck_session_players_role",
                table: "session_players");

            migrationBuilder.DropCheckConstraint(
                name: "session_players_join_order_check",
                table: "session_players");

            migrationBuilder.DropIndex(
                name: "ix_security_audit_logs_event",
                table: "security_audit_logs");

            migrationBuilder.DropIndex(
                name: "ix_security_audit_logs_occurred",
                table: "security_audit_logs");

            migrationBuilder.DropIndex(
                name: "ix_security_audit_logs_user",
                table: "security_audit_logs");

            migrationBuilder.DropCheckConstraint(
                name: "security_audit_logs_outcome_check",
                table: "security_audit_logs");

            migrationBuilder.DropCheckConstraint(
                name: "security_audit_logs_status_code_check",
                table: "security_audit_logs");

            migrationBuilder.DropIndex(
                name: "ix_rulesets_created_at",
                table: "rulesets");

            migrationBuilder.DropIndex(
                name: "ix_rulesets_instructor_user",
                table: "rulesets");

            migrationBuilder.DropIndex(
                name: "ix_ruleset_versions_config_gin",
                table: "ruleset_versions");

            migrationBuilder.DropIndex(
                name: "ix_ruleset_versions_ruleset",
                table: "ruleset_versions");

            migrationBuilder.DropIndex(
                name: "ix_ruleset_versions_status",
                table: "ruleset_versions");

            migrationBuilder.DropIndex(
                name: "ruleset_versions_ruleset_id_config_hash_key",
                table: "ruleset_versions");

            migrationBuilder.DropCheckConstraint(
                name: "ruleset_versions_status_check",
                table: "ruleset_versions");

            migrationBuilder.DropCheckConstraint(
                name: "ruleset_versions_version_check",
                table: "ruleset_versions");

            migrationBuilder.DropIndex(
                name: "ix_players_instructor_user",
                table: "players");

            migrationBuilder.DropIndex(
                name: "IX_metric_snapshots_player_id",
                table: "metric_snapshots");

            migrationBuilder.DropIndex(
                name: "ix_metrics_ruleset_version",
                table: "metric_snapshots");

            migrationBuilder.DropIndex(
                name: "ix_metrics_session_name_time",
                table: "metric_snapshots");

            migrationBuilder.DropIndex(
                name: "ix_metrics_session_player_name_time",
                table: "metric_snapshots");

            migrationBuilder.DropIndex(
                name: "ix_metrics_session_player_time",
                table: "metric_snapshots");

            migrationBuilder.DropIndex(
                name: "events_session_id_event_id_key",
                table: "events");

            migrationBuilder.DropIndex(
                name: "ix_events_payload_gin",
                table: "events");

            migrationBuilder.DropIndex(
                name: "ix_events_player_time",
                table: "events");

            migrationBuilder.DropIndex(
                name: "IX_events_ruleset_version_id",
                table: "events");

            migrationBuilder.DropIndex(
                name: "ix_events_session_action",
                table: "events");

            migrationBuilder.DropIndex(
                name: "ix_events_session_seq",
                table: "events");

            migrationBuilder.DropIndex(
                name: "ix_events_session_time",
                table: "events");

            migrationBuilder.DropCheckConstraint(
                name: "events_actor_type_check",
                table: "events");

            migrationBuilder.DropCheckConstraint(
                name: "events_day_index_check",
                table: "events");

            migrationBuilder.DropCheckConstraint(
                name: "events_sequence_number_check",
                table: "events");

            migrationBuilder.DropCheckConstraint(
                name: "events_turn_number_check",
                table: "events");

            migrationBuilder.DropCheckConstraint(
                name: "events_weekday_check",
                table: "events");

            migrationBuilder.DropIndex(
                name: "event_cashflow_projections_session_id_event_id_key",
                table: "event_cashflow_projections");

            migrationBuilder.DropIndex(
                name: "ix_ecp_category",
                table: "event_cashflow_projections");

            migrationBuilder.DropIndex(
                name: "ix_ecp_session_player_time",
                table: "event_cashflow_projections");

            migrationBuilder.DropIndex(
                name: "ix_ecp_session_time",
                table: "event_cashflow_projections");

            migrationBuilder.DropIndex(
                name: "IX_event_cashflow_projections_event_pk",
                table: "event_cashflow_projections");

            migrationBuilder.DropIndex(
                name: "IX_event_cashflow_projections_player_id",
                table: "event_cashflow_projections");

            migrationBuilder.DropCheckConstraint(
                name: "event_cashflow_projections_amount_check",
                table: "event_cashflow_projections");

            migrationBuilder.DropCheckConstraint(
                name: "event_cashflow_projections_direction_check",
                table: "event_cashflow_projections");

            migrationBuilder.DropIndex(
                name: "app_users_username_key",
                table: "app_users");

            migrationBuilder.DropIndex(
                name: "ix_app_users_role_active",
                table: "app_users");

            migrationBuilder.DropCheckConstraint(
                name: "app_users_role_check",
                table: "app_users");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.AlterColumn<string>(
                name: "error_message",
                table: "validation_logs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "error_code",
                table: "validation_logs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "validation_logs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "user_player_links",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "sessions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "session_name",
                table: "sessions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "mode",
                table: "sessions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "sessions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "activated_by",
                table: "session_ruleset_activations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "activated_at",
                table: "session_ruleset_activations",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "session_players",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "PLAYER");

            migrationBuilder.AlterColumn<int>(
                name: "join_order",
                table: "session_players",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "session_players",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "security_audit_logs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "user_agent",
                table: "security_audit_logs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "trace_id",
                table: "security_audit_logs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "security_audit_logs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "path",
                table: "security_audit_logs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(240)",
                oldMaxLength: 240);

            migrationBuilder.AlterColumn<string>(
                name: "outcome",
                table: "security_audit_logs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "occurred_at",
                table: "security_audit_logs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "method",
                table: "security_audit_logs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<string>(
                name: "ip_address",
                table: "security_audit_logs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "event_type",
                table: "security_audit_logs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "rulesets",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "rulesets",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "rulesets",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "ruleset_versions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "ruleset_versions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "ruleset_versions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "config_hash",
                table: "ruleset_versions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "display_name",
                table: "players",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "players",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "metric_name",
                table: "metric_snapshots",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "computed_at",
                table: "metric_snapshots",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "weekday",
                table: "events",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "received_at",
                table: "events",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "client_request_id",
                table: "events",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "actor_type",
                table: "events",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "action_type",
                table: "events",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "reference",
                table: "event_cashflow_projections",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "note",
                table: "event_cashflow_projections",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(160)",
                oldMaxLength: 160,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "direction",
                table: "event_cashflow_projections",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "counterparty",
                table: "event_cashflow_projections",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "category",
                table: "event_cashflow_projections",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "app_users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "app_users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "app_users",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "app_users",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");
        }
    }
}
