using FluentValidation;
using InvoiceApp.DTOs;

namespace InvoiceApp.Validators
{
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        public ProductDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Net).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Gross).GreaterThanOrEqualTo(0);
            RuleFor(x => x.UnitId).GreaterThan(0);
            RuleFor(x => x.ProductGroupId).GreaterThan(0);
            RuleFor(x => x.TaxRateId).GreaterThan(0);
        }
    }
}
