using FluentValidation;
using InvoiceApp.Application.DTOs;

namespace InvoiceApp.Application.Validators
{
    public class InvoiceItemDtoValidator : AbstractValidator<InvoiceItemDto>
    {
        public InvoiceItemDtoValidator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0);
            RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleFor(x => x.TaxRateId).GreaterThan(0);
        }
    }
}
