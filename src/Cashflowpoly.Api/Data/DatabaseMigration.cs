// Fungsi file: Mendefinisikan unit migrasi database bernama untuk eksekusi berurutan.
namespace Cashflowpoly.Api.Data;

/// <summary>
/// Satu migrasi database yang memiliki ID stabil dan SQL idempotent.
/// </summary>
public sealed record DatabaseMigration(string Id, string Sql);
