using FluentValidation;
using WindPark.Shared;

namespace WindPark.Features.TurbinePark.SetMarketPrice;

public class SetMarketPriceEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/turbines");

        group.MapPost("/market-price", async (
            SetMarketPriceCommand setMarketPriceCommand,
            SetMarketPriceHandler handler,
            IValidator<SetMarketPriceCommand> validator) =>
        {
            var validationResult = await validator.ValidateAsync(setMarketPriceCommand);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Results.BadRequest(ErrorResponse.Create("Invalid market price", errors));
            }

            var result = await handler.Handle(setMarketPriceCommand);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(result.Error, statusCode: 500);

        })
            .Produces<SetMarketPriceResponse>()
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }
}