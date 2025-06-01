namespace WindPark.Domain.ValueObjects;

public record ProductionSummary(
    IReadOnlyList<TurbineStatus> TurbineStatuses,
    decimal ActualProduction,
    decimal TargetProduction,
    decimal MarketPrice,
    decimal TotalRevenue,
    decimal TotalCost,
    decimal TotalProfit)
{
    public bool TargetMet => ActualProduction >= TargetProduction;
    public int RunningTurbineCount => TurbineStatuses.Count(t => t.IsRunning);
    public int TotalTurbineCount => TurbineStatuses.Count;
    public decimal ProfitMargin => TotalRevenue > 0 ? (TotalProfit / TotalRevenue) * 100 : 0;
}