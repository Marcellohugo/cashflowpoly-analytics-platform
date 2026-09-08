// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui BearerTokenHandlerTests.
// Mengimpor namespace `System.Net` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net;
// Mengimpor namespace `System.Security.Claims` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Claims;
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;
// Mengimpor namespace `Microsoft.AspNetCore.Http.Features` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http.Features;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `BearerTokenHandlerTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class BearerTokenHandlerTests
// Membuka scope tipe BearerTokenHandlerTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SendAsync_ForwardsIdentityLanguageClientIpAndTraceId` dengan hasil bertipe `Task`; operasi ini menangani send asinkron
    // forwards identity language client ip dan trace identitas. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan
    // penyelesaian melalui Task.
    public async Task SendAsync_ForwardsIdentityLanguageClientIpAndTraceId()
    // Membuka scope metode SendAsync_ForwardsIdentityLanguageClientIpAndTraceId; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
    {
        // Menyiapkan variabel lokal `session` untuk nilai sesi dengan objek baru bertipe `TestSession` dengan nilai awal sesuai konstruktornya. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var session = new TestSession();
        // Menjalankan memanggil `session.SetString` dengan `AuthConstants.SessionLanguageKey`, `AuthConstants.LanguageEn` dalam
        // SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
        session.SetString(AuthConstants.SessionLanguageKey, AuthConstants.LanguageEn);
        // Menyiapkan variabel lokal `context` untuk konteks operasi yang menyediakan data lingkungan pemrosesan saat ini dengan objek baru bertipe
        // `DefaultHttpContext` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var context = new DefaultHttpContext
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
        {
            // Memperbarui `TraceIdentifier` menggunakan nilai literal `”trace-123”` dalam SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
            TraceIdentifier = "trace-123"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
        };
        // Memperbarui `context.User` menggunakan objek baru bertipe `ClaimsPrincipal` dengan argumen (new ClaimsIdentity( [new
        // Claim(AuthConstants.AccessTokenClaim, ”token-123”)], ”test”)) dalam SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            // Meneruskan koleksi berisi new Claim(AuthConstants.AccessTokenClaim, ”token-1... sebagai argumen ke konstruktor `ClaimsIdentity`; Meneruskan
            // `AuthConstants.AccessTokenClaim` (nilai akses token claim) sebagai argumen ke konstruktor `Claim`; Meneruskan nilai literal `”token-123”` sebagai
            // argumen ke konstruktor `Claim`.
            [new Claim(AuthConstants.AccessTokenClaim, "token-123")],
            // Meneruskan nilai literal `”test”` sebagai argumen ke konstruktor `ClaimsIdentity`.
            "test"));
        // Memperbarui `context.Connection.RemoteIpAddress` menggunakan memanggil `IPAddress.Parse` dengan `”203.0.113.8”` dalam
        // SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
        context.Connection.RemoteIpAddress = IPAddress.Parse("203.0.113.8");
        // Menjalankan memanggil `context.Features.Set<ISessionFeature>` dengan `new TestSessionFeature(session)` dalam
        // SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
        context.Features.Set<ISessionFeature>(new TestSessionFeature(session));

        // Menyiapkan variabel lokal `terminalHandler` untuk nilai terminal handler dengan objek baru bertipe `CaptureHandler` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var terminalHandler = new CaptureHandler();
        // Menyiapkan variabel lokal `handler` untuk nilai handler dengan objek baru bertipe `BearerTokenHandler` dengan argumen (new HttpContextAccessor {
        // HttpContext = context }). Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var handler = new BearerTokenHandler(new HttpContextAccessor { HttpContext = context })
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
        {
            // Memperbarui `InnerHandler` menggunakan `terminalHandler` (nilai terminal handler) dalam SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
            InnerHandler = terminalHandler
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
        };
        // Menyiapkan variabel lokal `invoker` untuk nilai invoker dengan objek baru bertipe `HttpMessageInvoker` dengan argumen (handler). Tipe variabel
        // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var invoker = new HttpMessageInvoker(handler);

        // Menjalankan hasil operasi asinkron memanggil `invoker.SendAsync` dengan `new HttpRequestMessage(HttpMethod.Get,
        // ”http://api.test/api/v1/sessions”)`, `CancellationToken.None`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
        await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://api.test/api/v1/sessions"), CancellationToken.None);

        // Menjalankan pemeriksaan NotNull atas `terminalHandler.Request` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
        Assert.NotNull(terminalHandler.Request);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Bearer”`,
        // `terminalHandler.Request.Headers.Authorization?.Scheme`); pengujian gagal jika keduanya berbeda dalam
        // SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
        Assert.Equal("Bearer", terminalHandler.Request.Headers.Authorization?.Scheme);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”token-123”`,
        // `terminalHandler.Request.Headers.Authorization?.Parameter`); pengujian gagal jika keduanya berbeda dalam
        // SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
        Assert.Equal("token-123", terminalHandler.Request.Headers.Authorization?.Parameter);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”en”`,
        // `Assert.Single(terminalHandler.Request.Headers.AcceptLanguage).Value`); pengujian gagal jika keduanya berbeda dalam
        // SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
        Assert.Equal("en", Assert.Single(terminalHandler.Request.Headers.AcceptLanguage).Value);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”203.0.113.8”`,
        // `Assert.Single(terminalHandler.Request.Headers.GetValues(”X-Forwarded-For”))`); pengujian gagal jika keduanya berbeda dalam
        // SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
        Assert.Equal("203.0.113.8", Assert.Single(terminalHandler.Request.Headers.GetValues("X-Forwarded-For")));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”trace-123”`,
        // `Assert.Single(terminalHandler.Request.Headers.GetValues(”X-Client-Request-Id”))`); pengujian gagal jika keduanya berbeda dalam
        // SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
        Assert.Equal("trace-123", Assert.Single(terminalHandler.Request.Headers.GetValues("X-Client-Request-Id")));
    // Menutup scope metode SendAsync_ForwardsIdentityLanguageClientIpAndTraceId; bagian berikut berada di luar batas blok tersebut dalam
    // SendAsync_ForwardsIdentityLanguageClientIpAndTraceId.
    }

    // Mendefinisikan tipe class `CaptureHandler` yang mewarisi atau menerapkan `HttpMessageHandler`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class CaptureHandler : HttpMessageHandler
    // Membuka scope tipe CaptureHandler; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `Request` bertipe `HttpRequestMessage?` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan;
        // get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
        public HttpRequestMessage? Request { get; private set; }

        // Mendefinisikan metode `SendAsync` dengan hasil bertipe `Task<HttpResponseMessage>`; operasi ini menangani send asinkron. Masukan: Parameter
        // `request` bertipe `HttpRequestMessage` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter
        // `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan
        // atau aplikasi berhenti.
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        // Membuka scope metode SendAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SendAsync.
        {
            // Memperbarui `Request` menggunakan `request` (data masukan permintaan yang akan divalidasi atau diteruskan ke layanan) dalam SendAsync.
            Request = request;
            // Mengembalikan memanggil `Task.FromResult` dengan `new HttpResponseMessage(HttpStatusCode.OK)` kepada pemanggil dalam SendAsync; eksekusi jalur
            // ini selesai setelah nilai hasil ditentukan.
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        // Menutup scope metode SendAsync; bagian berikut berada di luar batas blok tersebut dalam SendAsync.
        }
    // Menutup scope tipe CaptureHandler; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `TestSessionFeature` yang mewarisi atau menerapkan `ISessionFeature`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class TestSessionFeature(ISession session) : ISessionFeature
    // Membuka scope tipe TestSessionFeature; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `Session` bertipe `ISession` untuk nilai sesi; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai
        // awalnya `session` (nilai sesi).
        public ISession Session { get; set; } = session;
    // Menutup scope tipe TestSessionFeature; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `TestSession` yang mewarisi atau menerapkan `ISession`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class TestSession : ISession
    // Membuka scope tipe TestSession; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendeklarasikan field bertipe `Dictionary<string, byte[]>`: `_values` menyimpan nilai nilai dengan nilai awal objek baru dengan tipe mengikuti
        // konteks tujuan dan argumen (StringComparer.Ordinal). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
        private readonly Dictionary<string, byte[]> _values = new(StringComparer.Ordinal);

        // Mendefinisikan properti `IsAvailable` bertipe `bool` untuk nilai berstatus tersedia; nilainya dihitung dari true, yaitu kondisi aktif/terpenuhi.
        public bool IsAvailable => true;
        // Mendefinisikan properti `Id` bertipe `string` untuk nilai identitas; nilainya dihitung dari nilai literal `”test-session”`.
        public string Id => "test-session";
        // Mendefinisikan properti `Keys` bertipe `IEnumerable<string>` untuk nilai kunci; nilainya dihitung dari `_values.Keys` (nilai kunci).
        public IEnumerable<string> Keys => _values.Keys;

        // Mendefinisikan metode `Clear` dengan hasil bertipe `void`; operasi ini menangani clear. Nilai hasil langsung berasal dari mengosongkan seluruh
        // elemen `_values`.
        public void Clear() => _values.Clear();

        // Mendefinisikan metode `CommitAsync` dengan hasil bertipe `Task`; operasi ini menangani commit asinkron. Masukan: Parameter `cancellationToken`
        // bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
        // berhenti; bila argumen tidak diberikan digunakan nilai literal `default`. Nilai hasil langsung berasal dari `Task.CompletedTask` (nilai selesai
        // task).
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        // Mendefinisikan metode `LoadAsync` dengan hasil bertipe `Task`; operasi ini menangani load asinkron. Masukan: Parameter `cancellationToken`
        // bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
        // berhenti; bila argumen tidak diberikan digunakan nilai literal `default`. Nilai hasil langsung berasal dari `Task.CompletedTask` (nilai selesai
        // task).
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        // Mendefinisikan metode `Remove` dengan hasil bertipe `void`; operasi ini menangani remove. Masukan: Parameter `key` bertipe `string` membawa nilai
        // kunci. Nilai hasil langsung berasal dari menghapus elemen dari `_values` berdasarkan `key`.
        public void Remove(string key) => _values.Remove(key);

        // Mendefinisikan metode `Set` dengan hasil bertipe `void`; operasi ini menangani set. Masukan: Parameter `key` bertipe `string` membawa nilai
        // kunci; Parameter `value` bertipe `byte[]` membawa nilai nilai. Nilai hasil langsung berasal dari `_values[key] = value`.
        public void Set(string key, byte[] value) => _values[key] = value;

        // Mendefinisikan metode `TryGetValue` dengan hasil bertipe `bool`; operasi ini menangani try get nilai. Masukan: Parameter `key` bertipe `string`
        // membawa nilai kunci; Parameter `value` bertipe `byte[]` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh
        // metode. Nilai hasil langsung berasal dari mencari kunci `key` pada `_values`; hasil boolean menandakan kunci ditemukan dan argumen out menerima
        // nilainya.
        public bool TryGetValue(string key, out byte[] value) => _values.TryGetValue(key, out value!);
    // Menutup scope tipe TestSession; bagian berikut berada di luar batas blok tersebut.
    }
// Menutup scope tipe BearerTokenHandlerTests; bagian berikut berada di luar batas blok tersebut.
}
