using System.Net.Http.Json;
using WindPark.Tests;

namespace WindPark.Tests.Integration;

public abstract class IntegrationTestBase(TestWebApplicationFactory factory) : IClassFixture<TestWebApplicationFactory>
{
    protected readonly HttpClient Client = factory.CreateClient();
    protected readonly TestWebApplicationFactory Factory = factory;
} 