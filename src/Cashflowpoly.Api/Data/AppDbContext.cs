// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk AppDbContext.
// Mengimpor namespace `Microsoft.EntityFrameworkCore` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.EntityFrameworkCore;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Data` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Data;

/// <summary>
/// EF Core context untuk design-time schema tooling. Runtime data access memakai Dapper.
/// </summary>
// Mendefinisikan tipe class `AppDbContext` yang mewarisi atau menerapkan `DbContext`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AppDbContext : DbContext
// Membuka scope tipe AppDbContext; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan konstruktor AppDbContext yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter `options`
    // bertipe `DbContextOptions<AppDbContext>` membawa kumpulan pengaturan yang mengendalikan perilaku komponen.
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Mendefinisikan properti `AppUsers` bertipe `DbSet<AppUserEf>` untuk nilai app pengguna; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; nilai awalnya `null` dengan penegasan non-null untuk analisis compiler; operator ! tidak menambah pemeriksaan saat runtime.
    public DbSet<AppUserEf> AppUsers { get; set; } = null!;
    // Mendefinisikan properti `Sessions` bertipe `DbSet<SessionDb>` untuk nilai sessions; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; nilai awalnya `null` dengan penegasan non-null untuk analisis compiler; operator ! tidak menambah pemeriksaan saat runtime.
    public DbSet<SessionDb> Sessions { get; set; } = null!;
    // Mendefinisikan properti `Rulesets` bertipe `DbSet<RulesetDb>` untuk nilai aturan; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; nilai awalnya `null` dengan penegasan non-null untuk analisis compiler; operator ! tidak menambah pemeriksaan saat runtime.
    public DbSet<RulesetDb> Rulesets { get; set; } = null!;
    // Mendefinisikan properti `RulesetVersions` bertipe `DbSet<RulesetVersionDb>` untuk nilai aturan versions; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai; nilai awalnya `null` dengan penegasan non-null untuk analisis compiler; operator ! tidak menambah pemeriksaan saat
    // runtime.
    public DbSet<RulesetVersionDb> RulesetVersions { get; set; } = null!;
    // Mendefinisikan properti `SessionParticipants` bertipe `DbSet<SessionParticipantEf>` untuk nilai sesi participants; get menyediakan pembacaan
    // nilai, set mengizinkan penggantian nilai; nilai awalnya `null` dengan penegasan non-null untuk analisis compiler; operator ! tidak menambah
    // pemeriksaan saat runtime.
    public DbSet<SessionParticipantEf> SessionParticipants { get; set; } = null!;
    // Mendefinisikan properti `SessionStates` bertipe `DbSet<SessionStateEf>` untuk nilai sesi states; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; nilai awalnya `null` dengan penegasan non-null untuk analisis compiler; operator ! tidak menambah pemeriksaan saat runtime.
    public DbSet<SessionStateEf> SessionStates { get; set; } = null!;
    // Mendefinisikan properti `Events` bertipe `DbSet<EventDb>` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan;
    // get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai awalnya `null` dengan penegasan non-null untuk analisis compiler;
    // operator ! tidak menambah pemeriksaan saat runtime.
    public DbSet<EventDb> Events { get; set; } = null!;
    // Mendefinisikan properti `EventCashflowProjections` bertipe `DbSet<CashflowProjectionDb>` untuk nilai event arus kas projections; get menyediakan
    // pembacaan nilai, set mengizinkan penggantian nilai; nilai awalnya `null` dengan penegasan non-null untuk analisis compiler; operator ! tidak
    // menambah pemeriksaan saat runtime.
    public DbSet<CashflowProjectionDb> EventCashflowProjections { get; set; } = null!;
    // Mendefinisikan properti `MetricSnapshots` bertipe `DbSet<MetricSnapshotDb>` untuk nilai metric snapshots; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai; nilai awalnya `null` dengan penegasan non-null untuk analisis compiler; operator ! tidak menambah pemeriksaan saat
    // runtime.
    public DbSet<MetricSnapshotDb> MetricSnapshots { get; set; } = null!;
    // Mendefinisikan properti `ValidationLogs` bertipe `DbSet<ValidationLogEf>` untuk nilai validasi logs; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai; nilai awalnya `null` dengan penegasan non-null untuk analisis compiler; operator ! tidak menambah pemeriksaan saat
    // runtime.
    public DbSet<ValidationLogEf> ValidationLogs { get; set; } = null!;
    // Mendefinisikan properti `SecurityAuditLogs` bertipe `DbSet<SecurityAuditLogDb>` untuk nilai security audit logs; get menyediakan pembacaan nilai,
    // set mengizinkan penggantian nilai; nilai awalnya `null` dengan penegasan non-null untuk analisis compiler; operator ! tidak menambah pemeriksaan
    // saat runtime.
    public DbSet<SecurityAuditLogDb> SecurityAuditLogs { get; set; } = null!;

    // Mendefinisikan metode `OnModelCreating` dengan hasil bertipe `void`; operasi ini menangani on model creating. Masukan: Parameter `modelBuilder`
    // bertipe `ModelBuilder` membawa nilai model pembentuk.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    // Membuka scope metode OnModelCreating; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam OnModelCreating.
    {
        // Menjalankan memanggil `modelBuilder.HasPostgresExtension` dengan `”citext”` dalam OnModelCreating.
        modelBuilder.HasPostgresExtension("citext");
        // Menjalankan memanggil `modelBuilder.HasPostgresExtension` dengan `”pgcrypto”` dalam OnModelCreating.
        modelBuilder.HasPostgresExtension("pgcrypto");

        // Menjalankan memanggil `modelBuilder.Entity<AppUserEf>` dengan `entity => { entity.ToTable(”app_users”, table => {
        // table.HasCheckConstraint(”ck_app_users_role”, ”role in ('INSTRUCTOR', 'PLAYER')”); }); entity.HasKey(e => e.UserId).HasName(...` dalam
        // OnModelCreating.
        modelBuilder.Entity<AppUserEf>(entity =>
        // Membuka scope fungsi lambda yang dipasok ke `modelBuilder.Entity<AppUserEf>`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // OnModelCreating.
        {
            // Menjalankan memanggil `entity.ToTable` dengan `”app_users”`, `table => { table.HasCheckConstraint(”ck_app_users_role”, ”role in ('INSTRUCTOR',
            // 'PLAYER')”); }` dalam OnModelCreating.
            entity.ToTable("app_users", table =>
            // Membuka scope fungsi lambda yang dipasok ke `entity.ToTable`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam OnModelCreating.
            {
                // Menjalankan memanggil `table.HasCheckConstraint` dengan `”ck_app_users_role”`, `”role in ('INSTRUCTOR', 'PLAYER')”` dalam OnModelCreating.
                table.HasCheckConstraint("ck_app_users_role", "role in ('INSTRUCTOR', 'PLAYER')");
            // Menutup scope fungsi lambda yang dipasok ke `entity.ToTable`; bagian berikut berada di luar batas blok tersebut dalam OnModelCreating.
            });
            // Menjalankan memanggil `entity.HasKey(e => e.UserId).HasName` dengan `”pk_app_users”` dalam OnModelCreating.
            entity.HasKey(e => e.UserId).HasName("pk_app_users");
            // Menjalankan memanggil `entity.Property(e => e.UserId).HasColumnName` dengan `”user_id”` dalam OnModelCreating.
            entity.Property(e => e.UserId).HasColumnName("user_id");
            // Menjalankan memanggil `entity.Property(e => e.Username).HasColumnName(”username”).HasColumnType` dengan `”citext”` dalam OnModelCreating.
            entity.Property(e => e.Username).HasColumnName("username").HasColumnType("citext");
            // Menjalankan memanggil `entity.Property(e => e.DisplayName).HasColumnName(”display_name”).HasMaxLength` dengan `80` dalam OnModelCreating.
            entity.Property(e => e.DisplayName).HasColumnName("display_name").HasMaxLength(80);
            // Menjalankan memanggil `entity.Property(e => e.PasswordHash).HasColumnName` dengan `”password_hash”` dalam OnModelCreating.
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            // Menjalankan memanggil `entity.Property(e => e.Role).HasColumnName(”role”).HasMaxLength` dengan `20` dalam OnModelCreating.
            entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(20);
            // Menjalankan memanggil `entity.Property(e => e.IsActive).HasColumnName(”is_active”).HasDefaultValue` dengan `true` dalam OnModelCreating.
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            // Menjalankan memanggil `entity.Property(e => e.CreatedAt).HasColumnName(”created_at”).HasDefaultValueSql` dengan `”now()”` dalam OnModelCreating.
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            // Menjalankan memanggil `entity.HasIndex(e => e.Username).IsUnique().HasDatabaseName` dengan `”uq_app_users_username”` dalam OnModelCreating.
            entity.HasIndex(e => e.Username).IsUnique().HasDatabaseName("uq_app_users_username");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.Role, e.IsActive }).HasDatabaseName` dengan `”ix_app_users_role_active”` dalam
            // OnModelCreating.
            entity.HasIndex(e => new { e.Role, e.IsActive }).HasDatabaseName("ix_app_users_role_active");
        // Menutup scope fungsi lambda yang dipasok ke `modelBuilder.Entity<AppUserEf>`; bagian berikut berada di luar batas blok tersebut dalam
        // OnModelCreating.
        });

        // Menjalankan memanggil `modelBuilder.Entity<SessionDb>` dengan `entity => { entity.ToTable(”sessions”, table => {
        // table.HasCheckConstraint(”ck_sessions_mode”, ”mode in ('PEMULA', 'MAHIR')”); table.HasCheckConstraint(”ck_sessions_status”, ”...` dalam
        // OnModelCreating.
        modelBuilder.Entity<SessionDb>(entity =>
        // Membuka scope fungsi lambda yang dipasok ke `modelBuilder.Entity<SessionDb>`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // OnModelCreating.
        {
            // Menjalankan memanggil `entity.ToTable` dengan `”sessions”`, `table => { table.HasCheckConstraint(”ck_sessions_mode”, ”mode in ('PEMULA',
            // 'MAHIR')”); table.HasCheckConstraint(”ck_sessions_status”, ”status in ('CREATED', 'STARTED', 'ENDED...` dalam OnModelCreating.
            entity.ToTable("sessions", table =>
            // Membuka scope fungsi lambda yang dipasok ke `entity.ToTable`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam OnModelCreating.
            {
                // Menjalankan memanggil `table.HasCheckConstraint` dengan `”ck_sessions_mode”`, `”mode in ('PEMULA', 'MAHIR')”` dalam OnModelCreating.
                table.HasCheckConstraint("ck_sessions_mode", "mode in ('PEMULA', 'MAHIR')");
                // Menjalankan memanggil `table.HasCheckConstraint` dengan `”ck_sessions_status”`, `”status in ('CREATED', 'STARTED', 'ENDED')”` dalam
                // OnModelCreating.
                table.HasCheckConstraint("ck_sessions_status", "status in ('CREATED', 'STARTED', 'ENDED')");
                // Menjalankan memanggil `table.HasCheckConstraint` dengan `”ck_sessions_player_count”`, `”player_count between 0 and 4”` dalam OnModelCreating.
                table.HasCheckConstraint("ck_sessions_player_count", "player_count between 0 and 4");
            // Menutup scope fungsi lambda yang dipasok ke `entity.ToTable`; bagian berikut berada di luar batas blok tersebut dalam OnModelCreating.
            });
            // Menjalankan memanggil `entity.HasKey(e => e.SessionId).HasName` dengan `”pk_sessions”` dalam OnModelCreating.
            entity.HasKey(e => e.SessionId).HasName("pk_sessions");
            // Menjalankan memanggil `entity.Property(e => e.SessionId).HasColumnName` dengan `”session_id”` dalam OnModelCreating.
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            // Menjalankan memanggil `entity.Property(e => e.SessionName).HasColumnName(”session_name”).HasMaxLength` dengan `120` dalam OnModelCreating.
            entity.Property(e => e.SessionName).HasColumnName("session_name").HasMaxLength(120);
            // Menjalankan memanggil `entity.Property(e => e.Mode).HasColumnName(”mode”).HasMaxLength` dengan `10` dalam OnModelCreating.
            entity.Property(e => e.Mode).HasColumnName("mode").HasMaxLength(10);
            // Menjalankan memanggil `entity.Property(e => e.Status).HasColumnName(”status”).HasMaxLength` dengan `10` dalam OnModelCreating.
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(10);
            // Menjalankan memanggil `entity.Property<int>(”PlayerCount”).HasColumnName(”player_count”).HasDefaultValue` dengan `0` dalam OnModelCreating.
            entity.Property<int>("PlayerCount").HasColumnName("player_count").HasDefaultValue(0);
            // Menjalankan memanggil `entity.Property(e => e.StartedAt).HasColumnName` dengan `”started_at”` dalam OnModelCreating.
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
            // Menjalankan memanggil `entity.Property(e => e.EndedAt).HasColumnName` dengan `”ended_at”` dalam OnModelCreating.
            entity.Property(e => e.EndedAt).HasColumnName("ended_at");
            // Menjalankan memanggil `entity.Property(e => e.InstructorUserId).HasColumnName` dengan `”instructor_user_id”` dalam OnModelCreating.
            entity.Property(e => e.InstructorUserId).HasColumnName("instructor_user_id");
            // Menjalankan memanggil `entity.Property(e => e.RulesetVersionId).HasColumnName` dengan `”ruleset_version_id”` dalam OnModelCreating.
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            // Menjalankan memanggil `entity.Property(e => e.IsArchived).HasColumnName(”is_archived”).HasDefaultValue` dengan `false` dalam OnModelCreating.
            entity.Property(e => e.IsArchived).HasColumnName("is_archived").HasDefaultValue(false);
            // Menjalankan memanggil `entity.Property(e => e.ArchivedAt).HasColumnName` dengan `”archived_at”` dalam OnModelCreating.
            entity.Property(e => e.ArchivedAt).HasColumnName("archived_at");
            // Menjalankan memanggil `entity.Property(e => e.CreatedAt).HasColumnName(”created_at”).HasDefaultValueSql` dengan `”now()”` dalam OnModelCreating.
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            // Menjalankan memanggil `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey(e => e.InstructorUserId) .OnDelete(DeleteBehavior.SetNull)
            // .HasConstraintName` dengan `”fk_sessions_instructor_user_id”` dalam OnModelCreating.
            entity.HasOne<AppUserEf>()
                // Meneruskan fungsi lambda `entity => { entity.ToTable(”sessions”, table => { table.HasCheckConstraint(”ck_sessions_mode”, ”mode in ('PEMULA',
                // 'MAHIR')”); table.HasCheckConstraint(”ck_sessions_status”, ”...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                // argumen ke `modelBuilder.Entity<SessionDb>`.
                .WithMany()
                // Meneruskan fungsi lambda `e => e.InstructorUserId` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey`.
                .HasForeignKey(e => e.InstructorUserId)
                // Meneruskan `DeleteBehavior.SetNull` (nilai set null) sebagai argumen ke `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey(e =>
                // e.InstructorUserId) .OnDelete`.
                .OnDelete(DeleteBehavior.SetNull)
                // Meneruskan nilai literal `”fk_sessions_instructor_user_id”` sebagai argumen ke `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey(e =>
                // e.InstructorUserId) .OnDelete(DeleteBehavior.SetNull) .HasConstraintName`.
                .HasConstraintName("fk_sessions_instructor_user_id");

            // Menjalankan memanggil `entity.HasOne<RulesetVersionDb>() .WithMany() .HasForeignKey(e => e.RulesetVersionId) .OnDelete(DeleteBehavior.Restrict)
            // .HasConstraintName` dengan `”fk_sessions_ruleset_version_id”` dalam OnModelCreating.
            entity.HasOne<RulesetVersionDb>()
                // Meneruskan fungsi lambda `entity => { entity.ToTable(”sessions”, table => { table.HasCheckConstraint(”ck_sessions_mode”, ”mode in ('PEMULA',
                // 'MAHIR')”); table.HasCheckConstraint(”ck_sessions_status”, ”...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                // argumen ke `modelBuilder.Entity<SessionDb>`.
                .WithMany()
                // Meneruskan fungsi lambda `e => e.RulesetVersionId` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `entity.HasOne<RulesetVersionDb>() .WithMany() .HasForeignKey`.
                .HasForeignKey(e => e.RulesetVersionId)
                // Meneruskan `DeleteBehavior.Restrict` (nilai restrict) sebagai argumen ke `entity.HasOne<RulesetVersionDb>() .WithMany() .HasForeignKey(e =>
                // e.RulesetVersionId) .OnDelete`.
                .OnDelete(DeleteBehavior.Restrict)
                // Meneruskan nilai literal `”fk_sessions_ruleset_version_id”` sebagai argumen ke `entity.HasOne<RulesetVersionDb>() .WithMany() .HasForeignKey(e =>
                // e.RulesetVersionId) .OnDelete(DeleteBehavior.Restrict) .HasConstraintName`.
                .HasConstraintName("fk_sessions_ruleset_version_id");

            // Menjalankan memanggil `entity.HasIndex(e => e.Status).HasDatabaseName` dengan `”ix_sessions_status”` dalam OnModelCreating.
            entity.HasIndex(e => e.Status).HasDatabaseName("ix_sessions_status");
            // Menjalankan memanggil `entity.HasIndex(e => e.CreatedAt).HasDatabaseName` dengan `”ix_sessions_created_at”` dalam OnModelCreating.
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("ix_sessions_created_at");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.InstructorUserId, e.IsArchived, e.CreatedAt }).HasDatabaseName` dengan
            // `”ix_sessions_instructor_user”` dalam OnModelCreating.
            entity.HasIndex(e => new { e.InstructorUserId, e.IsArchived, e.CreatedAt }).HasDatabaseName("ix_sessions_instructor_user");
            // Menjalankan memanggil `entity.HasIndex(e => e.RulesetVersionId).HasDatabaseName` dengan `”ix_sessions_ruleset_version”` dalam OnModelCreating.
            entity.HasIndex(e => e.RulesetVersionId).HasDatabaseName("ix_sessions_ruleset_version");
        // Menutup scope fungsi lambda yang dipasok ke `modelBuilder.Entity<SessionDb>`; bagian berikut berada di luar batas blok tersebut dalam
        // OnModelCreating.
        });

        // Menjalankan memanggil `modelBuilder.Entity<RulesetDb>` dengan `entity => { entity.ToTable(”rulesets”); entity.HasKey(e =>
        // e.RulesetId).HasName(”pk_rulesets”); entity.Property(e => e.RulesetId).HasColumnName(”ruleset_id”); entity.Property(...` dalam OnModelCreating.
        modelBuilder.Entity<RulesetDb>(entity =>
        // Membuka scope fungsi lambda yang dipasok ke `modelBuilder.Entity<RulesetDb>`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // OnModelCreating.
        {
            // Menjalankan memanggil `entity.ToTable` dengan `”rulesets”` dalam OnModelCreating.
            entity.ToTable("rulesets");
            // Menjalankan memanggil `entity.HasKey(e => e.RulesetId).HasName` dengan `”pk_rulesets”` dalam OnModelCreating.
            entity.HasKey(e => e.RulesetId).HasName("pk_rulesets");
            // Menjalankan memanggil `entity.Property(e => e.RulesetId).HasColumnName` dengan `”ruleset_id”` dalam OnModelCreating.
            entity.Property(e => e.RulesetId).HasColumnName("ruleset_id");
            // Menjalankan memanggil `entity.Property(e => e.Name).HasColumnName(”name”).HasMaxLength` dengan `120` dalam OnModelCreating.
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(120);
            // Menjalankan memanggil `entity.Property(e => e.Description).HasColumnName` dengan `”description”` dalam OnModelCreating.
            entity.Property(e => e.Description).HasColumnName("description");
            // Menjalankan memanggil `entity.Property(e => e.InstructorUserId).HasColumnName` dengan `”instructor_user_id”` dalam OnModelCreating.
            entity.Property(e => e.InstructorUserId).HasColumnName("instructor_user_id");
            // Menjalankan memanggil `entity.Property(e => e.IsArchived).HasColumnName(”is_archived”).HasDefaultValue` dengan `false` dalam OnModelCreating.
            entity.Property(e => e.IsArchived).HasColumnName("is_archived").HasDefaultValue(false);
            // Menjalankan memanggil `entity.Property(e => e.ArchivedAt).HasColumnName` dengan `”archived_at”` dalam OnModelCreating.
            entity.Property(e => e.ArchivedAt).HasColumnName("archived_at");
            // Menjalankan memanggil `entity.Property(e => e.CreatedAt).HasColumnName(”created_at”).HasDefaultValueSql` dengan `”now()”` dalam OnModelCreating.
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            // Menjalankan memanggil `entity.Property(e => e.CreatedByUserId).HasColumnName` dengan `”created_by_user_id”` dalam OnModelCreating.
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");

            // Menjalankan memanggil `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey(e => e.InstructorUserId) .OnDelete(DeleteBehavior.SetNull)
            // .HasConstraintName` dengan `”fk_rulesets_instructor_user_id”` dalam OnModelCreating.
            entity.HasOne<AppUserEf>()
                // Meneruskan fungsi lambda `entity => { entity.ToTable(”rulesets”); entity.HasKey(e => e.RulesetId).HasName(”pk_rulesets”); entity.Property(e =>
                // e.RulesetId).HasColumnName(”ruleset_id”); entity.Property(...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                // argumen ke `modelBuilder.Entity<RulesetDb>`.
                .WithMany()
                // Meneruskan fungsi lambda `e => e.InstructorUserId` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey`.
                .HasForeignKey(e => e.InstructorUserId)
                // Meneruskan `DeleteBehavior.SetNull` (nilai set null) sebagai argumen ke `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey(e =>
                // e.InstructorUserId) .OnDelete`.
                .OnDelete(DeleteBehavior.SetNull)
                // Meneruskan nilai literal `”fk_rulesets_instructor_user_id”` sebagai argumen ke `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey(e =>
                // e.InstructorUserId) .OnDelete(DeleteBehavior.SetNull) .HasConstraintName`.
                .HasConstraintName("fk_rulesets_instructor_user_id");

            // Menjalankan memanggil `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey(e => e.CreatedByUserId) .OnDelete(DeleteBehavior.SetNull)
            // .HasConstraintName` dengan `”fk_rulesets_created_by_user_id”` dalam OnModelCreating.
            entity.HasOne<AppUserEf>()
                // Meneruskan fungsi lambda `entity => { entity.ToTable(”rulesets”); entity.HasKey(e => e.RulesetId).HasName(”pk_rulesets”); entity.Property(e =>
                // e.RulesetId).HasColumnName(”ruleset_id”); entity.Property(...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                // argumen ke `modelBuilder.Entity<RulesetDb>`.
                .WithMany()
                // Meneruskan fungsi lambda `e => e.CreatedByUserId` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey`.
                .HasForeignKey(e => e.CreatedByUserId)
                // Meneruskan `DeleteBehavior.SetNull` (nilai set null) sebagai argumen ke `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey(e =>
                // e.CreatedByUserId) .OnDelete`.
                .OnDelete(DeleteBehavior.SetNull)
                // Meneruskan nilai literal `”fk_rulesets_created_by_user_id”` sebagai argumen ke `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey(e =>
                // e.CreatedByUserId) .OnDelete(DeleteBehavior.SetNull) .HasConstraintName`.
                .HasConstraintName("fk_rulesets_created_by_user_id");

            // Menjalankan memanggil `entity.HasIndex(e => e.CreatedAt).HasDatabaseName` dengan `”ix_rulesets_created_at”` dalam OnModelCreating.
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("ix_rulesets_created_at");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.InstructorUserId, e.IsArchived, e.CreatedAt }).HasDatabaseName` dengan
            // `”ix_rulesets_instructor_user”` dalam OnModelCreating.
            entity.HasIndex(e => new { e.InstructorUserId, e.IsArchived, e.CreatedAt }).HasDatabaseName("ix_rulesets_instructor_user");
        // Menutup scope fungsi lambda yang dipasok ke `modelBuilder.Entity<RulesetDb>`; bagian berikut berada di luar batas blok tersebut dalam
        // OnModelCreating.
        });

        // Menjalankan memanggil `modelBuilder.Entity<RulesetVersionDb>` dengan `entity => { entity.ToTable(”ruleset_versions”, table => {
        // table.HasCheckConstraint(”ck_ruleset_versions_version”, ”version >= 1”); table.HasCheckConstraint(”ck_ruleset_version...` dalam OnModelCreating.
        modelBuilder.Entity<RulesetVersionDb>(entity =>
        // Membuka scope fungsi lambda yang dipasok ke `modelBuilder.Entity<RulesetVersionDb>`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam OnModelCreating.
        {
            // Menjalankan memanggil `entity.ToTable` dengan `”ruleset_versions”`, `table => { table.HasCheckConstraint(”ck_ruleset_versions_version”, ”version
            // >= 1”); table.HasCheckConstraint(”ck_ruleset_versions_status”, ”status in ('DRAFT', 'ACTIVE', 'ARCH...` dalam OnModelCreating.
            entity.ToTable("ruleset_versions", table =>
            // Membuka scope fungsi lambda yang dipasok ke `entity.ToTable`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam OnModelCreating.
            {
                // Menjalankan memanggil `table.HasCheckConstraint` dengan `”ck_ruleset_versions_version”`, `”version >= 1”` dalam OnModelCreating.
                table.HasCheckConstraint("ck_ruleset_versions_version", "version >= 1");
                // Menjalankan memanggil `table.HasCheckConstraint` dengan `”ck_ruleset_versions_status”`, `”status in ('DRAFT', 'ACTIVE', 'ARCHIVED')”` dalam
                // OnModelCreating.
                table.HasCheckConstraint("ck_ruleset_versions_status", "status in ('DRAFT', 'ACTIVE', 'ARCHIVED')");
                // Menjalankan memanggil `table.HasCheckConstraint` dengan `”ck_ruleset_versions_mode”`, `”mode in ('PEMULA', 'MAHIR')”` dalam OnModelCreating.
                table.HasCheckConstraint("ck_ruleset_versions_mode", "mode in ('PEMULA', 'MAHIR')");
            // Menutup scope fungsi lambda yang dipasok ke `entity.ToTable`; bagian berikut berada di luar batas blok tersebut dalam OnModelCreating.
            });
            // Menjalankan memanggil `entity.HasKey(e => e.RulesetVersionId).HasName` dengan `”pk_ruleset_versions”` dalam OnModelCreating.
            entity.HasKey(e => e.RulesetVersionId).HasName("pk_ruleset_versions");
            // Menjalankan memanggil `entity.Ignore` dengan `e => e.Definition` dalam OnModelCreating.
            entity.Ignore(e => e.Definition);
            // Menjalankan memanggil `entity.Property(e => e.RulesetVersionId).HasColumnName` dengan `”ruleset_version_id”` dalam OnModelCreating.
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            // Menjalankan memanggil `entity.Property(e => e.RulesetId).HasColumnName` dengan `”ruleset_id”` dalam OnModelCreating.
            entity.Property(e => e.RulesetId).HasColumnName("ruleset_id");
            // Menjalankan memanggil `entity.Property(e => e.Version).HasColumnName` dengan `”version”` dalam OnModelCreating.
            entity.Property(e => e.Version).HasColumnName("version");
            // Menjalankan memanggil `entity.Property(e => e.Status).HasColumnName(”status”).HasMaxLength` dengan `10` dalam OnModelCreating.
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(10);
            // Menjalankan memanggil `entity.Property(e => e.Mode).HasColumnName(”mode”).HasMaxLength` dengan `10` dalam OnModelCreating.
            entity.Property(e => e.Mode).HasColumnName("mode").HasMaxLength(10);
            // Menjalankan memanggil `entity.Property<string>(”SchemaVersion”).HasColumnName(”schema_version”).HasMaxLength(20).HasDefaultValue` dengan
            // `”3.0.0”` dalam OnModelCreating.
            entity.Property<string>("SchemaVersion").HasColumnName("schema_version").HasMaxLength(20).HasDefaultValue("3.0.0");
            // Menjalankan memanggil `entity.Property(e => e.ConfigHash).HasColumnName(”config_hash”).HasMaxLength` dengan `128` dalam OnModelCreating.
            entity.Property(e => e.ConfigHash).HasColumnName("config_hash").HasMaxLength(128);
            // Menjalankan memanggil `entity.Property<string?>(”ChangeNote”).HasColumnName` dengan `”change_note”` dalam OnModelCreating.
            entity.Property<string?>("ChangeNote").HasColumnName("change_note");
            // Menjalankan memanggil `entity.Property<DateTimeOffset?>(”PublishedAt”).HasColumnName` dengan `”published_at”` dalam OnModelCreating.
            entity.Property<DateTimeOffset?>("PublishedAt").HasColumnName("published_at");
            // Menjalankan memanggil `entity.Property(e => e.CreatedAt).HasColumnName(”created_at”).HasDefaultValueSql` dengan `”now()”` dalam OnModelCreating.
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            // Menjalankan memanggil `entity.Property(e => e.CreatedByUserId).HasColumnName` dengan `”created_by_user_id”` dalam OnModelCreating.
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");

            // Menjalankan memanggil `entity.HasOne<RulesetDb>() .WithMany() .HasForeignKey(e => e.RulesetId) .OnDelete(DeleteBehavior.Restrict)
            // .HasConstraintName` dengan `”fk_ruleset_versions_ruleset_id”` dalam OnModelCreating.
            entity.HasOne<RulesetDb>()
                // Meneruskan fungsi lambda `entity => { entity.ToTable(”ruleset_versions”, table => { table.HasCheckConstraint(”ck_ruleset_versions_version”,
                // ”version >= 1”); table.HasCheckConstraint(”ck_ruleset_version...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                // argumen ke `modelBuilder.Entity<RulesetVersionDb>`.
                .WithMany()
                // Meneruskan fungsi lambda `e => e.RulesetId` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `entity.HasOne<RulesetDb>() .WithMany() .HasForeignKey`.
                .HasForeignKey(e => e.RulesetId)
                // Meneruskan `DeleteBehavior.Restrict` (nilai restrict) sebagai argumen ke `entity.HasOne<RulesetDb>() .WithMany() .HasForeignKey(e => e.RulesetId)
                // .OnDelete`.
                .OnDelete(DeleteBehavior.Restrict)
                // Meneruskan nilai literal `”fk_ruleset_versions_ruleset_id”` sebagai argumen ke `entity.HasOne<RulesetDb>() .WithMany() .HasForeignKey(e =>
                // e.RulesetId) .OnDelete(DeleteBehavior.Restrict) .HasConstraintName`.
                .HasConstraintName("fk_ruleset_versions_ruleset_id");

            // Menjalankan memanggil `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey(e => e.CreatedByUserId) .OnDelete(DeleteBehavior.SetNull)
            // .HasConstraintName` dengan `”fk_ruleset_versions_created_by_user_id”` dalam OnModelCreating.
            entity.HasOne<AppUserEf>()
                // Meneruskan fungsi lambda `entity => { entity.ToTable(”ruleset_versions”, table => { table.HasCheckConstraint(”ck_ruleset_versions_version”,
                // ”version >= 1”); table.HasCheckConstraint(”ck_ruleset_version...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                // argumen ke `modelBuilder.Entity<RulesetVersionDb>`.
                .WithMany()
                // Meneruskan fungsi lambda `e => e.CreatedByUserId` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey`.
                .HasForeignKey(e => e.CreatedByUserId)
                // Meneruskan `DeleteBehavior.SetNull` (nilai set null) sebagai argumen ke `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey(e =>
                // e.CreatedByUserId) .OnDelete`.
                .OnDelete(DeleteBehavior.SetNull)
                // Meneruskan nilai literal `”fk_ruleset_versions_created_by_user_id”` sebagai argumen ke `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey(e
                // => e.CreatedByUserId) .OnDelete(DeleteBehavior.SetNull) .HasConstraintName`.
                .HasConstraintName("fk_ruleset_versions_created_by_user_id");

            // Menjalankan memanggil `entity.HasIndex(e => new { e.RulesetId, e.Version }).IsUnique().HasDatabaseName` dengan
            // `”uq_ruleset_versions_ruleset_version”` dalam OnModelCreating.
            entity.HasIndex(e => new { e.RulesetId, e.Version }).IsUnique().HasDatabaseName("uq_ruleset_versions_ruleset_version");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.RulesetId, e.ConfigHash }).IsUnique().HasDatabaseName` dengan
            // `”uq_ruleset_versions_ruleset_config_hash”` dalam OnModelCreating.
            entity.HasIndex(e => new { e.RulesetId, e.ConfigHash }).IsUnique().HasDatabaseName("uq_ruleset_versions_ruleset_config_hash");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.Status, e.Mode, e.CreatedAt }).HasDatabaseName` dengan `”ix_ruleset_versions_status_mode”`
            // dalam OnModelCreating.
            entity.HasIndex(e => new { e.Status, e.Mode, e.CreatedAt }).HasDatabaseName("ix_ruleset_versions_status_mode");
            // Menjalankan memanggil `entity.HasIndex(e => e.RulesetId) .IsUnique() .HasFilter(”status = 'ACTIVE'”) .HasDatabaseName` dengan
            // `”uq_ruleset_versions_one_active_per_ruleset”` dalam OnModelCreating.
            entity.HasIndex(e => e.RulesetId)
                // Meneruskan fungsi lambda `entity => { entity.ToTable(”ruleset_versions”, table => { table.HasCheckConstraint(”ck_ruleset_versions_version”,
                // ”version >= 1”); table.HasCheckConstraint(”ck_ruleset_version...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                // argumen ke `modelBuilder.Entity<RulesetVersionDb>`.
                .IsUnique()
                // Meneruskan nilai literal `”status = 'ACTIVE'”` sebagai argumen ke `entity.HasIndex(e => e.RulesetId) .IsUnique() .HasFilter`.
                .HasFilter("status = 'ACTIVE'")
                // Meneruskan nilai literal `”uq_ruleset_versions_one_active_per_ruleset”` sebagai argumen ke `entity.HasIndex(e => e.RulesetId) .IsUnique()
                // .HasFilter(”status = 'ACTIVE'”) .HasDatabaseName`.
                .HasDatabaseName("uq_ruleset_versions_one_active_per_ruleset");
        // Menutup scope fungsi lambda yang dipasok ke `modelBuilder.Entity<RulesetVersionDb>`; bagian berikut berada di luar batas blok tersebut dalam
        // OnModelCreating.
        });

        // Menjalankan memanggil `modelBuilder.Entity<SessionParticipantEf>` dengan `entity => { entity.ToTable(”session_participants”, table => {
        // table.HasCheckConstraint(”ck_session_participants_player_order_no”, ”player_order_no between 1 and 4”); table.Has...` dalam OnModelCreating.
        modelBuilder.Entity<SessionParticipantEf>(entity =>
        // Membuka scope fungsi lambda yang dipasok ke `modelBuilder.Entity<SessionParticipantEf>`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam OnModelCreating.
        {
            // Menjalankan memanggil `entity.ToTable` dengan `”session_participants”`, `table => {
            // table.HasCheckConstraint(”ck_session_participants_player_order_no”, ”player_order_no between 1 and 4”);
            // table.HasCheckConstraint(”ck_session_participants_player_nam...` dalam OnModelCreating.
            entity.ToTable("session_participants", table =>
            // Membuka scope fungsi lambda yang dipasok ke `entity.ToTable`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam OnModelCreating.
            {
                // Menjalankan memanggil `table.HasCheckConstraint` dengan `”ck_session_participants_player_order_no”`, `”player_order_no between 1 and 4”` dalam
                // OnModelCreating.
                table.HasCheckConstraint("ck_session_participants_player_order_no", "player_order_no between 1 and 4");
                // Menjalankan memanggil `table.HasCheckConstraint` dengan `”ck_session_participants_player_name_not_blank”`, `”player_name is null or
                // nullif(btrim(player_name), '') is not null”` dalam OnModelCreating.
                table.HasCheckConstraint("ck_session_participants_player_name_not_blank", "player_name is null or nullif(btrim(player_name), '') is not null");
            // Menutup scope fungsi lambda yang dipasok ke `entity.ToTable`; bagian berikut berada di luar batas blok tersebut dalam OnModelCreating.
            });
            // Menjalankan memanggil `entity.HasKey(e => e.SessionParticipantId).HasName` dengan `”pk_session_participants”` dalam OnModelCreating.
            entity.HasKey(e => e.SessionParticipantId).HasName("pk_session_participants");
            // Menjalankan memanggil `entity.HasAlternateKey(e => new { e.SessionId, e.SessionParticipantId }).HasName` dengan
            // `”uq_session_participants_session_participant”` dalam OnModelCreating.
            entity.HasAlternateKey(e => new { e.SessionId, e.SessionParticipantId }).HasName("uq_session_participants_session_participant");
            // Menjalankan memanggil `entity.Property(e => e.SessionParticipantId).HasColumnName` dengan `”session_participant_id”` dalam OnModelCreating.
            entity.Property(e => e.SessionParticipantId).HasColumnName("session_participant_id");
            // Menjalankan memanggil `entity.Property(e => e.SessionId).HasColumnName` dengan `”session_id”` dalam OnModelCreating.
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            // Menjalankan memanggil `entity.Property(e => e.UserId).HasColumnName` dengan `”user_id”` dalam OnModelCreating.
            entity.Property(e => e.UserId).HasColumnName("user_id");
            // Menjalankan memanggil `entity.Property(e => e.PlayerOrderNo).HasColumnName` dengan `”player_order_no”` dalam OnModelCreating.
            entity.Property(e => e.PlayerOrderNo).HasColumnName("player_order_no");
            // Menjalankan memanggil `entity.Property(e => e.PlayerName).HasColumnName(”player_name”).HasMaxLength` dengan `80` dalam OnModelCreating.
            entity.Property(e => e.PlayerName).HasColumnName("player_name").HasMaxLength(80);
            // Menjalankan memanggil `entity.Property(e => e.JoinedAt).HasColumnName(”joined_at”).HasDefaultValueSql` dengan `”now()”` dalam OnModelCreating.
            entity.Property(e => e.JoinedAt).HasColumnName("joined_at").HasDefaultValueSql("now()");

            // Menjalankan memanggil `entity.HasOne<SessionDb>() .WithMany() .HasForeignKey(e => e.SessionId) .OnDelete(DeleteBehavior.Restrict)
            // .HasConstraintName` dengan `”fk_session_participants_session_id”` dalam OnModelCreating.
            entity.HasOne<SessionDb>()
                // Meneruskan fungsi lambda `entity => { entity.ToTable(”session_participants”, table => {
                // table.HasCheckConstraint(”ck_session_participants_player_order_no”, ”player_order_no between 1 and 4”); table.Has...` yang dijalankan oleh
                // operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `modelBuilder.Entity<SessionParticipantEf>`.
                .WithMany()
                // Meneruskan fungsi lambda `e => e.SessionId` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `entity.HasOne<SessionDb>() .WithMany() .HasForeignKey`.
                .HasForeignKey(e => e.SessionId)
                // Meneruskan `DeleteBehavior.Restrict` (nilai restrict) sebagai argumen ke `entity.HasOne<SessionDb>() .WithMany() .HasForeignKey(e => e.SessionId)
                // .OnDelete`.
                .OnDelete(DeleteBehavior.Restrict)
                // Meneruskan nilai literal `”fk_session_participants_session_id”` sebagai argumen ke `entity.HasOne<SessionDb>() .WithMany() .HasForeignKey(e =>
                // e.SessionId) .OnDelete(DeleteBehavior.Restrict) .HasConstraintName`.
                .HasConstraintName("fk_session_participants_session_id");
            // Menjalankan memanggil `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey(e => e.UserId) .OnDelete(DeleteBehavior.Restrict)
            // .HasConstraintName` dengan `”fk_session_participants_user_id”` dalam OnModelCreating.
            entity.HasOne<AppUserEf>()
                // Meneruskan fungsi lambda `entity => { entity.ToTable(”session_participants”, table => {
                // table.HasCheckConstraint(”ck_session_participants_player_order_no”, ”player_order_no between 1 and 4”); table.Has...` yang dijalankan oleh
                // operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `modelBuilder.Entity<SessionParticipantEf>`.
                .WithMany()
                // Meneruskan fungsi lambda `e => e.UserId` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey`.
                .HasForeignKey(e => e.UserId)
                // Meneruskan `DeleteBehavior.Restrict` (nilai restrict) sebagai argumen ke `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey(e => e.UserId)
                // .OnDelete`.
                .OnDelete(DeleteBehavior.Restrict)
                // Meneruskan nilai literal `”fk_session_participants_user_id”` sebagai argumen ke `entity.HasOne<AppUserEf>() .WithMany() .HasForeignKey(e =>
                // e.UserId) .OnDelete(DeleteBehavior.Restrict) .HasConstraintName`.
                .HasConstraintName("fk_session_participants_user_id");

            // Menjalankan memanggil `entity.HasIndex(e => new { e.SessionId, e.UserId }).IsUnique().HasDatabaseName` dengan
            // `”uq_session_participants_session_user”` dalam OnModelCreating.
            entity.HasIndex(e => new { e.SessionId, e.UserId }).IsUnique().HasDatabaseName("uq_session_participants_session_user");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.SessionId, e.PlayerOrderNo }).IsUnique().HasDatabaseName` dengan
            // `”uq_session_participants_session_seat”` dalam OnModelCreating.
            entity.HasIndex(e => new { e.SessionId, e.PlayerOrderNo }).IsUnique().HasDatabaseName("uq_session_participants_session_seat");
            // Menjalankan memanggil `entity.HasIndex(e => e.UserId).HasDatabaseName` dengan `”ix_session_participants_user”` dalam OnModelCreating.
            entity.HasIndex(e => e.UserId).HasDatabaseName("ix_session_participants_user");
        // Menutup scope fungsi lambda yang dipasok ke `modelBuilder.Entity<SessionParticipantEf>`; bagian berikut berada di luar batas blok tersebut dalam
        // OnModelCreating.
        });

        // Menjalankan memanggil `modelBuilder.Entity<SessionStateEf>` dengan `entity => { entity.ToTable(”session_states”); entity.HasKey(e =>
        // e.SessionId).HasName(”pk_session_states”); entity.Property(e => e.SessionId).HasColumnName(”session_id”); enti...` dalam OnModelCreating.
        modelBuilder.Entity<SessionStateEf>(entity =>
        // Membuka scope fungsi lambda yang dipasok ke `modelBuilder.Entity<SessionStateEf>`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam OnModelCreating.
        {
            // Menjalankan memanggil `entity.ToTable` dengan `”session_states”` dalam OnModelCreating.
            entity.ToTable("session_states");
            // Menjalankan memanggil `entity.HasKey(e => e.SessionId).HasName` dengan `”pk_session_states”` dalam OnModelCreating.
            entity.HasKey(e => e.SessionId).HasName("pk_session_states");
            // Menjalankan memanggil `entity.Property(e => e.SessionId).HasColumnName` dengan `”session_id”` dalam OnModelCreating.
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            // Menjalankan memanggil `entity.Property(e => e.Day).HasColumnName` dengan `”day”` dalam OnModelCreating.
            entity.Property(e => e.Day).HasColumnName("day");
            // Menjalankan memanggil `entity.Property(e => e.Weekday).HasColumnName(”weekday”).HasMaxLength` dengan `3` dalam OnModelCreating.
            entity.Property(e => e.Weekday).HasColumnName("weekday").HasMaxLength(3);
            // Menjalankan memanggil `entity.Property(e => e.TurnNumber).HasColumnName` dengan `”turn_number”` dalam OnModelCreating.
            entity.Property(e => e.TurnNumber).HasColumnName("turn_number");
            // Menjalankan memanggil `entity.Property(e => e.ActionSlot).HasColumnName` dengan `”action_slot”` dalam OnModelCreating.
            entity.Property(e => e.ActionSlot).HasColumnName("action_slot");
            // Menjalankan memanggil `entity.Property(e => e.CurrentSessionPlayerId).HasColumnName` dengan `”current_session_player_id”` dalam OnModelCreating.
            entity.Property(e => e.CurrentSessionPlayerId).HasColumnName("current_session_player_id");
            // Menjalankan memanggil `entity.Property(e => e.CurrentActionSlot).HasColumnName(”current_action_slot”).HasDefaultValue` dengan `1` dalam
            // OnModelCreating.
            entity.Property(e => e.CurrentActionSlot).HasColumnName("current_action_slot").HasDefaultValue(1);
            // Menjalankan memanggil `entity.Property(e => e.ActionSlotsLeft).HasColumnName` dengan `”action_slots_left”` dalam OnModelCreating.
            entity.Property(e => e.ActionSlotsLeft).HasColumnName("action_slots_left");
            // Menjalankan memanggil `entity.Property(e => e.FinishDay).HasColumnName` dengan `”finish_day”` dalam OnModelCreating.
            entity.Property(e => e.FinishDay).HasColumnName("finish_day");
            // Menjalankan memanggil `entity.Property(e => e.Phase).HasColumnName(”phase”).HasMaxLength(30).HasDefaultValue` dengan `”PLAYER_TURN”` dalam
            // OnModelCreating.
            entity.Property(e => e.Phase).HasColumnName("phase").HasMaxLength(30).HasDefaultValue("PLAYER_TURN");
            // Menjalankan memanggil `entity.Property(e => e.IsGameOver).HasColumnName(”is_game_over”).HasDefaultValue` dengan `false` dalam OnModelCreating.
            entity.Property(e => e.IsGameOver).HasColumnName("is_game_over").HasDefaultValue(false);
            // Menjalankan memanggil `entity.Property(e => e.StateVersion).HasColumnName(”state_version”).HasDefaultValue` dengan `1L` dalam OnModelCreating.
            entity.Property(e => e.StateVersion).HasColumnName("state_version").HasDefaultValue(1L);
            // Menjalankan memanggil `entity.Property(e => e.LastEventId).HasColumnName` dengan `”last_event_id”` dalam OnModelCreating.
            entity.Property(e => e.LastEventId).HasColumnName("last_event_id");
            // Menjalankan memanggil `entity.Property(e => e.UiStateJson).HasColumnName(”ui_state_json”).HasColumnType(”jsonb”).HasDefaultValueSql` dengan
            // `”'{}'::jsonb”` dalam OnModelCreating.
            entity.Property(e => e.UiStateJson).HasColumnName("ui_state_json").HasColumnType("jsonb").HasDefaultValueSql("'{}'::jsonb");
            // Menjalankan memanggil `entity.Property(e => e.CreatedAt).HasColumnName(”created_at”).HasDefaultValueSql` dengan `”now()”` dalam OnModelCreating.
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            // Menjalankan memanggil `entity.Property(e => e.UpdatedAt).HasColumnName(”updated_at”).HasDefaultValueSql` dengan `”now()”` dalam OnModelCreating.
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");

            // Menjalankan memanggil `entity.HasOne<SessionDb>() .WithMany() .HasForeignKey(e => e.SessionId) .OnDelete(DeleteBehavior.Restrict)
            // .HasConstraintName` dengan `”fk_session_states_session_id”` dalam OnModelCreating.
            entity.HasOne<SessionDb>()
                // Meneruskan fungsi lambda `entity => { entity.ToTable(”session_states”); entity.HasKey(e => e.SessionId).HasName(”pk_session_states”);
                // entity.Property(e => e.SessionId).HasColumnName(”session_id”); enti...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan
                // sebagai argumen ke `modelBuilder.Entity<SessionStateEf>`.
                .WithMany()
                // Meneruskan fungsi lambda `e => e.SessionId` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `entity.HasOne<SessionDb>() .WithMany() .HasForeignKey`.
                .HasForeignKey(e => e.SessionId)
                // Meneruskan `DeleteBehavior.Restrict` (nilai restrict) sebagai argumen ke `entity.HasOne<SessionDb>() .WithMany() .HasForeignKey(e => e.SessionId)
                // .OnDelete`.
                .OnDelete(DeleteBehavior.Restrict)
                // Meneruskan nilai literal `”fk_session_states_session_id”` sebagai argumen ke `entity.HasOne<SessionDb>() .WithMany() .HasForeignKey(e =>
                // e.SessionId) .OnDelete(DeleteBehavior.Restrict) .HasConstraintName`.
                .HasConstraintName("fk_session_states_session_id");
        // Menutup scope fungsi lambda yang dipasok ke `modelBuilder.Entity<SessionStateEf>`; bagian berikut berada di luar batas blok tersebut dalam
        // OnModelCreating.
        });

        // Menjalankan memanggil `modelBuilder.Entity<EventDb>` dengan `entity => { entity.ToTable(”events”); entity.HasKey(e =>
        // e.EventPk).HasName(”pk_events”); entity.Property(e => e.EventPk).HasColumnName(”event_pk”); entity.Property(e => e.Eve...` dalam OnModelCreating.
        modelBuilder.Entity<EventDb>(entity =>
        // Membuka scope fungsi lambda yang dipasok ke `modelBuilder.Entity<EventDb>`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // OnModelCreating.
        {
            // Menjalankan memanggil `entity.ToTable` dengan `”events”` dalam OnModelCreating.
            entity.ToTable("events");
            // Menjalankan memanggil `entity.HasKey(e => e.EventPk).HasName` dengan `”pk_events”` dalam OnModelCreating.
            entity.HasKey(e => e.EventPk).HasName("pk_events");
            // Menjalankan memanggil `entity.Property(e => e.EventPk).HasColumnName` dengan `”event_pk”` dalam OnModelCreating.
            entity.Property(e => e.EventPk).HasColumnName("event_pk");
            // Menjalankan memanggil `entity.Property(e => e.EventId).HasColumnName` dengan `”event_id”` dalam OnModelCreating.
            entity.Property(e => e.EventId).HasColumnName("event_id");
            // Menjalankan memanggil `entity.Property(e => e.SessionId).HasColumnName` dengan `”session_id”` dalam OnModelCreating.
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            // Menjalankan memanggil `entity.Property(e => e.SessionPlayerId).HasColumnName` dengan `”session_player_id”` dalam OnModelCreating.
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            // Menjalankan memanggil `entity.Property(e => e.UserId).HasColumnName` dengan `”user_id”` dalam OnModelCreating.
            entity.Property(e => e.UserId).HasColumnName("user_id");
            // Menjalankan memanggil `entity.Property(e => e.ActorType).HasColumnName(”actor_type”).HasMaxLength` dengan `10` dalam OnModelCreating.
            entity.Property(e => e.ActorType).HasColumnName("actor_type").HasMaxLength(10);
            // Menjalankan memanggil `entity.Property(e => e.Timestamp).HasColumnName` dengan `”timestamp”` dalam OnModelCreating.
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            // Menjalankan memanggil `entity.Property(e => e.DayIndex).HasColumnName` dengan `”day_index”` dalam OnModelCreating.
            entity.Property(e => e.DayIndex).HasColumnName("day_index");
            // Menjalankan memanggil `entity.Property(e => e.Weekday).HasColumnName(”weekday”).HasMaxLength` dengan `3` dalam OnModelCreating.
            entity.Property(e => e.Weekday).HasColumnName("weekday").HasMaxLength(3);
            // Menjalankan memanggil `entity.Property(e => e.TurnNumber).HasColumnName` dengan `”turn_number”` dalam OnModelCreating.
            entity.Property(e => e.TurnNumber).HasColumnName("turn_number");
            // Menjalankan memanggil `entity.Property(e => e.ActionSlot).HasColumnName` dengan `”action_slot”` dalam OnModelCreating.
            entity.Property(e => e.ActionSlot).HasColumnName("action_slot");
            // Menjalankan memanggil `entity.Property(e => e.SequenceNumber).HasColumnName` dengan `”sequence_number”` dalam OnModelCreating.
            entity.Property(e => e.SequenceNumber).HasColumnName("sequence_number");
            // Menjalankan memanggil `entity.Property(e => e.RulesetActionId).HasColumnName` dengan `”ruleset_action_id”` dalam OnModelCreating.
            entity.Property(e => e.RulesetActionId).HasColumnName("ruleset_action_id");
            // Menjalankan memanggil `entity.Ignore` dengan `e => e.ActionId` dalam OnModelCreating.
            entity.Ignore(e => e.ActionId);
            // Menjalankan memanggil `entity.Property(e => e.ActionType).HasColumnName(”action_type”).HasMaxLength` dengan `80` dalam OnModelCreating.
            entity.Property(e => e.ActionType).HasColumnName("action_type").HasMaxLength(80);
            // Menjalankan memanggil `entity.Property(e => e.RulesetVersionId).HasColumnName` dengan `”ruleset_version_id”` dalam OnModelCreating.
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            // Menjalankan memanggil `entity.Property(e => e.PayloadVersion).HasColumnName(”payload_version”).HasMaxLength(20).HasDefaultValue` dengan `”1.0”`
            // dalam OnModelCreating.
            entity.Property(e => e.PayloadVersion).HasColumnName("payload_version").HasMaxLength(20).HasDefaultValue("1.0");
            // Menjalankan memanggil `entity.Property(e => e.Payload).HasColumnName(”payload”).HasColumnType` dengan `”jsonb”` dalam OnModelCreating.
            entity.Property(e => e.Payload).HasColumnName("payload").HasColumnType("jsonb");
            // Menjalankan memanggil `entity.Property(e => e.ReceivedAt).HasColumnName(”received_at”).HasDefaultValueSql` dengan `”now()”` dalam
            // OnModelCreating.
            entity.Property(e => e.ReceivedAt).HasColumnName("received_at").HasDefaultValueSql("now()");
            // Menjalankan memanggil `entity.Property(e => e.ClientRequestId).HasColumnName(”client_request_id”).HasMaxLength` dengan `120` dalam
            // OnModelCreating.
            entity.Property(e => e.ClientRequestId).HasColumnName("client_request_id").HasMaxLength(120);

            // Menjalankan memanggil `entity.HasIndex(e => new { e.SessionId, e.EventId }).IsUnique().HasDatabaseName` dengan `”uq_events_session_event”` dalam
            // OnModelCreating.
            entity.HasIndex(e => new { e.SessionId, e.EventId }).IsUnique().HasDatabaseName("uq_events_session_event");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.SessionId, e.SequenceNumber }).IsUnique().HasDatabaseName` dengan
            // `”uq_events_session_sequence”` dalam OnModelCreating.
            entity.HasIndex(e => new { e.SessionId, e.SequenceNumber }).IsUnique().HasDatabaseName("uq_events_session_sequence");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.SessionId, e.ClientRequestId }) .IsUnique() .HasFilter(”client_request_id is not null”)
            // .HasDatabaseName` dengan `”uq_events_session_client_request”` dalam OnModelCreating.
            entity.HasIndex(e => new { e.SessionId, e.ClientRequestId })
                // Meneruskan fungsi lambda `entity => { entity.ToTable(”events”); entity.HasKey(e => e.EventPk).HasName(”pk_events”); entity.Property(e =>
                // e.EventPk).HasColumnName(”event_pk”); entity.Property(e => e.Eve...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan
                // sebagai argumen ke `modelBuilder.Entity<EventDb>`.
                .IsUnique()
                // Meneruskan nilai literal `”client_request_id is not null”` sebagai argumen ke `entity.HasIndex(e => new { e.SessionId, e.ClientRequestId })
                // .IsUnique() .HasFilter`.
                .HasFilter("client_request_id is not null")
                // Meneruskan nilai literal `”uq_events_session_client_request”` sebagai argumen ke `entity.HasIndex(e => new { e.SessionId, e.ClientRequestId })
                // .IsUnique() .HasFilter(”client_request_id is not null”) .HasDatabaseName`.
                .HasDatabaseName("uq_events_session_client_request");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.SessionId, e.SessionPlayerId, e.SequenceNumber }).HasDatabaseName` dengan
            // `”ix_events_session_player_seq”` dalam OnModelCreating.
            entity.HasIndex(e => new { e.SessionId, e.SessionPlayerId, e.SequenceNumber }).HasDatabaseName("ix_events_session_player_seq");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.SessionId, e.Timestamp }).HasDatabaseName` dengan `”ix_events_session_time”` dalam
            // OnModelCreating.
            entity.HasIndex(e => new { e.SessionId, e.Timestamp }).HasDatabaseName("ix_events_session_time");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.SessionId, e.ActionType }).HasDatabaseName` dengan `”ix_events_session_action”` dalam
            // OnModelCreating.
            entity.HasIndex(e => new { e.SessionId, e.ActionType }).HasDatabaseName("ix_events_session_action");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.UserId, e.Timestamp }).HasDatabaseName` dengan `”ix_events_user_time”` dalam OnModelCreating.
            entity.HasIndex(e => new { e.UserId, e.Timestamp }).HasDatabaseName("ix_events_user_time");
            // Menjalankan memanggil `entity.HasIndex(e => e.ReceivedAt).HasDatabaseName` dengan `”ix_events_received_at”` dalam OnModelCreating.
            entity.HasIndex(e => e.ReceivedAt).HasDatabaseName("ix_events_received_at");
        // Menutup scope fungsi lambda yang dipasok ke `modelBuilder.Entity<EventDb>`; bagian berikut berada di luar batas blok tersebut dalam
        // OnModelCreating.
        });

        // Menjalankan memanggil `modelBuilder.Entity<CashflowProjectionDb>` dengan `entity => { entity.ToTable(”event_cashflow_projections”);
        // entity.HasKey(e => e.ProjectionId).HasName(”pk_event_cashflow_projections”); entity.Property(e => e.ProjectionId).Has...` dalam OnModelCreating.
        modelBuilder.Entity<CashflowProjectionDb>(entity =>
        // Membuka scope fungsi lambda yang dipasok ke `modelBuilder.Entity<CashflowProjectionDb>`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam OnModelCreating.
        {
            // Menjalankan memanggil `entity.ToTable` dengan `”event_cashflow_projections”` dalam OnModelCreating.
            entity.ToTable("event_cashflow_projections");
            // Menjalankan memanggil `entity.HasKey(e => e.ProjectionId).HasName` dengan `”pk_event_cashflow_projections”` dalam OnModelCreating.
            entity.HasKey(e => e.ProjectionId).HasName("pk_event_cashflow_projections");
            // Menjalankan memanggil `entity.Property(e => e.ProjectionId).HasColumnName` dengan `”projection_id”` dalam OnModelCreating.
            entity.Property(e => e.ProjectionId).HasColumnName("projection_id");
            // Menjalankan memanggil `entity.Property(e => e.SessionId).HasColumnName` dengan `”session_id”` dalam OnModelCreating.
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            // Menjalankan memanggil `entity.Property(e => e.UserId).HasColumnName` dengan `”user_id”` dalam OnModelCreating.
            entity.Property(e => e.UserId).HasColumnName("user_id");
            // Menjalankan memanggil `entity.Property(e => e.EventPk).HasColumnName` dengan `”event_pk”` dalam OnModelCreating.
            entity.Property(e => e.EventPk).HasColumnName("event_pk");
            // Menjalankan memanggil `entity.Property(e => e.EventId).HasColumnName` dengan `”event_id”` dalam OnModelCreating.
            entity.Property(e => e.EventId).HasColumnName("event_id");
            // Menjalankan memanggil `entity.Property(e => e.ProjectionOrder).HasColumnName(”projection_order”).HasDefaultValue` dengan `1` dalam
            // OnModelCreating.
            entity.Property(e => e.ProjectionOrder).HasColumnName("projection_order").HasDefaultValue(1);
            // Menjalankan memanggil `entity.Property(e => e.Timestamp).HasColumnName` dengan `”timestamp”` dalam OnModelCreating.
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            // Menjalankan memanggil `entity.Property(e => e.Direction).HasColumnName(”direction”).HasMaxLength` dengan `3` dalam OnModelCreating.
            entity.Property(e => e.Direction).HasColumnName("direction").HasMaxLength(3);
            // Menjalankan memanggil `entity.Property(e => e.Amount).HasColumnName` dengan `”amount”` dalam OnModelCreating.
            entity.Property(e => e.Amount).HasColumnName("amount");
            // Menjalankan memanggil `entity.Property(e => e.Category).HasColumnName(”category”).HasMaxLength` dengan `40` dalam OnModelCreating.
            entity.Property(e => e.Category).HasColumnName("category").HasMaxLength(40);
            // Menjalankan memanggil `entity.Property(e => e.Counterparty).HasColumnName(”counterparty”).HasMaxLength` dengan `40` dalam OnModelCreating.
            entity.Property(e => e.Counterparty).HasColumnName("counterparty").HasMaxLength(40);
            // Menjalankan memanggil `entity.Property(e => e.Reference).HasColumnName(”reference”).HasMaxLength` dengan `120` dalam OnModelCreating.
            entity.Property(e => e.Reference).HasColumnName("reference").HasMaxLength(120);
            // Menjalankan memanggil `entity.Property(e => e.Note).HasColumnName(”note”).HasMaxLength` dengan `200` dalam OnModelCreating.
            entity.Property(e => e.Note).HasColumnName("note").HasMaxLength(200);

            // Menjalankan memanggil `entity.HasOne<EventDb>() .WithMany() .HasForeignKey(e => e.EventPk) .OnDelete(DeleteBehavior.Restrict) .HasConstraintName`
            // dengan `”fk_event_cashflow_projections_event_pk”` dalam OnModelCreating.
            entity.HasOne<EventDb>()
                // Meneruskan fungsi lambda `entity => { entity.ToTable(”event_cashflow_projections”); entity.HasKey(e =>
                // e.ProjectionId).HasName(”pk_event_cashflow_projections”); entity.Property(e => e.ProjectionId).Has...` yang dijalankan oleh operasi pemanggil
                // untuk memproses setiap masukan sebagai argumen ke `modelBuilder.Entity<CashflowProjectionDb>`.
                .WithMany()
                // Meneruskan fungsi lambda `e => e.EventPk` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `entity.HasOne<EventDb>() .WithMany() .HasForeignKey`.
                .HasForeignKey(e => e.EventPk)
                // Meneruskan `DeleteBehavior.Restrict` (nilai restrict) sebagai argumen ke `entity.HasOne<EventDb>() .WithMany() .HasForeignKey(e => e.EventPk)
                // .OnDelete`.
                .OnDelete(DeleteBehavior.Restrict)
                // Meneruskan nilai literal `”fk_event_cashflow_projections_event_pk”` sebagai argumen ke `entity.HasOne<EventDb>() .WithMany() .HasForeignKey(e =>
                // e.EventPk) .OnDelete(DeleteBehavior.Restrict) .HasConstraintName`.
                .HasConstraintName("fk_event_cashflow_projections_event_pk");

            // Menjalankan memanggil `entity.HasIndex(e => new { e.SessionId, e.EventId, e.ProjectionOrder }).IsUnique().HasDatabaseName` dengan
            // `”uq_event_cashflow_projections_session_event_order”` dalam OnModelCreating.
            entity.HasIndex(e => new { e.SessionId, e.EventId, e.ProjectionOrder }).IsUnique().HasDatabaseName("uq_event_cashflow_projections_session_event_order");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.SessionId, e.Timestamp }).HasDatabaseName` dengan `”ix_ecp_session_time”` dalam
            // OnModelCreating.
            entity.HasIndex(e => new { e.SessionId, e.Timestamp }).HasDatabaseName("ix_ecp_session_time");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.SessionId, e.UserId, e.Timestamp }).HasDatabaseName` dengan `”ix_ecp_session_user_time”`
            // dalam OnModelCreating.
            entity.HasIndex(e => new { e.SessionId, e.UserId, e.Timestamp }).HasDatabaseName("ix_ecp_session_user_time");
            // Menjalankan memanggil `entity.HasIndex(e => e.Category).HasDatabaseName` dengan `”ix_ecp_category”` dalam OnModelCreating.
            entity.HasIndex(e => e.Category).HasDatabaseName("ix_ecp_category");
            // Menjalankan memanggil `entity.HasIndex(e => e.EventPk).HasDatabaseName` dengan `”ix_event_cashflow_projections_event_pk”` dalam OnModelCreating.
            entity.HasIndex(e => e.EventPk).HasDatabaseName("ix_event_cashflow_projections_event_pk");
        // Menutup scope fungsi lambda yang dipasok ke `modelBuilder.Entity<CashflowProjectionDb>`; bagian berikut berada di luar batas blok tersebut dalam
        // OnModelCreating.
        });

        // Menjalankan memanggil `modelBuilder.Entity<MetricSnapshotDb>` dengan `entity => { entity.ToTable(”metric_snapshots”); entity.HasKey(e =>
        // e.MetricSnapshotId).HasName(”pk_metric_snapshots”); entity.Property(e => e.MetricSnapshotId).HasColumnName(”...` dalam OnModelCreating.
        modelBuilder.Entity<MetricSnapshotDb>(entity =>
        // Membuka scope fungsi lambda yang dipasok ke `modelBuilder.Entity<MetricSnapshotDb>`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam OnModelCreating.
        {
            // Menjalankan memanggil `entity.ToTable` dengan `”metric_snapshots”` dalam OnModelCreating.
            entity.ToTable("metric_snapshots");
            // Menjalankan memanggil `entity.HasKey(e => e.MetricSnapshotId).HasName` dengan `”pk_metric_snapshots”` dalam OnModelCreating.
            entity.HasKey(e => e.MetricSnapshotId).HasName("pk_metric_snapshots");
            // Menjalankan memanggil `entity.Property(e => e.MetricSnapshotId).HasColumnName` dengan `”metric_snapshot_id”` dalam OnModelCreating.
            entity.Property(e => e.MetricSnapshotId).HasColumnName("metric_snapshot_id");
            // Menjalankan memanggil `entity.Property(e => e.SessionId).HasColumnName` dengan `”session_id”` dalam OnModelCreating.
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            // Menjalankan memanggil `entity.Property(e => e.UserId).HasColumnName` dengan `”user_id”` dalam OnModelCreating.
            entity.Property(e => e.UserId).HasColumnName("user_id");
            // Menjalankan memanggil `entity.Property(e => e.SessionPlayerId).HasColumnName` dengan `”session_player_id”` dalam OnModelCreating.
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            // Menjalankan memanggil `entity.Property(e => e.ComputedAt).HasColumnName(”computed_at”).HasDefaultValueSql` dengan `”now()”` dalam
            // OnModelCreating.
            entity.Property(e => e.ComputedAt).HasColumnName("computed_at").HasDefaultValueSql("now()");
            // Menjalankan memanggil `entity.Property(e => e.MetricName).HasColumnName(”metric_name”).HasMaxLength` dengan `120` dalam OnModelCreating.
            entity.Property(e => e.MetricName).HasColumnName("metric_name").HasMaxLength(120);
            // Menjalankan memanggil `entity.Property(e => e.MetricValueNumeric).HasColumnName` dengan `”metric_value_numeric”` dalam OnModelCreating.
            entity.Property(e => e.MetricValueNumeric).HasColumnName("metric_value_numeric");
            // Menjalankan memanggil `entity.Property<string?>(”MetricValueText”).HasColumnName` dengan `”metric_value_text”` dalam OnModelCreating.
            entity.Property<string?>("MetricValueText").HasColumnName("metric_value_text");
            // Menjalankan memanggil `entity.Property<bool?>(”MetricValueBoolean”).HasColumnName` dengan `”metric_value_boolean”` dalam OnModelCreating.
            entity.Property<bool?>("MetricValueBoolean").HasColumnName("metric_value_boolean");
            // Menjalankan memanggil `entity.Property(e => e.MetricValueJson).HasColumnName(”metric_payload_json”).HasColumnType` dengan `”jsonb”` dalam
            // OnModelCreating.
            entity.Property(e => e.MetricValueJson).HasColumnName("metric_payload_json").HasColumnType("jsonb");
            // Menjalankan memanggil `entity.Property(e => e.RulesetVersionId).HasColumnName` dengan `”ruleset_version_id”` dalam OnModelCreating.
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            // Menjalankan memanggil `entity.Property(e => e.LastEventId).HasColumnName` dengan `”last_event_id”` dalam OnModelCreating.
            entity.Property(e => e.LastEventId).HasColumnName("last_event_id");

            // Menjalankan memanggil `entity.HasIndex(e => new { e.SessionId, e.MetricName, e.ComputedAt }).HasDatabaseName` dengan
            // `”ix_metrics_session_name_time”` dalam OnModelCreating.
            entity.HasIndex(e => new { e.SessionId, e.MetricName, e.ComputedAt }).HasDatabaseName("ix_metrics_session_name_time");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.SessionId, e.UserId, e.MetricName, e.ComputedAt }).HasDatabaseName` dengan
            // `”ix_metrics_session_user_name_time”` dalam OnModelCreating.
            entity.HasIndex(e => new { e.SessionId, e.UserId, e.MetricName, e.ComputedAt }).HasDatabaseName("ix_metrics_session_user_name_time");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.SessionId, e.SessionPlayerId, e.ComputedAt }).HasDatabaseName` dengan
            // `”ix_metric_snapshots_session_player_time”` dalam OnModelCreating.
            entity.HasIndex(e => new { e.SessionId, e.SessionPlayerId, e.ComputedAt }).HasDatabaseName("ix_metric_snapshots_session_player_time");
            // Menjalankan memanggil `entity.HasIndex(e => e.RulesetVersionId).HasDatabaseName` dengan `”ix_metrics_ruleset_version”` dalam OnModelCreating.
            entity.HasIndex(e => e.RulesetVersionId).HasDatabaseName("ix_metrics_ruleset_version");
            // Menjalankan memanggil `entity.HasIndex(e => e.ComputedAt).HasDatabaseName` dengan `”ix_metric_snapshots_computed_at”` dalam OnModelCreating.
            entity.HasIndex(e => e.ComputedAt).HasDatabaseName("ix_metric_snapshots_computed_at");
        // Menutup scope fungsi lambda yang dipasok ke `modelBuilder.Entity<MetricSnapshotDb>`; bagian berikut berada di luar batas blok tersebut dalam
        // OnModelCreating.
        });

        // Menjalankan memanggil `modelBuilder.Entity<ValidationLogEf>` dengan `entity => { entity.ToTable(”validation_logs”); entity.HasKey(e =>
        // e.ValidationLogId).HasName(”pk_validation_logs”); entity.Property(e => e.ValidationLogId).HasColumnName(”vali...` dalam OnModelCreating.
        modelBuilder.Entity<ValidationLogEf>(entity =>
        // Membuka scope fungsi lambda yang dipasok ke `modelBuilder.Entity<ValidationLogEf>`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam OnModelCreating.
        {
            // Menjalankan memanggil `entity.ToTable` dengan `”validation_logs”` dalam OnModelCreating.
            entity.ToTable("validation_logs");
            // Menjalankan memanggil `entity.HasKey(e => e.ValidationLogId).HasName` dengan `”pk_validation_logs”` dalam OnModelCreating.
            entity.HasKey(e => e.ValidationLogId).HasName("pk_validation_logs");
            // Menjalankan memanggil `entity.Property(e => e.ValidationLogId).HasColumnName` dengan `”validation_log_id”` dalam OnModelCreating.
            entity.Property(e => e.ValidationLogId).HasColumnName("validation_log_id");
            // Menjalankan memanggil `entity.Property(e => e.SessionId).HasColumnName` dengan `”session_id”` dalam OnModelCreating.
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            // Menjalankan memanggil `entity.Property(e => e.RulesetVersionId).HasColumnName` dengan `”ruleset_version_id”` dalam OnModelCreating.
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            // Menjalankan memanggil `entity.Property(e => e.EventId).HasColumnName` dengan `”event_id”` dalam OnModelCreating.
            entity.Property(e => e.EventId).HasColumnName("event_id");
            // Menjalankan memanggil `entity.Property(e => e.RawPayloadJson).HasColumnName(”raw_payload_json”).HasColumnType` dengan `”jsonb”` dalam
            // OnModelCreating.
            entity.Property(e => e.RawPayloadJson).HasColumnName("raw_payload_json").HasColumnType("jsonb");
            // Menjalankan memanggil `entity.Property(e => e.ErrorCode).HasColumnName(”error_code”).HasMaxLength` dengan `40` dalam OnModelCreating.
            entity.Property(e => e.ErrorCode).HasColumnName("error_code").HasMaxLength(40);
            // Menjalankan memanggil `entity.Property(e => e.ErrorMessage).HasColumnName(”error_message”).HasMaxLength` dengan `240` dalam OnModelCreating.
            entity.Property(e => e.ErrorMessage).HasColumnName("error_message").HasMaxLength(240);
            // Menjalankan memanggil `entity.Property(e => e.StatusCode).HasColumnName(”status_code”).HasDefaultValue` dengan `422` dalam OnModelCreating.
            entity.Property(e => e.StatusCode).HasColumnName("status_code").HasDefaultValue(422);
            // Menjalankan memanggil `entity.Property(e => e.TraceId).HasColumnName(”trace_id”).HasMaxLength(64).HasDefaultValue` dengan `”legacy”` dalam
            // OnModelCreating.
            entity.Property(e => e.TraceId).HasColumnName("trace_id").HasMaxLength(64).HasDefaultValue("legacy");
            // Menjalankan memanggil `entity.Property(e => e.DetailsJson).HasColumnName(”details_json”).HasColumnType` dengan `”jsonb”` dalam OnModelCreating.
            entity.Property(e => e.DetailsJson).HasColumnName("details_json").HasColumnType("jsonb");
            // Menjalankan memanggil `entity.Property(e => e.CreatedAt).HasColumnName(”created_at”).HasDefaultValueSql` dengan `”now()”` dalam OnModelCreating.
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            // Menjalankan memanggil `entity.HasOne<SessionDb>() .WithMany() .HasForeignKey(e => e.SessionId) .OnDelete(DeleteBehavior.Cascade)
            // .HasConstraintName` dengan `”fk_validation_logs_session_id”` dalam OnModelCreating.
            entity.HasOne<SessionDb>()
                // Meneruskan fungsi lambda `entity => { entity.ToTable(”validation_logs”); entity.HasKey(e => e.ValidationLogId).HasName(”pk_validation_logs”);
                // entity.Property(e => e.ValidationLogId).HasColumnName(”vali...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                // argumen ke `modelBuilder.Entity<ValidationLogEf>`.
                .WithMany()
                // Meneruskan fungsi lambda `e => e.SessionId` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `entity.HasOne<SessionDb>() .WithMany() .HasForeignKey`.
                .HasForeignKey(e => e.SessionId)
                // Meneruskan `DeleteBehavior.Cascade` (nilai cascade) sebagai argumen ke `entity.HasOne<SessionDb>() .WithMany() .HasForeignKey(e => e.SessionId)
                // .OnDelete`.
                .OnDelete(DeleteBehavior.Cascade)
                // Meneruskan nilai literal `”fk_validation_logs_session_id”` sebagai argumen ke `entity.HasOne<SessionDb>() .WithMany() .HasForeignKey(e =>
                // e.SessionId) .OnDelete(DeleteBehavior.Cascade) .HasConstraintName`.
                .HasConstraintName("fk_validation_logs_session_id");

            // Menjalankan memanggil `entity.HasOne<RulesetVersionDb>() .WithMany() .HasForeignKey(e => e.RulesetVersionId) .OnDelete(DeleteBehavior.SetNull)
            // .HasConstraintName` dengan `”fk_validation_logs_ruleset_version_id”` dalam OnModelCreating.
            entity.HasOne<RulesetVersionDb>()
                // Meneruskan fungsi lambda `entity => { entity.ToTable(”validation_logs”); entity.HasKey(e => e.ValidationLogId).HasName(”pk_validation_logs”);
                // entity.Property(e => e.ValidationLogId).HasColumnName(”vali...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                // argumen ke `modelBuilder.Entity<ValidationLogEf>`.
                .WithMany()
                // Meneruskan fungsi lambda `e => e.RulesetVersionId` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `entity.HasOne<RulesetVersionDb>() .WithMany() .HasForeignKey`.
                .HasForeignKey(e => e.RulesetVersionId)
                // Meneruskan `DeleteBehavior.SetNull` (nilai set null) sebagai argumen ke `entity.HasOne<RulesetVersionDb>() .WithMany() .HasForeignKey(e =>
                // e.RulesetVersionId) .OnDelete`.
                .OnDelete(DeleteBehavior.SetNull)
                // Meneruskan nilai literal `”fk_validation_logs_ruleset_version_id”` sebagai argumen ke `entity.HasOne<RulesetVersionDb>() .WithMany()
                // .HasForeignKey(e => e.RulesetVersionId) .OnDelete(DeleteBehavior.SetNull) .HasConstraintName`.
                .HasConstraintName("fk_validation_logs_ruleset_version_id");

            // Menjalankan memanggil `entity.HasIndex(e => new { e.SessionId, e.EventId }).IsUnique().HasDatabaseName` dengan
            // `”uq_validation_logs_session_event”` dalam OnModelCreating.
            entity.HasIndex(e => new { e.SessionId, e.EventId }).IsUnique().HasDatabaseName("uq_validation_logs_session_event");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.SessionId, e.CreatedAt }).HasDatabaseName` dengan `”ix_validation_session_time”` dalam
            // OnModelCreating.
            entity.HasIndex(e => new { e.SessionId, e.CreatedAt }).HasDatabaseName("ix_validation_session_time");
            // Menjalankan memanggil `entity.HasIndex(e => e.ErrorCode).HasDatabaseName` dengan `”ix_validation_logs_error_code”` dalam OnModelCreating.
            entity.HasIndex(e => e.ErrorCode).HasDatabaseName("ix_validation_logs_error_code");
            // Menjalankan memanggil `entity.HasIndex(e => e.CreatedAt).HasDatabaseName` dengan `”ix_validation_logs_created_at”` dalam OnModelCreating.
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("ix_validation_logs_created_at");
        // Menutup scope fungsi lambda yang dipasok ke `modelBuilder.Entity<ValidationLogEf>`; bagian berikut berada di luar batas blok tersebut dalam
        // OnModelCreating.
        });

        // Menjalankan memanggil `modelBuilder.Entity<SecurityAuditLogDb>` dengan `entity => { entity.ToTable(”security_audit_logs”); entity.HasKey(e =>
        // e.SecurityAuditLogId).HasName(”pk_security_audit_logs”); entity.Property(e => e.SecurityAuditLogId).HasCo...` dalam OnModelCreating.
        modelBuilder.Entity<SecurityAuditLogDb>(entity =>
        // Membuka scope fungsi lambda yang dipasok ke `modelBuilder.Entity<SecurityAuditLogDb>`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam OnModelCreating.
        {
            // Menjalankan memanggil `entity.ToTable` dengan `”security_audit_logs”` dalam OnModelCreating.
            entity.ToTable("security_audit_logs");
            // Menjalankan memanggil `entity.HasKey(e => e.SecurityAuditLogId).HasName` dengan `”pk_security_audit_logs”` dalam OnModelCreating.
            entity.HasKey(e => e.SecurityAuditLogId).HasName("pk_security_audit_logs");
            // Menjalankan memanggil `entity.Property(e => e.SecurityAuditLogId).HasColumnName` dengan `”security_audit_log_id”` dalam OnModelCreating.
            entity.Property(e => e.SecurityAuditLogId).HasColumnName("security_audit_log_id");
            // Menjalankan memanggil `entity.Property(e => e.OccurredAt).HasColumnName(”occurred_at”).HasDefaultValueSql` dengan `”now()”` dalam
            // OnModelCreating.
            entity.Property(e => e.OccurredAt).HasColumnName("occurred_at").HasDefaultValueSql("now()");
            // Menjalankan memanggil `entity.Property(e => e.TraceId).HasColumnName(”trace_id”).HasMaxLength` dengan `64` dalam OnModelCreating.
            entity.Property(e => e.TraceId).HasColumnName("trace_id").HasMaxLength(64);
            // Menjalankan memanggil `entity.Property(e => e.EventType).HasColumnName(”event_type”).HasMaxLength` dengan `80` dalam OnModelCreating.
            entity.Property(e => e.EventType).HasColumnName("event_type").HasMaxLength(80);
            // Menjalankan memanggil `entity.Property(e => e.Outcome).HasColumnName(”outcome”).HasMaxLength` dengan `40` dalam OnModelCreating.
            entity.Property(e => e.Outcome).HasColumnName("outcome").HasMaxLength(40);
            // Menjalankan memanggil `entity.Property(e => e.UserId).HasColumnName` dengan `”user_id”` dalam OnModelCreating.
            entity.Property(e => e.UserId).HasColumnName("user_id");
            // Menjalankan memanggil `entity.Property(e => e.Username).HasColumnName(”username”).HasMaxLength` dengan `80` dalam OnModelCreating.
            entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(80);
            // Menjalankan memanggil `entity.Property(e => e.Role).HasColumnName(”role”).HasMaxLength` dengan `20` dalam OnModelCreating.
            entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(20);
            // Menjalankan memanggil `entity.Property(e => e.IpAddress).HasColumnName(”ip_address”).HasMaxLength` dengan `80` dalam OnModelCreating.
            entity.Property(e => e.IpAddress).HasColumnName("ip_address").HasMaxLength(80);
            // Menjalankan memanggil `entity.Property(e => e.UserAgent).HasColumnName(”user_agent”).HasMaxLength` dengan `300` dalam OnModelCreating.
            entity.Property(e => e.UserAgent).HasColumnName("user_agent").HasMaxLength(300);
            // Menjalankan memanggil `entity.Property(e => e.Method).HasColumnName(”method”).HasMaxLength` dengan `16` dalam OnModelCreating.
            entity.Property(e => e.Method).HasColumnName("method").HasMaxLength(16);
            // Menjalankan memanggil `entity.Property(e => e.Path).HasColumnName(”path”).HasMaxLength` dengan `240` dalam OnModelCreating.
            entity.Property(e => e.Path).HasColumnName("path").HasMaxLength(240);
            // Menjalankan memanggil `entity.Property(e => e.StatusCode).HasColumnName` dengan `”status_code”` dalam OnModelCreating.
            entity.Property(e => e.StatusCode).HasColumnName("status_code");
            // Menjalankan memanggil `entity.Property(e => e.DetailJson).HasColumnName(”detail_json”).HasColumnType` dengan `”jsonb”` dalam OnModelCreating.
            entity.Property(e => e.DetailJson).HasColumnName("detail_json").HasColumnType("jsonb");

            // Menjalankan memanggil `entity.HasIndex(e => e.OccurredAt).HasDatabaseName` dengan `”ix_security_audit_logs_occurred”` dalam OnModelCreating.
            entity.HasIndex(e => e.OccurredAt).HasDatabaseName("ix_security_audit_logs_occurred");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.EventType, e.OccurredAt }).HasDatabaseName` dengan `”ix_security_audit_logs_event”` dalam
            // OnModelCreating.
            entity.HasIndex(e => new { e.EventType, e.OccurredAt }).HasDatabaseName("ix_security_audit_logs_event");
            // Menjalankan memanggil `entity.HasIndex(e => new { e.UserId, e.OccurredAt }).HasDatabaseName` dengan `”ix_security_audit_logs_user”` dalam
            // OnModelCreating.
            entity.HasIndex(e => new { e.UserId, e.OccurredAt }).HasDatabaseName("ix_security_audit_logs_user");
        // Menutup scope fungsi lambda yang dipasok ke `modelBuilder.Entity<SecurityAuditLogDb>`; bagian berikut berada di luar batas blok tersebut dalam
        // OnModelCreating.
        });
    // Menutup scope metode OnModelCreating; bagian berikut berada di luar batas blok tersebut dalam OnModelCreating.
    }
// Menutup scope tipe AppDbContext; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `AppUserEf`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AppUserEf
// Membuka scope tipe AppUserEf; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `UserId` bertipe `Guid` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai.
    public Guid UserId { get; set; }
    // Mendefinisikan properti `Username` bertipe `string` untuk nama akun yang dipakai saat autentikasi; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Username { get; set; } = string.Empty;
    // Mendefinisikan properti `DisplayName` bertipe `string` untuk nilai display nama; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string DisplayName { get; set; } = string.Empty;
    // Mendefinisikan properti `PasswordHash` bertipe `string` untuk hasil hash kata sandi untuk penyimpanan tanpa menyimpan teks kata sandi asli; get
    // menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string PasswordHash { get; set; } = string.Empty;
    // Mendefinisikan properti `Role` bertipe `string` untuk peran pengguna yang menentukan hak akses; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Role { get; set; } = string.Empty;
    // Mendefinisikan properti `IsActive` bertipe `bool` untuk nilai berstatus aktif; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public bool IsActive { get; set; }
    // Mendefinisikan properti `CreatedAt` bertipe `DateTimeOffset` untuk nilai created at; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public DateTimeOffset CreatedAt { get; set; }
// Menutup scope tipe AppUserEf; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `SessionParticipantEf`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionParticipantEf
// Membuka scope tipe SessionParticipantEf; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `SessionParticipantId` bertipe `Guid` untuk identitas keikutsertaan pemain pada sesi tertentu; get menyediakan pembacaan
    // nilai, set mengizinkan penggantian nilai.
    public Guid SessionParticipantId { get; set; }
    // Mendefinisikan properti `SessionId` bertipe `Guid` untuk identitas unik sesi permainan yang menjadi batas data operasi ini; get menyediakan
    // pembacaan nilai, set mengizinkan penggantian nilai.
    public Guid SessionId { get; set; }
    // Mendefinisikan properti `UserId` bertipe `Guid` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai.
    public Guid UserId { get; set; }
    // Mendefinisikan properti `PlayerOrderNo` bertipe `int` untuk nilai pemain urutan/pesanan no; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public int PlayerOrderNo { get; set; }
    // Mendefinisikan properti `PlayerName` bertipe `string?` untuk nilai pemain nama; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; tanda ? mengizinkan nilai null.
    public string? PlayerName { get; set; }
    // Mendefinisikan properti `JoinedAt` bertipe `DateTimeOffset` untuk nilai joined at; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public DateTimeOffset JoinedAt { get; set; }
// Menutup scope tipe SessionParticipantEf; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `SessionStateEf`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionStateEf
// Membuka scope tipe SessionStateEf; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `SessionId` bertipe `Guid` untuk identitas unik sesi permainan yang menjadi batas data operasi ini; get menyediakan
    // pembacaan nilai, set mengizinkan penggantian nilai.
    public Guid SessionId { get; set; }
    // Mendefinisikan properti `Day` bertipe `int` untuk nomor hari permainan yang menjadi konteks aktivitas; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai.
    public int Day { get; set; }
    // Mendefinisikan properti `Weekday` bertipe `string` untuk nilai weekday; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai
    // awalnya nilai literal `”MON”`.
    public string Weekday { get; set; } = "MON";
    // Mendefinisikan properti `TurnNumber` bertipe `int` untuk nilai giliran number; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public int TurnNumber { get; set; }
    // Mendefinisikan properti `ActionSlot` bertipe `int` untuk nilai aksi slot; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
    public int ActionSlot { get; set; }
    // Mendefinisikan properti `CurrentSessionPlayerId` bertipe `Guid?` untuk nilai saat ini sesi pemain identitas; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public Guid? CurrentSessionPlayerId { get; set; }
    // Mendefinisikan properti `CurrentActionSlot` bertipe `int` untuk nilai saat ini aksi slot; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public int CurrentActionSlot { get; set; }
    // Mendefinisikan properti `ActionSlotsLeft` bertipe `int` untuk nilai aksi slots left; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public int ActionSlotsLeft { get; set; }
    // Mendefinisikan properti `FinishDay` bertipe `int` untuk nilai finish hari; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
    public int FinishDay { get; set; }
    // Mendefinisikan properti `Phase` bertipe `string` untuk nilai phase; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai
    // awalnya nilai literal `”PLAYER_TURN”`.
    public string Phase { get; set; } = "PLAYER_TURN";
    // Mendefinisikan properti `IsGameOver` bertipe `bool` untuk nilai berstatus game over; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public bool IsGameOver { get; set; }
    // Mendefinisikan properti `StateVersion` bertipe `long` untuk nilai keadaan versi; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public long StateVersion { get; set; }
    // Mendefinisikan properti `LastEventId` bertipe `Guid?` untuk nilai last event identitas; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public Guid? LastEventId { get; set; }
    // Mendefinisikan properti `UiStateJson` bertipe `string` untuk nilai ui keadaan JSON; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; nilai awalnya nilai literal `”{}”`.
    public string UiStateJson { get; set; } = "{}";
    // Mendefinisikan properti `CreatedAt` bertipe `DateTimeOffset` untuk nilai created at; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public DateTimeOffset CreatedAt { get; set; }
    // Mendefinisikan properti `UpdatedAt` bertipe `DateTimeOffset` untuk nilai updated at; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public DateTimeOffset UpdatedAt { get; set; }
// Menutup scope tipe SessionStateEf; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `ValidationLogEf`; sealed mencegah tipe ini diturunkan lagi.
public sealed class ValidationLogEf
// Membuka scope tipe ValidationLogEf; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `ValidationLogId` bertipe `Guid` untuk nilai validasi log identitas; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public Guid ValidationLogId { get; set; }
    // Mendefinisikan properti `SessionId` bertipe `Guid` untuk identitas unik sesi permainan yang menjadi batas data operasi ini; get menyediakan
    // pembacaan nilai, set mengizinkan penggantian nilai.
    public Guid SessionId { get; set; }
    // Mendefinisikan properti `RulesetVersionId` bertipe `Guid?` untuk identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang
    // tepat; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public Guid? RulesetVersionId { get; set; }
    // Mendefinisikan properti `EventId` bertipe `Guid` untuk identitas unik event untuk pencatatan dan pemeriksaan duplikasi; get menyediakan pembacaan
    // nilai, set mengizinkan penggantian nilai.
    public Guid EventId { get; set; }
    // Mendefinisikan properti `RawPayloadJson` bertipe `string` untuk nilai raw payload JSON; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; nilai awalnya nilai literal `”{}”`.
    public string RawPayloadJson { get; set; } = "{}";
    // Mendefinisikan properti `ErrorCode` bertipe `string?` untuk nilai kesalahan kode; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; tanda ? mengizinkan nilai null.
    public string? ErrorCode { get; set; }
    // Mendefinisikan properti `ErrorMessage` bertipe `string?` untuk nilai kesalahan pesan; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public string? ErrorMessage { get; set; }
    // Mendefinisikan properti `StatusCode` bertipe `int` untuk kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan; get
    // menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
    public int StatusCode { get; set; }
    // Mendefinisikan properti `TraceId` bertipe `string` untuk identitas penelusuran yang menghubungkan respons, log, dan permintaan yang sama; get
    // menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai awalnya nilai literal `”legacy”`.
    public string TraceId { get; set; } = "legacy";
    // Mendefinisikan properti `DetailsJson` bertipe `string?` untuk nilai rincian JSON; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; tanda ? mengizinkan nilai null.
    public string? DetailsJson { get; set; }
    // Mendefinisikan properti `CreatedAt` bertipe `DateTimeOffset` untuk nilai created at; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public DateTimeOffset CreatedAt { get; set; }
// Menutup scope tipe ValidationLogEf; bagian berikut berada di luar batas blok tersebut.
}
