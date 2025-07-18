using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using InvoiceApp.DTOs;
using InvoiceApp.Mappers;
using FluentValidation;
using Serilog;

namespace InvoiceApp.Services
{
    public class PaymentMethodService : BaseService<PaymentMethod>, IPaymentMethodService
    {
        private readonly IValidator<PaymentMethodDto> _validator;

        public PaymentMethodService(IPaymentMethodRepository repository, IChangeLogService logService, IValidator<PaymentMethodDto> validator)
            : base(repository, logService)
        {
            _validator = validator;
        }

        protected override Task ValidateAsync(PaymentMethod entity)
        {
            return _validator.ValidateAndThrowAsync(entity.ToDto());
        }
    }
}
