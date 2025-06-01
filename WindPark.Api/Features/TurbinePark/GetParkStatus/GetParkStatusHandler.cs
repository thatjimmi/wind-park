using WindPark.Infrastructure.Repositories;
using WindPark.Shared;

namespace WindPark.Features.TurbinePark.GetParkStatus;

public class GetParkStatusHandler(
    ITurbineParkRepository turbineParkRepository,
    ILogger<GetParkStatusHandler> logger)
{
    public async Task<Result<GetParkStatusResponse>> Handle(GetParkStatusQuery getParkStatusQuery)
    {
        logger.LogInformation("Getting current park status");

        try
        {
            var park = await turbineParkRepository.GetTurbineParkState();
            
            var summary = park.GetProductionSummary();
            
            var response = new GetParkStatusResponse(summary);

            return Result<GetParkStatusResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting current status");
            return Result<GetParkStatusResponse>.Failure("Failed to get current status");
        }
    }
}