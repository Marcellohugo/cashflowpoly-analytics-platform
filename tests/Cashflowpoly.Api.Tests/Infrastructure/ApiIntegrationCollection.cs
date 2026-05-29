using Xunit;

namespace Cashflowpoly.Api.Tests.Infrastructure;

[CollectionDefinition("ApiIntegration", DisableParallelization = true)]
/// <summary>
/// Definisi koleksi xUnit "ApiIntegration" yang membagikan ApiIntegrationTestFixture
/// ke seluruh kelas pengujian integrasi dan menonaktifkan eksekusi paralel.
/// </summary>
public sealed class ApiIntegrationCollection : ICollectionFixture<ApiIntegrationTestFixture>
{
}
