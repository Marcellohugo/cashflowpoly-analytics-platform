// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui UiTextLexicon.
// Mengimpor namespace `System.Collections.Frozen` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Collections.Frozen;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

// Mendefinisikan tipe class `UiTextLexicon`.
internal static partial class UiTextLexicon
// Membuka scope tipe UiTextLexicon; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `Build` dengan hasil bertipe `FrozenDictionary<string, (string Id, string En)>`; operasi ini menangani build.
    internal static FrozenDictionary<string, (string Id, string En)> Build()
    // Membuka scope metode Build; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
    {
        // Menyiapkan variabel lokal `terms` untuk nilai terms dengan objek baru bertipe `Dictionary<string, (string Id, string En)>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var terms = new Dictionary<string, (string Id, string En)>(StringComparer.OrdinalIgnoreCase);
        // Menjalankan memanggil `AddCore` dengan `terms` dalam Build.
        AddCore(terms);
        // Menjalankan memanggil `AddSessions` dengan `terms` dalam Build.
        AddSessions(terms);
        // Menjalankan memanggil `AddPlayers` dengan `terms` dalam Build.
        AddPlayers(terms);
        // Menjalankan memanggil `AddRulesets` dengan `terms` dalam Build.
        AddRulesets(terms);
        // Menjalankan memanggil `AddRulebook` dengan `terms` dalam Build.
        AddRulebook(terms);
        // Mengembalikan membentuk kamus yang tidak dapat diubah dari `terms` untuk pencarian berulang kepada pemanggil dalam Build; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return terms.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    // Menutup scope metode Build; bagian berikut berada di luar batas blok tersebut dalam Build.
    }

    // Mendefinisikan metode `AddCore` dengan hasil bertipe `void`; operasi ini menangani add core. Masukan: Parameter `terms` bertipe
    // `Dictionary<string, (string Id, string En)>` membawa nilai terms.
    private static partial void AddCore(Dictionary<string, (string Id, string En)> terms);

    // Mendefinisikan metode `AddSessions` dengan hasil bertipe `void`; operasi ini menangani add sessions. Masukan: Parameter `terms` bertipe
    // `Dictionary<string, (string Id, string En)>` membawa nilai terms.
    private static partial void AddSessions(Dictionary<string, (string Id, string En)> terms);

    // Mendefinisikan metode `AddPlayers` dengan hasil bertipe `void`; operasi ini menangani add pemain. Masukan: Parameter `terms` bertipe
    // `Dictionary<string, (string Id, string En)>` membawa nilai terms.
    private static partial void AddPlayers(Dictionary<string, (string Id, string En)> terms);

    // Mendefinisikan metode `AddRulesets` dengan hasil bertipe `void`; operasi ini menangani add aturan. Masukan: Parameter `terms` bertipe
    // `Dictionary<string, (string Id, string En)>` membawa nilai terms.
    private static partial void AddRulesets(Dictionary<string, (string Id, string En)> terms);

    // Mendefinisikan metode `AddRulebook` dengan hasil bertipe `void`; operasi ini menangani add rulebook. Masukan: Parameter `terms` bertipe
    // `Dictionary<string, (string Id, string En)>` membawa nilai terms.
    private static partial void AddRulebook(Dictionary<string, (string Id, string En)> terms);
// Menutup scope tipe UiTextLexicon; bagian berikut berada di luar batas blok tersebut.
}