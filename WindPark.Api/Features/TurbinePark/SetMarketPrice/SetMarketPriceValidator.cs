using FluentValidation;

namespace WindPark.Features.TurbinePark.SetMarketPrice;

public class SetMarketPriceValidator : AbstractValidator<SetMarketPriceCommand>
{
    public SetMarketPriceValidator()
    {
        RuleFor(x => x.Price)
            .NotEmpty()
            .WithMessage("Price is required")
            .Must(price => decimal.TryParse(price.ToString(), out _));
    }
}