namespace WindPark.Domain;

public record Turbine(char Id, int CapacityMwh, decimal CostPerMwh)
{
    public bool IsProfitable(decimal marketPrice) => marketPrice > CostPerMwh;
}