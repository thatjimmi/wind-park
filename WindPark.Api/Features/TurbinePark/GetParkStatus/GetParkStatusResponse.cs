using WindPark.Domain.ValueObjects;

namespace WindPark.Features.TurbinePark.GetParkStatus;

public record GetParkStatusResponse(ProductionSummary Status);
