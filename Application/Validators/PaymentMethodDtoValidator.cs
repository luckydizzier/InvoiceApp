using FluentValidation;
using InvoiceApp.Application.DTOs;

namespace InvoiceApp.Application.Validators
{
    public class PaymentMethodDtoValidator : AbstractValidator<PaymentMethodDto>
    {
        public PaymentMethodDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.DueInDays).GreaterThanOrEqualTo(0);
        }
    }
}
