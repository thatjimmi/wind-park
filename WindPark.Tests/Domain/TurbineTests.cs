using FluentAssertions;
using WindPark.Domain;
using Xunit;

namespace WindPark.Tests.Domain;

public class TurbineTests
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
    [InlineData(0, 0)] 
    [InlineData(4, 1)] 
    [InlineData(6, 4)]
    [InlineData(16, 5)]
    public void IsProfitable_ShouldReturnCorrectCount(decimal marketPrice, int expectedProfitableCount)
    {
        // Act
        var profitableCount = Turbines.Count(t => t.IsProfitable(marketPrice));

        // Assert
        profitableCount.Should().Be(expectedProfitableCount);
    }
} 