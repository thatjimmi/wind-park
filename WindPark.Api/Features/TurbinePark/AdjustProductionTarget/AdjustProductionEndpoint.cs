using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WindPark.Shared;

namespace WindPark.Features.TurbinePark.AdjustProductionTarget;

public class AdjustProductionEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/turbines");

        group.MapPost("/production-target", async (
            [FromBody] AdjustProductionCommand adjustProductionCommand,
            AdjustProductionTargetHandler handler,
            IValidator<AdjustProductionCommand> validator) =>
        {
            var validationResult = await validator.ValidateAsync(adjustProductionCommand);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Results.BadRequest(ErrorResponse.Create("Invalid production target adjustment", errors));
            }

            var result = await handler.Handle(adjustProductionCommand);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(result.Error, statusCode: 500);
            
        })
            .Produces<AdjustProductionResponse>()
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }
}