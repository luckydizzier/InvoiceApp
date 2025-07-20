using FluentValidation;
using InvoiceApp.Application.DTOs;

namespace InvoiceApp.Application.Validators
{
    public class SupplierDtoValidator : AbstractValidator<SupplierDto>
    {
        public SupplierDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Address).NotEmpty().When(x => x.Address != null);
            RuleFor(x => x.TaxId).NotEmpty().When(x => x.TaxId != null);
        }
    }
}
