using FluentValidation;
using InvoiceApp.Application.DTOs;

namespace InvoiceApp.Application.Validators
{
    public class UnitDtoValidator : AbstractValidator<UnitDto>
    {
        public UnitDtoValidator()
        {
            RuleFor(x => x.Code).NotEmpty().When(x => x.Code != null);
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
