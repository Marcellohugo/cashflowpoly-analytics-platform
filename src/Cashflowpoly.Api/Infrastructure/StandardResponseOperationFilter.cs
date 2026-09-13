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
    {
        var httpMethod = context.ApiDescription.HttpMethod?.ToUpperInvariant() ?? "GET";
        var hasRouteId = context.ApiDescription.RelativePath?.Contains('{') == true;

        var allowAnonymous = HasAttribute<AllowAnonymousAttribute>(context.MethodInfo) ||
                             HasAttribute<AllowAnonymousAttribute>(context.MethodInfo.DeclaringType);
        var hasAuthorize = HasAttribute<AuthorizeAttribute>(context.MethodInfo) ||
                           HasAttribute<AuthorizeAttribute>(context.MethodInfo.DeclaringType);
        var requiresAuth = hasAuthorize && !allowAnonymous;

        EnsureSuccessResponse(operation, httpMethod);
        AddErrorResponse(operation, context, "400", "Validasi request gagal.");

        if (requiresAuth)
        {
            AddErrorResponse(operation, context, "401", "Akses membutuhkan autentikasi.");
            AddErrorResponse(operation, context, "403", "Role tidak diizinkan.");
        }

        if (hasRouteId || httpMethod is "PUT" or "PATCH" or "DELETE")
        {
            AddErrorResponse(operation, context, "404", "Resource tidak ditemukan.");
        }

        if (httpMethod is "POST" or "PUT" or "PATCH")
        {
            AddErrorResponse(operation, context, "409", "Terjadi konflik data.");
            AddErrorResponse(operation, context, "422", "Aturan domain tidak terpenuhi.");
        }

        AddErrorResponse(operation, context, "429", "Terlalu banyak request.");
        AddErrorResponse(operation, context, "500", "Terjadi kesalahan pada server.");
    }

    /// <summary>
    /// Menambahkan respons success default (200/201/204) jika belum ada respons 2xx pada operasi.
    /// </summary>
    // Mendefinisikan metode `EnsureSuccessResponse` dengan hasil bertipe `void`. Menambahkan respons success default (200/201/204) jika belum ada
    // respons 2xx pada operasi. Masukan: Parameter `operation` bertipe `OpenApiOperation` membawa nilai operation; Parameter `httpMethod` bertipe
    // `string` membawa nilai HTTP method.
    private static void EnsureSuccessResponse(OpenApiOperation operation, string httpMethod)
    {
        operation.Responses ??= new OpenApiResponses();

        if (operation.Responses.Keys.Any(key => key.StartsWith("2", StringComparison.Ordinal)))
        {
            return;
        }

        var successCode = httpMethod switch
        {
            // Untuk pola `”POST”`, menghasilkan nilai literal `”201”` sebagai hasil switch.
            "POST" => "201",
            // Untuk pola `”DELETE”`, menghasilkan nilai literal `”204”` sebagai hasil switch.
            "DELETE" => "204",
            // Untuk pola `_`, menghasilkan nilai literal `”200”` sebagai hasil switch.
            _ => "200"
        };

        operation.Responses[successCode] = new OpenApiResponse { Description = "Berhasil." };
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
    {
        operation.Responses ??= new OpenApiResponses();

        if (operation.Responses.ContainsKey(statusCode))
        {
            return;
        }

        var schema = context.SchemaGenerator.GenerateSchema(typeof(ErrorResponse), context.SchemaRepository);
        operation.Responses[statusCode] = new OpenApiResponse
        {
            Description = description,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new()
                {
                    Schema = schema
                }
            }
        };
    }

    /// <summary>
    /// Mengecek apakah anggota refleksi memiliki atribut tertentu.
    /// </summary>
    // Mendefinisikan metode `HasAttribute` dengan hasil bertipe `bool`. Mengecek apakah anggota refleksi memiliki atribut tertentu. Masukan: Parameter
    // `memberInfo` bertipe `MemberInfo?` membawa nilai member info; nilai null diizinkan ketika data opsional belum tersedia.
    private static bool HasAttribute<T>(MemberInfo? memberInfo) where T : Attribute
    {
        return memberInfo?.GetCustomAttributes(typeof(T), true).Any() == true;
    }
}
