using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using WindPark.Features.TurbinePark.AdjustProductionTarget;
using WindPark.Features.TurbinePark.GetParkStatus;
using WindPark.Shared;
using WindPark.Tests.Integration;
using Xunit.Sdk;

namespace WindPark.Tests.Integration;

public class TurbineParkEndpointsTests(TestWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetStatus_ShouldReturnCurrentParkStatus()
    {
        // Act
        var response = await Client.GetAsync("/api/turbines/status");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<GetParkStatusResponse>();
        content.Should().NotBeNull();
        content!.Status.Should().NotBeNull();
        content.Status.TotalTurbineCount.Should().Be(5);
    }

    [Fact]
    public async Task SetMarketPrice_ShouldUpdatePrice()
    {
        // Arrange
        var price = 10.0m;
        
        // Act
        var response = await Client.PostAsJsonAsync("/api/turbines/market-price", new { price });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify the price was updated by getting the status
        var statusResponse = await Client.GetAsync("/api/turbines/status");
        var status = await statusResponse.Content.ReadFromJsonAsync<GetParkStatusResponse>();
        status!.Status.MarketPrice.Should().Be(price);
    }

    [Fact]
    public async Task AdjustProductionTarget_ShouldUpdateTarget()
    {
        // Arrange
        var delta = 5.0m;
        
        // Act
        var response = await Client.PostAsJsonAsync("/api/turbines/production-target", new { delta });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify the target was updated by getting the status
        var statusResponse = await Client.GetAsync("/api/turbines/status");
        var status = await statusResponse.Content.ReadFromJsonAsync<GetParkStatusResponse>();
        status!.Status.TargetProduction.Should().Be(15); // Initial 10 + delta 5
    }

    [Fact]
    public async Task AdjustProductionTargetWithTooMuch_ShouldFail()
    {
        // Arrange
        var delta = 999.0m;
        
        // Act
        var response = await Client.PostAsJsonAsync("/api/turbines/production-target", new { delta });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        errorResponse!.Message.Should().Be("Invalid production target adjustment");
        errorResponse.Errors.Should().Contain(e => e.Contains("Delta"));
    }
} 