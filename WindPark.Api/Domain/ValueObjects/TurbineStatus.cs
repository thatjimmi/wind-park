namespace WindPark.Domain.ValueObjects;

public record TurbineStatus(
    char TurbineId,
    int CapacityMwh,
    decimal CostPerMwh,
    bool IsRunning,
    int CurrentProduction,
    decimal MarketPrice
    )
{
    public decimal CurrentRevenue => CurrentProduction * MarketPrice;
    public decimal CurrentCost => CurrentProduction * CostPerMwh;
    public decimal CurrentProfit => CurrentRevenue - CurrentCost;
    public bool IsProfitable => MarketPrice > CostPerMwh;
}