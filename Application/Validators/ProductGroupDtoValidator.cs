using FluentValidation;
using InvoiceApp.DTOs;

namespace InvoiceApp.Validators
{
    public class ProductGroupDtoValidator : AbstractValidator<ProductGroupDto>
    {
        public ProductGroupDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
