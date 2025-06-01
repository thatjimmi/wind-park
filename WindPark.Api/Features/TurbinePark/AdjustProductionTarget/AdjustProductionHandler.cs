using WindPark.Infrastructure.Repositories;
using WindPark.Shared;

namespace WindPark.Features.TurbinePark.AdjustProductionTarget;

public class AdjustProductionTargetHandler(ITurbineParkRepository turbineParkRepository, ILogger<AdjustProductionTargetHandler> logger)
{
    public async Task<Result<AdjustProductionResponse>> Handle(AdjustProductionCommand adjustProductionCommand)
    {
        logger.LogInformation("Updating target by {Delta}MW", adjustProductionCommand.Delta);

        try
        {
            var park = await turbineParkRepository.GetTurbineParkState();
            
            var updatedPark = park.AdjustProductionTarget(adjustProductionCommand.Delta);
            
            var operation = "AdjustProductionTarget";
            var details = $"Delta: {adjustProductionCommand.Delta:+#;-#;0}MW, Target: {park.ProductionTarget}MW → {updatedPark.ProductionTarget}MW";
            await turbineParkRepository.UpdateStateAsync(updatedPark, operation, details);
            
            var summary = updatedPark.GetProductionSummary();
            
            var message = adjustProductionCommand.Delta > 0 
                ? $"Target increased by {adjustProductionCommand.Delta}MW to {updatedPark.ProductionTarget}MW. Production: {summary.ActualProduction}MW"
                : $"Target decreased by {Math.Abs(adjustProductionCommand.Delta)}MW to {updatedPark.ProductionTarget}MW. Production: {summary.ActualProduction}MW";
            
            var response = new AdjustProductionResponse(message);

            return Result<AdjustProductionResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating target");
            return Result<AdjustProductionResponse>.Failure("Failed to update target");
        }
    }
}