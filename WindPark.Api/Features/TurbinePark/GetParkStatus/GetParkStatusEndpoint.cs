using WindPark.Shared;

namespace WindPark.Features.TurbinePark.GetParkStatus;
public class GetParkStatusEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/turbines");

        group.MapGet("/status", async (GetParkStatusHandler handler) =>
            {
                var result = await handler.Handle(new GetParkStatusQuery());

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { Error = result.Error });
            })
            .Produces<GetParkStatusResponse>();
    }
}