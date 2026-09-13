// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui SessionRules.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Konstanta aturan domain terkait batasan sesi permainan.
/// </summary>
// Mendefinisikan tipe class `SessionRules`.
public static class SessionRules
{
    /// <summary>
    /// Batas maksimum pemain per sesi.
    /// </summary>
    // Mendeklarasikan field bertipe `int`: `MaxPlayersPerSession` menyimpan nilai maksimum pemain per sesi dengan nilai awal nilai literal `4`.
    public const int MaxPlayersPerSession = 4;
}
