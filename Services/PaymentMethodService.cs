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
        private readonly IPaymentMethodRepository _repository;
        private readonly IValidator<PaymentMethodDto> _validator;

        public PaymentMethodService(IPaymentMethodRepository repository, IChangeLogService logService, IValidator<PaymentMethodDto> validator)
            : base(repository, logService)
        {
            _repository = repository;
            _validator = validator;
        }

        public Task<IEnumerable<PaymentMethod>> GetAllAsync()
        {
            Log.Debug("PaymentMethodService.GetAllAsync called");
            return _repository.GetAllAsync();
        }

        public Task<PaymentMethod?> GetByIdAsync(int id)
        {
            Log.Debug("PaymentMethodService.GetByIdAsync called with {Id}", id);
            return _repository.GetByIdAsync(id);
        }

        protected override Task ValidateAsync(PaymentMethod entity)
        {
            return _validator.ValidateAndThrowAsync(entity.ToDto());
        }
    }
}
