// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui ApiIntegrationCollection.
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests.Infrastructure;

// mendefinisikan koleksi fixture bersama dengan pengaturan (”ApiIntegration”, DisableParallelization = true).
[CollectionDefinition("ApiIntegration", DisableParallelization = true)]
/// <summary>
/// Definisi koleksi xUnit "ApiIntegration" yang membagikan ApiIntegrationTestFixture
/// ke seluruh kelas pengujian integrasi dan menonaktifkan eksekusi paralel.
/// </summary>
// Mendefinisikan tipe class `ApiIntegrationCollection` yang mewarisi atau menerapkan `ICollectionFixture<ApiIntegrationTestFixture>`; sealed
// mencegah tipe ini diturunkan lagi.
public sealed class ApiIntegrationCollection : ICollectionFixture<ApiIntegrationTestFixture>
// Membuka scope tipe ApiIntegrationCollection; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
// Menutup scope tipe ApiIntegrationCollection; bagian berikut berada di luar batas blok tersebut.
}
