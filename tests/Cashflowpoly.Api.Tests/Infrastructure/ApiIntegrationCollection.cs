using Xunit;

namespace Cashflowpoly.Api.Tests.Infrastructure;

[CollectionDefinition("ApiIntegration", DisableParallelization = true)]
public sealed class ApiIntegrationCollection : ICollectionFixture<ApiIntegrationTestFixture>
{
}
