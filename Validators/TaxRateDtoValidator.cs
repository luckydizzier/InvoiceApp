using System;
using FluentValidation;
using InvoiceApp.DTOs;

namespace InvoiceApp.Validators
{
    public class TaxRateDtoValidator : AbstractValidator<TaxRateDto>
    {
        public TaxRateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Percentage).GreaterThanOrEqualTo(0);
            RuleFor(x => x.EffectiveFrom).NotEqual(default(DateTime));
            RuleFor(x => x.EffectiveTo).GreaterThan(x => x.EffectiveFrom).When(x => x.EffectiveTo != null);
        }
    }
}
