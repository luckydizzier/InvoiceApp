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
    public class InvoiceService : BaseService<Invoice>, IInvoiceService
    {
        private readonly IInvoiceRepository _repository;
        private readonly IValidator<InvoiceDto> _validator;

        public InvoiceService(IInvoiceRepository repository, IChangeLogService logService, IValidator<InvoiceDto> validator)
            : base(repository, logService)
        {
            _repository = repository;
            _validator = validator;
        }


        public Task<Invoice?> GetLatestForSupplierAsync(int supplierId)
        {
            Log.Debug("InvoiceService.GetLatestForSupplierAsync called with {SupplierId}", supplierId);
            return _repository.GetLatestForSupplierAsync(supplierId);
        }

        public Task<Invoice?> GetLatestAsync()
        {
            Log.Debug("InvoiceService.GetLatestAsync called");
            return _repository.GetLatestAsync();
        }

        protected override Task ValidateAsync(Invoice entity)
        {
            return _validator.ValidateAndThrowAsync(entity.ToDto());
        }
    }
}
