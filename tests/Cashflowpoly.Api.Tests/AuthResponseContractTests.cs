// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AuthResponseContractTests.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

/// <summary>
/// Kelas pengujian unit untuk kontrak response login dan register.
/// </summary>
// Mendefinisikan tipe class `AuthResponseContractTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AuthResponseContractTests
// Membuka scope tipe AuthResponseContractTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi respons login menyertakan display_name agar client tidak perlu lookup profil pemain.
    /// </summary>
    // Mendefinisikan metode `LoginResponse_SerializesDisplayName` dengan hasil bertipe `void`; operasi ini menangani login respons serializes display
    // nama.
    public void LoginResponse_SerializesDisplayName()
    // Membuka scope metode LoginResponse_SerializesDisplayName; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // LoginResponse_SerializesDisplayName.
    {
        // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan objek baru bertipe `LoginResponse`
        // dengan argumen ( Guid.Parse(”aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa”), ”player_one”, ”PLAYER”, ”Player One”, ”token”,
        // DateTimeOffset.Parse(”2026-01-01T00:00:00Z”)). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var response = new LoginResponse(
            // Meneruskan memanggil `Guid.Parse` dengan `”aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa”` sebagai argumen ke konstruktor `LoginResponse`; Meneruskan
            // nilai literal `”aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa”` sebagai argumen ke `Guid.Parse`.
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            // Meneruskan nilai literal `”player_one”` sebagai argumen ke konstruktor `LoginResponse`.
            "player_one",
            // Meneruskan nilai literal `”PLAYER”` sebagai argumen ke konstruktor `LoginResponse`.
            "PLAYER",
            // Meneruskan nilai literal `”Player One”` sebagai argumen ke konstruktor `LoginResponse`.
            "Player One",
            // Meneruskan nilai literal `”token”` sebagai argumen ke konstruktor `LoginResponse`.
            "token",
            // Meneruskan memanggil `DateTimeOffset.Parse` dengan `”2026-01-01T00:00:00Z”` sebagai argumen ke konstruktor `LoginResponse`; Meneruskan nilai
            // literal `”2026-01-01T00:00:00Z”` sebagai argumen ke `DateTimeOffset.Parse`.
            DateTimeOffset.Parse("2026-01-01T00:00:00Z"));

        // Menyiapkan variabel lokal `json` untuk nilai JSON dengan menserialisasi `response` menjadi JSON melalui `JsonSerializer.Serialize`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var json = JsonSerializer.Serialize(response);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””display_name””:””Player One”””`,
        // `json` dalam LoginResponse_SerializesDisplayName.
        Assert.Contains(@"""display_name"":""Player One""", json);
    // Menutup scope metode LoginResponse_SerializesDisplayName; bagian berikut berada di luar batas blok tersebut dalam
    // LoginResponse_SerializesDisplayName.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi respons register menyertakan display_name agar UI dapat mengisi sesi dari respons auth.
    /// </summary>
    // Mendefinisikan metode `RegisterResponse_SerializesDisplayName` dengan hasil bertipe `void`; operasi ini menangani register respons serializes
    // display nama.
    public void RegisterResponse_SerializesDisplayName()
    // Membuka scope metode RegisterResponse_SerializesDisplayName; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // RegisterResponse_SerializesDisplayName.
    {
        // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan objek baru bertipe
        // `RegisterResponse` dengan argumen ( Guid.Parse(”bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb”), ”player_two”, ”PLAYER”, ”Player Two”, ”token”,
        // DateTimeOffset.Parse(”2026-01-01T00:00:00Z”)). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var response = new RegisterResponse(
            // Meneruskan memanggil `Guid.Parse` dengan `”bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb”` sebagai argumen ke konstruktor `RegisterResponse`; Meneruskan
            // nilai literal `”bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb”` sebagai argumen ke `Guid.Parse`.
            Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            // Meneruskan nilai literal `”player_two”` sebagai argumen ke konstruktor `RegisterResponse`.
            "player_two",
            // Meneruskan nilai literal `”PLAYER”` sebagai argumen ke konstruktor `RegisterResponse`.
            "PLAYER",
            // Meneruskan nilai literal `”Player Two”` sebagai argumen ke konstruktor `RegisterResponse`.
            "Player Two",
            // Meneruskan nilai literal `”token”` sebagai argumen ke konstruktor `RegisterResponse`.
            "token",
            // Meneruskan memanggil `DateTimeOffset.Parse` dengan `”2026-01-01T00:00:00Z”` sebagai argumen ke konstruktor `RegisterResponse`; Meneruskan nilai
            // literal `”2026-01-01T00:00:00Z”` sebagai argumen ke `DateTimeOffset.Parse`.
            DateTimeOffset.Parse("2026-01-01T00:00:00Z"));

        // Menyiapkan variabel lokal `json` untuk nilai JSON dengan menserialisasi `response` menjadi JSON melalui `JsonSerializer.Serialize`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var json = JsonSerializer.Serialize(response);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””display_name””:””Player Two”””`,
        // `json` dalam RegisterResponse_SerializesDisplayName.
        Assert.Contains(@"""display_name"":""Player Two""", json);
    // Menutup scope metode RegisterResponse_SerializesDisplayName; bagian berikut berada di luar batas blok tersebut dalam
    // RegisterResponse_SerializesDisplayName.
    }
// Menutup scope tipe AuthResponseContractTests; bagian berikut berada di luar batas blok tersebut.
}
