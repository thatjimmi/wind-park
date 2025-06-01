using WindPark.Infrastructure.Repositories;
using WindPark.Shared;

namespace WindPark.Features.TurbinePark.SetMarketPrice;

public class SetMarketPriceHandler(
    ITurbineParkRepository turbineParkRepository,
    ILogger<SetMarketPriceHandler> logger)
{
    public async Task<Result<SetMarketPriceResponse>> Handle(SetMarketPriceCommand setMarketPriceCommand)
    {
        logger.LogInformation("Updating price to {Price}€/MW", setMarketPriceCommand.Price);

        try
        {
            var park = await turbineParkRepository.GetTurbineParkState();
            
            var updatedPark = park.SetMarketPrice(setMarketPriceCommand.Price);
            
            var operation = "SetMarketPrice";
            var details = $"Price: {park.MarketPrice:F0}€ → {setMarketPriceCommand.Price:F0}€";
            await turbineParkRepository.UpdateStateAsync(updatedPark, operation, details);
            
            var summary = updatedPark.GetProductionSummary();
            
            var message = $"Price updated to {setMarketPriceCommand.Price:F0}€/MW. Production: {summary.ActualProduction}MW";
            
            var response = new SetMarketPriceResponse(message);

            return Result<SetMarketPriceResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating price");
            return Result<SetMarketPriceResponse>.Failure("Failed to update price");
        }
    }
}