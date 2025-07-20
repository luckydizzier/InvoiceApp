using System;
using FluentValidation;
using InvoiceApp.DTOs;

namespace InvoiceApp.Validators
{
    public class ChangeLogDtoValidator : AbstractValidator<ChangeLogDto>
    {
        public ChangeLogDtoValidator()
        {
            RuleFor(x => x.Entity).NotEmpty();
            RuleFor(x => x.Operation).NotEmpty();
            RuleFor(x => x.Data).NotEmpty();
            RuleFor(x => x.DateCreated).NotEqual(default(DateTime));
        }
    }
}
