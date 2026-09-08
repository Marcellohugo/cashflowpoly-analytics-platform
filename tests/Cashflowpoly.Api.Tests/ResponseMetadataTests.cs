// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui ResponseMetadataTests.
// Mengimpor namespace `System.Reflection` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Reflection;
// Mengimpor namespace `Cashflowpoly.Api.Controllers` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Controllers;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc.Routing` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc.Routing;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

/// <summary>
/// Kelas pengujian unit untuk memastikan setiap action API controller
/// mendeklarasikan atribut ProducesResponseType yang lengkap untuk status sukses maupun error.
/// </summary>
// Mendefinisikan tipe class `ResponseMetadataTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class ResponseMetadataTests
// Membuka scope tipe ResponseMetadataTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa setiap action API memiliki setidaknya satu
    /// atribut ProducesResponseType dengan status code sukses (2xx).
    /// </summary>
    // Mendefinisikan metode `Every_Api_Action_Defines_Success_Response_Metadata` dengan hasil bertipe `void`; operasi ini menangani every api aksi
    // defines success respons metadata.
    public void Every_Api_Action_Defines_Success_Response_Metadata()
    // Membuka scope metode Every_Api_Action_Defines_Success_Response_Metadata; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Every_Api_Action_Defines_Success_Response_Metadata.
    {
        // Menyiapkan variabel lokal `allowedNoSuccessMetadata` untuk nilai allowed no success metadata dengan objek baru bertipe `HashSet<string>` dengan
        // argumen (StringComparer.Ordinal). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var allowedNoSuccessMetadata = new HashSet<string>(StringComparer.Ordinal)
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Every_Api_Action_Defines_Success_Response_Metadata.
        {
            // Menggunakan nilai literal `”SessionsController.SaveState”` sebagai bagian ekspresi yang sedang disusun dalam
            // Every_Api_Action_Defines_Success_Response_Metadata.
            "SessionsController.SaveState"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Every_Api_Action_Defines_Success_Response_Metadata.
        };

        // Menyiapkan variabel lokal `missing` untuk nilai missing dengan mematerialisasi urutan `GetActionResponseMetadata() .Where(item =>
        // item.StatusCodes.All(code => code < 200 || code >= 300) && !allowedNoSuccessMetadata.Contains(item.ActionDisplayName)) .Select(item ...` menjadi
        // List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var missing = GetActionResponseMetadata()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => item.StatusCodes.All(code => code < 200 || code >= 300) &&
            // !allowedNoSuccessMetadata.Contains(item.ActionDisplayName)) dalam Every_Api_Action_Defines_Success_Response_Metadata; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => item.StatusCodes.All(code => code < 200 || code >= 300) && !allowedNoSuccessMetadata.Contains(item.ActionDisplayName))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => item.ActionDisplayName) dalam
            // Every_Api_Action_Defines_Success_Response_Metadata; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(item => item.ActionDisplayName)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam Every_Api_Action_Defines_Success_Response_Metadata; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();

        // Menjalankan pemeriksaan bahwa `missing.Count == 0`, `$”Action tanpa response sukses metadata: {string.Join(”, ”, missing)}”` bernilai benar;
        // pengujian gagal jika kondisi tidak terpenuhi dalam Every_Api_Action_Defines_Success_Response_Metadata.
        Assert.True(
            // Meneruskan perbandingan kesamaan antara `missing.Count` dan `0` sebagai argumen ke `Assert.True`.
            missing.Count == 0,
            // Meneruskan teks interpolasi `$”Action tanpa response sukses metadata: {string.Join(”, ”, missing)}”`; nilai ekspresi di dalam kurung kurawal
            // disisipkan saat program berjalan sebagai argumen ke `Assert.True`; Meneruskan nilai literal `”, ”` sebagai argumen ke `string.Join`; Meneruskan
            // `missing` (nilai missing) sebagai argumen ke `string.Join`.
            $"Action tanpa response sukses metadata: {string.Join(", ", missing)}");
    // Menutup scope metode Every_Api_Action_Defines_Success_Response_Metadata; bagian berikut berada di luar batas blok tersebut dalam
    // Every_Api_Action_Defines_Success_Response_Metadata.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa setiap action API memiliki setidaknya satu
    /// atribut ProducesResponseType dengan status code error (4xx/5xx).
    /// </summary>
    // Mendefinisikan metode `Every_Api_Action_Defines_Error_Response_Metadata` dengan hasil bertipe `void`; operasi ini menangani every api aksi
    // defines kesalahan respons metadata.
    public void Every_Api_Action_Defines_Error_Response_Metadata()
    // Membuka scope metode Every_Api_Action_Defines_Error_Response_Metadata; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Every_Api_Action_Defines_Error_Response_Metadata.
    {
        // Menyiapkan variabel lokal `missing` untuk nilai missing dengan mematerialisasi urutan `GetActionResponseMetadata() .Where(item =>
        // item.StatusCodes.All(code => code < 400)) .Select(item => item.ActionDisplayName)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam
        // memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var missing = GetActionResponseMetadata()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => item.StatusCodes.All(code => code < 400)) dalam
            // Every_Api_Action_Defines_Error_Response_Metadata; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => item.StatusCodes.All(code => code < 400))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => item.ActionDisplayName) dalam
            // Every_Api_Action_Defines_Error_Response_Metadata; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(item => item.ActionDisplayName)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam Every_Api_Action_Defines_Error_Response_Metadata; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();

        // Menjalankan pemeriksaan bahwa `missing.Count == 0`, `$”Action tanpa response error metadata: {string.Join(”, ”, missing)}”` bernilai benar;
        // pengujian gagal jika kondisi tidak terpenuhi dalam Every_Api_Action_Defines_Error_Response_Metadata.
        Assert.True(
            // Meneruskan perbandingan kesamaan antara `missing.Count` dan `0` sebagai argumen ke `Assert.True`.
            missing.Count == 0,
            // Meneruskan teks interpolasi `$”Action tanpa response error metadata: {string.Join(”, ”, missing)}”`; nilai ekspresi di dalam kurung kurawal
            // disisipkan saat program berjalan sebagai argumen ke `Assert.True`; Meneruskan nilai literal `”, ”` sebagai argumen ke `string.Join`; Meneruskan
            // `missing` (nilai missing) sebagai argumen ke `string.Join`.
            $"Action tanpa response error metadata: {string.Join(", ", missing)}");
    // Menutup scope metode Every_Api_Action_Defines_Error_Response_Metadata; bagian berikut berada di luar batas blok tersebut dalam
    // Every_Api_Action_Defines_Error_Response_Metadata.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa tidak ada action API yang hanya mendeklarasikan
    /// status 200 OK saja tanpa status code lain yang lebih spesifik.
    /// </summary>
    // Mendefinisikan metode `Every_Api_Action_Exposes_More_Than_200_Status` dengan hasil bertipe `void`; operasi ini menangani every api aksi exposes
    // more than 200 status.
    public void Every_Api_Action_Exposes_More_Than_200_Status()
    // Membuka scope metode Every_Api_Action_Exposes_More_Than_200_Status; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Every_Api_Action_Exposes_More_Than_200_Status.
    {
        // Menyiapkan variabel lokal `missing` untuk nilai missing dengan mematerialisasi urutan `GetActionResponseMetadata() .Where(item =>
        // item.StatusCodes.Count == 1 && item.StatusCodes.Contains(StatusCodes.Status200OK)) .Select(item => item.ActionDisplayName)` menjadi List;
        // enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var missing = GetActionResponseMetadata()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => item.StatusCodes.Count == 1 &&
            // item.StatusCodes.Contains(StatusCodes.Status200OK)) dalam Every_Api_Action_Exposes_More_Than_200_Status; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .Where(item => item.StatusCodes.Count == 1 && item.StatusCodes.Contains(StatusCodes.Status200OK))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => item.ActionDisplayName) dalam
            // Every_Api_Action_Exposes_More_Than_200_Status; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(item => item.ActionDisplayName)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam Every_Api_Action_Exposes_More_Than_200_Status; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();

        // Menjalankan pemeriksaan bahwa `missing.Count == 0`, `$”Action masih hanya 200: {string.Join(”, ”, missing)}”` bernilai benar; pengujian gagal
        // jika kondisi tidak terpenuhi dalam Every_Api_Action_Exposes_More_Than_200_Status.
        Assert.True(
            // Meneruskan perbandingan kesamaan antara `missing.Count` dan `0` sebagai argumen ke `Assert.True`.
            missing.Count == 0,
            // Meneruskan teks interpolasi `$”Action masih hanya 200: {string.Join(”, ”, missing)}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `Assert.True`; Meneruskan nilai literal `”, ”` sebagai argumen ke `string.Join`; Meneruskan `missing` (nilai
            // missing) sebagai argumen ke `string.Join`.
            $"Action masih hanya 200: {string.Join(", ", missing)}");
    // Menutup scope metode Every_Api_Action_Exposes_More_Than_200_Status; bagian berikut berada di luar batas blok tersebut dalam
    // Every_Api_Action_Exposes_More_Than_200_Status.
    }

    /// <summary>
    /// Helper yang menggunakan refleksi untuk mengumpulkan semua action pada controller API
    /// beserta daftar status code dari atribut ProducesResponseType yang dideklarasikan.
    /// </summary>
    // Mendefinisikan metode `GetActionResponseMetadata` dengan hasil bertipe `IReadOnlyList<ActionMetadata>`. Helper yang menggunakan refleksi untuk
    // mengumpulkan semua action pada controller API beserta daftar status code dari atribut ProducesResponseType yang dideklarasikan.
    private static IReadOnlyList<ActionMetadata> GetActionResponseMetadata()
    // Membuka scope metode GetActionResponseMetadata; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetActionResponseMetadata.
    {
        // Menyiapkan variabel lokal `assembly` untuk nilai assembly dengan `typeof(AuthController).Assembly` (nilai assembly). Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var assembly = typeof(AuthController).Assembly;
        // Menyiapkan variabel lokal `controllers` untuk nilai controllers dengan mengurutkan `assembly.GetTypes() .Where(type =>
        // typeof(ControllerBase).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract && string.Equals(type.Namespace, ”Cashflowpoly.Api.Control...`
        // secara menaik berdasarkan `type => type.Name`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var controllers = assembly.GetTypes()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(type => dalam GetActionResponseMetadata; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(type =>
                // Meneruskan `type` (nilai jenis) sebagai argumen ke `typeof(ControllerBase).IsAssignableFrom`.
                typeof(ControllerBase).IsAssignableFrom(type) &&
                // Meneruskan fungsi lambda `type => typeof(ControllerBase).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract &&
                // string.Equals(type.Namespace, ”Cashflowpoly.Api.Controllers”, StringComparison.Ord...` yang dijalankan oleh operasi pemanggil untuk memproses
                // setiap masukan sebagai argumen ke `assembly.GetTypes() .Where`.
                type.IsClass &&
                // Meneruskan fungsi lambda `type => typeof(ControllerBase).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract &&
                // string.Equals(type.Namespace, ”Cashflowpoly.Api.Controllers”, StringComparison.Ord...` yang dijalankan oleh operasi pemanggil untuk memproses
                // setiap masukan sebagai argumen ke `assembly.GetTypes() .Where`.
                !type.IsAbstract &&
                // Meneruskan `type.Namespace` (nilai namespace) sebagai argumen ke `string.Equals`; Meneruskan nilai literal `”Cashflowpoly.Api.Controllers”`
                // sebagai argumen ke `string.Equals`; Meneruskan `StringComparison.Ordinal` (nilai ordinal) sebagai argumen ke `string.Equals`.
                string.Equals(type.Namespace, "Cashflowpoly.Api.Controllers", StringComparison.Ordinal))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(type => type.Name); dalam GetActionResponseMetadata; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderBy(type => type.Name);

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan objek baru bertipe
        // `List<ActionMetadata>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new List<ActionMetadata>();

        // Mengulangi setiap elemen `controllers`; elemen saat ini disimpan sebagai `controller` bertipe `var` untuk diproses oleh badan loop dalam
        // GetActionResponseMetadata.
        foreach (var controller in controllers)
        // Membuka scope loop setiap controller dari `controllers`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetActionResponseMetadata.
        {
            // Menyiapkan variabel lokal `controllerCodes` untuk nilai controller kode dengan memetakan setiap elemen `controller
            // .GetCustomAttributes<ProducesResponseTypeAttribute>(true)` melalui `attr => attr.StatusCode` menjadi bentuk hasil yang dibutuhkan. Tipe variabel
            // disimpulkan dari ekspresi nilai awal.
            var controllerCodes = controller
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GetCustomAttributes<ProducesResponseTypeAttribute>(true) dalam
                // GetActionResponseMetadata; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .GetCustomAttributes<ProducesResponseTypeAttribute>(true)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(attr => attr.StatusCode); dalam GetActionResponseMetadata; token pada
                // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Select(attr => attr.StatusCode);

            // Menyiapkan variabel lokal `actions` untuk nilai aksi dengan mengurutkan `controller.GetMethods(BindingFlags.Instance | BindingFlags.Public |
            // BindingFlags.DeclaredOnly) .Where(method => method.GetCustomAttributes<HttpMethodAttribute>(true).Any())` secara menaik berdasarkan `method =>
            // method.Name`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var actions = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(method => method.GetCustomAttributes<HttpMethodAttribute>(true).Any())
                // dalam GetActionResponseMetadata; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Where(method => method.GetCustomAttributes<HttpMethodAttribute>(true).Any())
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(method => method.Name); dalam GetActionResponseMetadata; token pada
                // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .OrderBy(method => method.Name);

            // Mengulangi setiap elemen `actions`; elemen saat ini disimpan sebagai `action` bertipe `var` untuk diproses oleh badan loop dalam
            // GetActionResponseMetadata.
            foreach (var action in actions)
            // Membuka scope loop setiap action dari `actions`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetActionResponseMetadata.
            {
                // Menyiapkan variabel lokal `actionCodes` untuk nilai aksi kode dengan memetakan setiap elemen `action
                // .GetCustomAttributes<ProducesResponseTypeAttribute>(true)` melalui `attr => attr.StatusCode` menjadi bentuk hasil yang dibutuhkan. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var actionCodes = action
                    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GetCustomAttributes<ProducesResponseTypeAttribute>(true) dalam
                    // GetActionResponseMetadata; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                    .GetCustomAttributes<ProducesResponseTypeAttribute>(true)
                    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(attr => attr.StatusCode); dalam GetActionResponseMetadata; token pada
                    // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                    .Select(attr => attr.StatusCode);

                // Menyiapkan variabel lokal `allCodes` untuk nilai all kode dengan mematerialisasi urutan `controllerCodes .Concat(actionCodes) .Distinct()
                // .OrderBy(code => code)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai
                // awal.
                var allCodes = controllerCodes
                    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Concat(actionCodes) dalam GetActionResponseMetadata; token pada baris ini
                    // menyambungkan bagian kode sebelum dan sesudahnya.
                    .Concat(actionCodes)
                    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Distinct() dalam GetActionResponseMetadata; token pada baris ini menyambungkan
                    // bagian kode sebelum dan sesudahnya.
                    .Distinct()
                    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(code => code) dalam GetActionResponseMetadata; token pada baris ini
                    // menyambungkan bagian kode sebelum dan sesudahnya.
                    .OrderBy(code => code)
                    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam GetActionResponseMetadata; token pada baris ini menyambungkan
                    // bagian kode sebelum dan sesudahnya.
                    .ToList();

                // Menjalankan menambahkan `new ActionMetadata( $”{controller.Name}.{action.Name}”, allCodes)` ke `result` dalam GetActionResponseMetadata.
                result.Add(new ActionMetadata(
                    // Meneruskan teks interpolasi `$”{controller.Name}.{action.Name}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai
                    // argumen ke konstruktor `ActionMetadata`.
                    $"{controller.Name}.{action.Name}",
                    // Meneruskan `allCodes` (nilai all kode) sebagai argumen ke konstruktor `ActionMetadata`.
                    allCodes));
            // Menutup scope loop setiap action dari `actions`; bagian berikut berada di luar batas blok tersebut dalam GetActionResponseMetadata.
            }
        // Menutup scope loop setiap controller dari `controllers`; bagian berikut berada di luar batas blok tersebut dalam GetActionResponseMetadata.
        }

        // Mengembalikan `result` (nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya) kepada pemanggil dalam GetActionResponseMetadata;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return result;
    // Menutup scope metode GetActionResponseMetadata; bagian berikut berada di luar batas blok tersebut dalam GetActionResponseMetadata.
    }

    /// <summary>
    /// Record internal yang menyimpan nama action dan daftar status code metadata-nya.
    /// </summary>
    // Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `ActionMetadata`; sealed mencegah tipe ini diturunkan lagi.
    private sealed record ActionMetadata(string ActionDisplayName, IReadOnlyList<int> StatusCodes);
// Menutup scope tipe ResponseMetadataTests; bagian berikut berada di luar batas blok tersebut.
}
