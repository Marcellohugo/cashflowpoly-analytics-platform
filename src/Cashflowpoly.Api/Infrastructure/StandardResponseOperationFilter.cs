// Fungsi file: Menyediakan dukungan infrastruktur API melalui StandardResponseOperationFilter.
// Mengimpor namespace `System.Reflection` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Reflection;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Microsoft.AspNetCore.Authorization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Authorization;
// Mengimpor namespace `Microsoft.OpenApi` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.OpenApi;
// Mengimpor namespace `Swashbuckle.AspNetCore.SwaggerGen` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Swashbuckle.AspNetCore.SwaggerGen;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Infrastructure;

/// <summary>
/// Operation filter Swagger yang otomatis menambahkan respons error standar dan success default ke semua operasi API berdasarkan konteks HTTP method, route, dan otorisasi.
/// </summary>
// Mendefinisikan tipe class `StandardResponseOperationFilter` yang mewarisi atau menerapkan `IOperationFilter`; sealed mencegah tipe ini diturunkan
// lagi.
public sealed class StandardResponseOperationFilter : IOperationFilter
// Membuka scope tipe StandardResponseOperationFilter; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Menerapkan respons error standar dan success default pada operasi Swagger sesuai HTTP method, route pattern, dan atribut otorisasi.
    /// </summary>
    /// <param name="operation">Operasi OpenAPI yang sedang diproses.</param>
    /// <param name="context">Konteks filter berisi metadata endpoint.</param>
    // Mendefinisikan metode `Apply` dengan hasil bertipe `void`. Menerapkan respons error standar dan success default pada operasi Swagger sesuai HTTP
    // method, route pattern, dan atribut otorisasi. Masukan: Parameter `operation` bertipe `OpenApiOperation` membawa nilai operation; Parameter
    // `context` bertipe `OperationFilterContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini.
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    // Membuka scope metode Apply; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Apply.
    {
        // Menyiapkan variabel lokal `httpMethod` untuk nilai HTTP method dengan `context.ApiDescription.HttpMethod?.ToUpperInvariant()` bila tidak null;
        // jika null gunakan `”GET”` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var httpMethod = context.ApiDescription.HttpMethod?.ToUpperInvariant() ?? "GET";
        // Menyiapkan variabel lokal `hasRouteId` untuk nilai memiliki route identitas dengan perbandingan kesamaan antara
        // `context.ApiDescription.RelativePath?.Contains('{')` dan `true`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var hasRouteId = context.ApiDescription.RelativePath?.Contains('{') == true;

        // Menyiapkan variabel lokal `allowAnonymous` untuk nilai allow anonymous dengan gabungan syarat OR: setidaknya satu kondisi wajib benar antara
        // `HasAttribute<AllowAnonymousAttribute>(context.MethodInfo)` dan `HasAttribute<AllowAnonymousAttribute>(context.MethodInfo.DeclaringType)`; sisi
        // kanan diperiksa hanya jika sisi kiri salah. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var allowAnonymous = HasAttribute<AllowAnonymousAttribute>(context.MethodInfo) ||
                             // Melanjutkan pengolahan dengan memanggil `HasAttribute<AllowAnonymousAttribute>` dengan `context.MethodInfo.DeclaringType` dalam Apply.
                             HasAttribute<AllowAnonymousAttribute>(context.MethodInfo.DeclaringType);
        // Menyiapkan variabel lokal `hasAuthorize` untuk nilai memiliki authorize dengan gabungan syarat OR: setidaknya satu kondisi wajib benar antara
        // `HasAttribute<AuthorizeAttribute>(context.MethodInfo)` dan `HasAttribute<AuthorizeAttribute>(context.MethodInfo.DeclaringType)`; sisi kanan
        // diperiksa hanya jika sisi kiri salah. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var hasAuthorize = HasAttribute<AuthorizeAttribute>(context.MethodInfo) ||
                           // Melanjutkan pengolahan dengan memanggil `HasAttribute<AuthorizeAttribute>` dengan `context.MethodInfo.DeclaringType` dalam Apply.
                           HasAttribute<AuthorizeAttribute>(context.MethodInfo.DeclaringType);
        // Menyiapkan variabel lokal `requiresAuth` untuk nilai requires auth dengan gabungan syarat AND: kedua kondisi wajib benar antara `hasAuthorize`
        // dan `!allowAnonymous`; sisi kanan diperiksa hanya jika sisi kiri benar. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var requiresAuth = hasAuthorize && !allowAnonymous;

        // Menjalankan memanggil `EnsureSuccessResponse` dengan `operation`, `httpMethod` dalam Apply.
        EnsureSuccessResponse(operation, httpMethod);
        // Menjalankan memanggil `AddErrorResponse` dengan `operation`, `context`, `”400”`, `”Validasi request gagal.”` dalam Apply.
        AddErrorResponse(operation, context, "400", "Validasi request gagal.");

        // Memeriksa `requiresAuth` (nilai requires auth); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Apply.
        if (requiresAuth)
        // Membuka scope cabang if untuk kondisi `requiresAuth`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Apply.
        {
            // Menjalankan memanggil `AddErrorResponse` dengan `operation`, `context`, `”401”`, `”Akses membutuhkan autentikasi.”` dalam Apply.
            AddErrorResponse(operation, context, "401", "Akses membutuhkan autentikasi.");
            // Menjalankan memanggil `AddErrorResponse` dengan `operation`, `context`, `”403”`, `”Role tidak diizinkan.”` dalam Apply.
            AddErrorResponse(operation, context, "403", "Role tidak diizinkan.");
        // Menutup scope cabang if untuk kondisi `requiresAuth`; bagian berikut berada di luar batas blok tersebut dalam Apply.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `hasRouteId` dan `httpMethod is ”PUT” or ”PATCH” or ”DELETE”`; sisi
        // kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Apply.
        if (hasRouteId || httpMethod is "PUT" or "PATCH" or "DELETE")
        // Membuka scope cabang if untuk kondisi `hasRouteId || httpMethod is ”PUT” or ”PATCH” or ”DELETE”`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam Apply.
        {
            // Menjalankan memanggil `AddErrorResponse` dengan `operation`, `context`, `”404”`, `”Resource tidak ditemukan.”` dalam Apply.
            AddErrorResponse(operation, context, "404", "Resource tidak ditemukan.");
        // Menutup scope cabang if untuk kondisi `hasRouteId || httpMethod is ”PUT” or ”PATCH” or ”DELETE”`; bagian berikut berada di luar batas blok
        // tersebut dalam Apply.
        }

        // Memeriksa hasil pencocokan `httpMethod` dengan pola `”POST” or ”PUT” or ”PATCH”`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam Apply.
        if (httpMethod is "POST" or "PUT" or "PATCH")
        // Membuka scope cabang if untuk kondisi `httpMethod is ”POST” or ”PUT” or ”PATCH”`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam Apply.
        {
            // Menjalankan memanggil `AddErrorResponse` dengan `operation`, `context`, `”409”`, `”Terjadi konflik data.”` dalam Apply.
            AddErrorResponse(operation, context, "409", "Terjadi konflik data.");
            // Menjalankan memanggil `AddErrorResponse` dengan `operation`, `context`, `”422”`, `”Aturan domain tidak terpenuhi.”` dalam Apply.
            AddErrorResponse(operation, context, "422", "Aturan domain tidak terpenuhi.");
        // Menutup scope cabang if untuk kondisi `httpMethod is ”POST” or ”PUT” or ”PATCH”`; bagian berikut berada di luar batas blok tersebut dalam Apply.
        }

        // Menjalankan memanggil `AddErrorResponse` dengan `operation`, `context`, `”429”`, `”Terlalu banyak request.”` dalam Apply.
        AddErrorResponse(operation, context, "429", "Terlalu banyak request.");
        // Menjalankan memanggil `AddErrorResponse` dengan `operation`, `context`, `”500”`, `”Terjadi kesalahan pada server.”` dalam Apply.
        AddErrorResponse(operation, context, "500", "Terjadi kesalahan pada server.");
    // Menutup scope metode Apply; bagian berikut berada di luar batas blok tersebut dalam Apply.
    }

    /// <summary>
    /// Menambahkan respons success default (200/201/204) jika belum ada respons 2xx pada operasi.
    /// </summary>
    // Mendefinisikan metode `EnsureSuccessResponse` dengan hasil bertipe `void`. Menambahkan respons success default (200/201/204) jika belum ada
    // respons 2xx pada operasi. Masukan: Parameter `operation` bertipe `OpenApiOperation` membawa nilai operation; Parameter `httpMethod` bertipe
    // `string` membawa nilai HTTP method.
    private static void EnsureSuccessResponse(OpenApiOperation operation, string httpMethod)
    // Membuka scope metode EnsureSuccessResponse; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam EnsureSuccessResponse.
    {
        // Memperbarui `operation.Responses` hanya jika nilainya null, menggunakan objek baru bertipe `OpenApiResponses` dengan nilai awal sesuai
        // konstruktornya dalam EnsureSuccessResponse.
        operation.Responses ??= new OpenApiResponses();

        // Memeriksa memeriksa apakah `operation.Responses.Keys` memiliki setidaknya satu elemen yang memenuhi `key => key.StartsWith(”2”,
        // StringComparison.Ordinal)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam EnsureSuccessResponse.
        if (operation.Responses.Keys.Any(key => key.StartsWith("2", StringComparison.Ordinal)))
        // Membuka scope cabang if untuk kondisi `operation.Responses.Keys.Any(key => key.StartsWith(”2”, StringComparison.Ordinal))`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam EnsureSuccessResponse.
        {
            // Mengakhiri eksekusi lebih awal dalam EnsureSuccessResponse tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `operation.Responses.Keys.Any(key => key.StartsWith(”2”, StringComparison.Ordinal))`; bagian berikut berada
        // di luar batas blok tersebut dalam EnsureSuccessResponse.
        }

        // Menyiapkan variabel lokal `successCode` untuk nilai success kode dengan hasil pemetaan `httpMethod` melalui cabang pola switch yang cocok. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var successCode = httpMethod switch
        // Membuka scope pemetaan switch atas `httpMethod`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam EnsureSuccessResponse.
        {
            // Untuk pola `”POST”`, menghasilkan nilai literal `”201”` sebagai hasil switch.
            "POST" => "201",
            // Untuk pola `”DELETE”`, menghasilkan nilai literal `”204”` sebagai hasil switch.
            "DELETE" => "204",
            // Untuk pola `_`, menghasilkan nilai literal `”200”` sebagai hasil switch.
            _ => "200"
        // Menutup scope pemetaan switch atas `httpMethod`; bagian berikut berada di luar batas blok tersebut dalam EnsureSuccessResponse.
        };

        // Memperbarui `operation.Responses[successCode]` menggunakan objek baru bertipe `OpenApiResponse` dengan nilai awal sesuai konstruktornya dalam
        // EnsureSuccessResponse.
        operation.Responses[successCode] = new OpenApiResponse { Description = "Berhasil." };
    // Menutup scope metode EnsureSuccessResponse; bagian berikut berada di luar batas blok tersebut dalam EnsureSuccessResponse.
    }

    /// <summary>
    /// Menambahkan respons error dengan status code dan deskripsi tertentu beserta schema ErrorResponse pada operasi.
    /// </summary>
    // Mendefinisikan metode `AddErrorResponse` dengan hasil bertipe `void`. Menambahkan respons error dengan status code dan deskripsi tertentu beserta
    // schema ErrorResponse pada operasi. Masukan: Parameter `operation` bertipe `OpenApiOperation` membawa nilai operation; Parameter `context` bertipe
    // `OperationFilterContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini; Parameter `statusCode` bertipe `string`
    // membawa kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan; Parameter `description` bertipe `string` membawa nilai
    // description.
    private static void AddErrorResponse(
        // Parameter `operation` bertipe `OpenApiOperation` membawa nilai operation.
        OpenApiOperation operation,
        // Parameter `context` bertipe `OperationFilterContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini.
        OperationFilterContext context,
        // Parameter `statusCode` bertipe `string` membawa kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan.
        string statusCode,
        // Parameter `description` bertipe `string` membawa nilai description.
        string description)
    // Membuka scope metode AddErrorResponse; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AddErrorResponse.
    {
        // Memperbarui `operation.Responses` hanya jika nilainya null, menggunakan objek baru bertipe `OpenApiResponses` dengan nilai awal sesuai
        // konstruktornya dalam AddErrorResponse.
        operation.Responses ??= new OpenApiResponses();

        // Memeriksa memeriksa keberadaan kunci `statusCode` dalam `operation.Responses`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // AddErrorResponse.
        if (operation.Responses.ContainsKey(statusCode))
        // Membuka scope cabang if untuk kondisi `operation.Responses.ContainsKey(statusCode)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam AddErrorResponse.
        {
            // Mengakhiri eksekusi lebih awal dalam AddErrorResponse tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `operation.Responses.ContainsKey(statusCode)`; bagian berikut berada di luar batas blok tersebut dalam
        // AddErrorResponse.
        }

        // Menyiapkan variabel lokal `schema` untuk nilai schema dengan memanggil `context.SchemaGenerator.GenerateSchema` dengan `typeof(ErrorResponse)`,
        // `context.SchemaRepository`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var schema = context.SchemaGenerator.GenerateSchema(typeof(ErrorResponse), context.SchemaRepository);
        // Memperbarui `operation.Responses[statusCode]` menggunakan objek baru bertipe `OpenApiResponse` dengan nilai awal sesuai konstruktornya dalam
        // AddErrorResponse.
        operation.Responses[statusCode] = new OpenApiResponse
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AddErrorResponse.
        {
            // Memperbarui `Description` menggunakan `description` (nilai description) dalam AddErrorResponse.
            Description = description,
            // Memperbarui `Content` menggunakan objek baru bertipe `Dictionary<string, OpenApiMediaType>` dengan nilai awal sesuai konstruktornya dalam
            // AddErrorResponse.
            Content = new Dictionary<string, OpenApiMediaType>
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AddErrorResponse.
            {
                // Memperbarui `[”application/json”]` menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () dalam AddErrorResponse.
                ["application/json"] = new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AddErrorResponse.
                {
                    // Memperbarui `Schema` menggunakan `schema` (nilai schema) dalam AddErrorResponse.
                    Schema = schema
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam AddErrorResponse.
                }
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam AddErrorResponse.
            }
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam AddErrorResponse.
        };
    // Menutup scope metode AddErrorResponse; bagian berikut berada di luar batas blok tersebut dalam AddErrorResponse.
    }

    /// <summary>
    /// Mengecek apakah anggota refleksi memiliki atribut tertentu.
    /// </summary>
    // Mendefinisikan metode `HasAttribute` dengan hasil bertipe `bool`. Mengecek apakah anggota refleksi memiliki atribut tertentu. Masukan: Parameter
    // `memberInfo` bertipe `MemberInfo?` membawa nilai member info; nilai null diizinkan ketika data opsional belum tersedia.
    private static bool HasAttribute<T>(MemberInfo? memberInfo) where T : Attribute
    // Membuka scope metode HasAttribute; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam HasAttribute.
    {
        // Mengembalikan perbandingan kesamaan antara `memberInfo?.GetCustomAttributes(typeof(T), true).Any()` dan `true` kepada pemanggil dalam
        // HasAttribute; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return memberInfo?.GetCustomAttributes(typeof(T), true).Any() == true;
    // Menutup scope metode HasAttribute; bagian berikut berada di luar batas blok tersebut dalam HasAttribute.
    }
// Menutup scope tipe StandardResponseOperationFilter; bagian berikut berada di luar batas blok tersebut.
}
