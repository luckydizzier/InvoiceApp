using FluentValidation;
using InvoiceApp.Application.DTOs;

namespace InvoiceApp.Application.Validators
{
    public class ProductGroupDtoValidator : AbstractValidator<ProductGroupDto>
    {
        public ProductGroupDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
