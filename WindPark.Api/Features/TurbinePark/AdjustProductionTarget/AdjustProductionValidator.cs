using FluentValidation;
using WindPark.Infrastructure.Repositories;

namespace WindPark.Features.TurbinePark.AdjustProductionTarget;

public class AdjustProductionValidator : AbstractValidator<AdjustProductionCommand>
{
    public AdjustProductionValidator(ITurbineParkRepository turbineParkRepository)
    {
        RuleFor(x => x.Delta)
            .NotEqual(0).WithMessage("Delta cannot be zero")
            .MustAsync(async (delta, ct) => await IsValidDelta(delta, turbineParkRepository))
            .WithMessage("Delta would result in invalid production target (below 0 or above max capacity)");
    }
    
    private static async Task<bool> IsValidDelta(decimal delta, ITurbineParkRepository parkRepository)
    {
        var turbinePark = await parkRepository.GetTurbineParkState();
        var maxCapacity = turbinePark.MaxCapacity;
        
        var newTarget = turbinePark.ProductionTarget + delta;
        return newTarget >= 0 && newTarget <= maxCapacity;
    }
}