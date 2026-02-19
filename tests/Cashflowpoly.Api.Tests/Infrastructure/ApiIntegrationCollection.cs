// Fungsi file: Menguji perilaku dan kontrak komponen pada domain ApiIntegrationCollection.
using Xunit;

namespace Cashflowpoly.Api.Tests.Infrastructure;

[CollectionDefinition("ApiIntegration", DisableParallelization = true)]
/// <summary>
/// Menyatakan peran utama tipe ApiIntegrationCollection pada modul ini.
/// </summary>
public sealed class ApiIntegrationCollection : ICollectionFixture<ApiIntegrationTestFixture>
{
}
