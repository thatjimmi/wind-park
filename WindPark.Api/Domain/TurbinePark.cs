using WindPark.Domain.ValueObjects;

namespace WindPark.Domain;

public record TurbinePark(decimal ProductionTarget, decimal MarketPrice, List<Turbine> Turbines)
{
    public TurbinePark SetMarketPrice(decimal marketPrice) => this with { MarketPrice = marketPrice };

    public TurbinePark AdjustProductionTarget(decimal delta)
    {
        var requestedTarget = ProductionTarget + delta;
        var newProductionTarget = Math.Max(0, Math.Min(requestedTarget, Turbines.Sum(t => t.CapacityMwh)));
        return this with { ProductionTarget = newProductionTarget };
    }

    public int MaxCapacity => Turbines.Sum(t => t.CapacityMwh);

    public IReadOnlyList<TurbineStatus> DetermineTurbineProduction()
    {
        var profitableTurbines = Turbines
            .Where(t => t.IsProfitable(MarketPrice))
            .OrderBy(t => t.CostPerMwh) // Cheapest first
            .ThenBy(t => t.Id)                    
            .ToList();

        var remainingProductionTarget = ProductionTarget;
        var selectedTurbines = new List<Turbine>();

        foreach (var turbine in profitableTurbines)
        {
            if (remainingProductionTarget <= 0) break;

            if (turbine.CapacityMwh <= remainingProductionTarget)
            {
                selectedTurbines.Add(turbine);
                remainingProductionTarget -= turbine.CapacityMwh;
            }
        }

        return Turbines.Select(turbine => 
            new TurbineStatus(
                TurbineId: turbine.Id,
                CapacityMwh: turbine.CapacityMwh,
                CostPerMwh: turbine.CostPerMwh,
                IsRunning: selectedTurbines.Contains(turbine),
                CurrentProduction: selectedTurbines.Contains(turbine) ? turbine.CapacityMwh : 0,
                MarketPrice: MarketPrice
            )).ToList();
    }
    
    public ProductionSummary GetProductionSummary()
    {
        var turbineStatuses = DetermineTurbineProduction();
        var actualProduction = turbineStatuses.Sum(t => t.CurrentProduction);
        var totalRevenue = turbineStatuses.Sum(t => t.CurrentRevenue);
        var totalCost = turbineStatuses.Sum(t => t.CurrentCost);

        return new ProductionSummary(
            TurbineStatuses: turbineStatuses,
            ActualProduction: actualProduction,
            TargetProduction: ProductionTarget,
            MarketPrice: MarketPrice,
            TotalRevenue: totalRevenue,
            TotalCost: totalCost,
            TotalProfit: totalRevenue - totalCost
        );
    }
}