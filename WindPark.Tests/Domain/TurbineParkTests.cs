using FluentAssertions;
using WindPark.Domain;

namespace WindPark.Tests.Domain;

public class TurbineParkTests
{
    private static readonly List<Turbine> Turbines =
    [
        new('A', 2, 15m),
        new('B', 2, 5m),
        new('C', 6, 5m),
        new('D', 6, 5m),
        new('E', 5, 3m)
    ];
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)] 
    [InlineData(100)] 
    public void SetMarketPrice_ShouldHandleEdgeCases(decimal price)
    {
        // Arrange
        var park = new TurbinePark(10, 5, Turbines);
        
        // Act
        var result = park.SetMarketPrice(price);
        
        // Assert
        result.MarketPrice.Should().Be(price);
    }

    [Theory]
    [InlineData(10, 5,  15)] 
    [InlineData(10, -11,  0)] 
    [InlineData(10, 15,  21)]
    public void AdjustProductionTarget_ShouldRespectBounds(
        decimal initialTarget, decimal delta, decimal expectedTarget)
    {
        // Arrange
        var park = new TurbinePark(initialTarget, 5, Turbines);

        // Act
        var result = park.AdjustProductionTarget(delta);

        // Assert
        result.ProductionTarget.Should().Be(expectedTarget);
        result.MarketPrice.Should().Be(5);
    }
    
    [Fact]
    public void GetProductionPlans_WhenMarketPriceBelowAllCosts_ShouldRunNoTurbines()
    {
        // Arrange
        var park = new TurbinePark(21, 2m, Turbines);
        
        // Act
        var plans = park.DetermineTurbineProduction();
        
        // Assert
        plans.Should().AllSatisfy(p => p.IsRunning.Should().BeFalse());
        plans.Should().AllSatisfy(p => p.CurrentProduction.Should().Be(0));
    }

    [Fact]
    public void GetProductionPlans_ShouldSelectMostProfitableTurbines()
    {
        // Arrange
        var park = new TurbinePark(15, 6, Turbines); 
        
        // Act
        var plans = park.DetermineTurbineProduction();

        // Assert
        plans.Should().HaveCount(5);
        plans.Should().Contain(p => p.TurbineId == 'A' && !p.IsRunning && p.CurrentProduction == 0); 
        plans.Should().Contain(p => p.TurbineId == 'B' && p.IsRunning && p.CurrentProduction == 2); 
        plans.Should().Contain(p => p.TurbineId == 'C' && p.IsRunning && p.CurrentProduction == 6);  
        plans.Should().Contain(p => p.TurbineId == 'D' && !p.IsRunning && p.CurrentProduction == 0); 
        plans.Should().Contain(p => p.TurbineId == 'E' && p.IsRunning && p.CurrentProduction == 5); 
    }
    
    [Fact]
    public void AdjustProductionTarget_WhenTargetEqualsMaxCapacity_ShouldRunAllProfitableTurbines()
    {
        // Arrange
        var maxCapacity = Turbines.Sum(t => t.CapacityMwh);
        var park = new TurbinePark(maxCapacity, 16m, Turbines);
        
        // Act
        var plans = park.DetermineTurbineProduction();
        
        // Assert
        var totalProduction = plans.Sum(p => p.CurrentProduction);
        totalProduction.Should().Be(maxCapacity);
    }

    [Fact]
    public void GetProductionSummary_ShouldCalculateCorrectMetrics()
    {
        // Arrange
        var park = new TurbinePark(15, 6, Turbines);
        
        // Act
        var summary = park.GetProductionSummary();

        // Assert
        summary.ActualProduction.Should().Be(13);
        summary.TargetProduction.Should().Be(15);
        summary.MarketPrice.Should().Be(6);
        summary.TotalRevenue.Should().Be(78); 
        summary.TotalCost.Should().Be(55);    
        summary.TotalProfit.Should().Be(23); 
        summary.RunningTurbineCount.Should().Be(3);
        summary.TotalTurbineCount.Should().Be(5);
        summary.ProfitMargin.Should().Be(29.487179487179487179487179490m); 
        
        // Assert turbine statuses
        var turbineA = summary.TurbineStatuses.First(t => t.TurbineId == 'A');
        turbineA.IsRunning.Should().BeFalse();
        turbineA.CurrentProduction.Should().Be(0);
        turbineA.CurrentRevenue.Should().Be(0);
        turbineA.CurrentCost.Should().Be(0);
        turbineA.CurrentProfit.Should().Be(0);
        turbineA.IsProfitable.Should().BeFalse();

        var turbineE = summary.TurbineStatuses.First(t => t.TurbineId == 'E');
        turbineE.IsRunning.Should().BeTrue();
        turbineE.CurrentProduction.Should().Be(5);
        turbineE.CurrentRevenue.Should().Be(30);
        turbineE.CurrentCost.Should().Be(15);
        turbineE.CurrentProfit.Should().Be(15);
        turbineE.IsProfitable.Should().BeTrue();
    }
    
    [Fact]
    public void ChainedScenarios_BuildingOnEachOther()
    {
        // Start with initial state - 10 MWh target, 6€ price
        var park = new TurbinePark(10, 6m, Turbines);
    
        // Scenario 1: Initial state
        var plans1 = park.DetermineTurbineProduction();
        plans1.Sum(p => p.CurrentProduction).Should().Be(7);
        plans1.First(p => p.TurbineId == 'B').CurrentProduction.Should().Be(2);
        plans1.First(p => p.TurbineId == 'E').CurrentProduction.Should().Be(5);
    
        // Scenario 2: Increase target to 15MWh
        park = park.AdjustProductionTarget(5); // 10 + 5 = 15
        var plans2 = park.DetermineTurbineProduction();
        plans2.Sum(p => p.CurrentProduction).Should().Be(13);
        plans2.First(p => p.TurbineId == 'B').CurrentProduction.Should().Be(2);
        plans2.First(p => p.TurbineId == 'C').CurrentProduction.Should().Be(6);
        plans2.First(p => p.TurbineId == 'E').CurrentProduction.Should().Be(5);
    
        // Scenario 3: Reduce market price to 4€
        park = park.SetMarketPrice(4m);
        var plans3 = park.DetermineTurbineProduction();
        plans3.Sum(p => p.CurrentProduction).Should().Be(5);
        plans3.First(p => p.TurbineId == 'E').CurrentProduction.Should().Be(5);
        // All others should be 0 due to unprofitable market price
        plans3.Where(p => p.TurbineId != 'E').Should().AllSatisfy(p => p.CurrentProduction.Should().Be(0));
    }
} 