using System;
using FluentValidation;
using InvoiceApp.Application.DTOs;

namespace InvoiceApp.Application.Validators
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
            RuleFor(x => x.Items)
                .Must(items => items?.Count > 0)
                .WithMessage("Legal\u00e1bb egy t\u00e9tel sz\u00fcks\u00e9ges.");
            RuleForEach(x => x.Items).SetValidator(new InvoiceItemDtoValidator());
        }
    }
}
