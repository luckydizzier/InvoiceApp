using System;
using FluentValidation;
using InvoiceApp.DTOs;

namespace InvoiceApp.Validators
{
    public class InvoiceDtoValidator : AbstractValidator<InvoiceDto>
    {
        public InvoiceDtoValidator()
        {
            RuleFor(x => x.Number).NotEmpty();
            RuleFor(x => x.Issuer).NotEmpty();
            RuleFor(x => x.Date).NotEqual(default(DateTime));
            RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.SupplierId).GreaterThan(0);
            RuleFor(x => x.PaymentMethodId).GreaterThan(0);
            RuleForEach(x => x.Items).SetValidator(new InvoiceItemDtoValidator());
        }
    }
}
