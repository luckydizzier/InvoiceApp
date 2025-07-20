using FluentValidation;
using InvoiceApp.DTOs;

namespace InvoiceApp.Validators
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
