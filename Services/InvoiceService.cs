using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using System.Text.Json;
using System.Text.Json.Serialization;
using InvoiceApp.DTOs;
using InvoiceApp.Mappers;
using FluentValidation;
using Serilog;

namespace InvoiceApp.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _repository;
        private readonly IChangeLogService _logService;
        private readonly IValidator<InvoiceDto> _validator;

        public InvoiceService(IInvoiceRepository repository, IChangeLogService logService, IValidator<InvoiceDto> validator)
        {
            _repository = repository;
            _logService = logService;
            _validator = validator;
        }

        public Task<IEnumerable<Invoice>> GetAllAsync()
        {
            Log.Debug("InvoiceService.GetAllAsync called");
            return _repository.GetAllAsync();
        }

        public Task<Invoice?> GetByIdAsync(int id)
        {
            Log.Debug("InvoiceService.GetByIdAsync called with {Id}", id);
            return _repository.GetByIdAsync(id);
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

        public async Task SaveAsync(Invoice invoice)
        {
            if (invoice == null) throw new ArgumentNullException(nameof(invoice));
            Log.Debug("InvoiceService.SaveAsync called for {Id}", invoice.Id);

            await _validator.ValidateAndThrowAsync(invoice.ToDto());

            if (invoice.Id == 0)
            {
                invoice.DateCreated = DateTime.Now;
                invoice.DateUpdated = invoice.DateCreated;
                invoice.Active = true;
                await _repository.AddAsync(invoice);
                var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(Invoice),
                    Operation = "Add",
                    Data = JsonSerializer.Serialize(invoice, options),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });
                Log.Information("Invoice {Id} created", invoice.Id);
            }
            else
            {
                invoice.DateUpdated = DateTime.Now;
                await _repository.UpdateAsync(invoice);
                var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(Invoice),
                    Operation = "Update",
                    Data = JsonSerializer.Serialize(invoice, options),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });
                Log.Information("Invoice {Id} updated", invoice.Id);
            }
        }

        public async Task DeleteAsync(int id)
        {
            Log.Debug("InvoiceService.DeleteAsync called for {Id}", id);
            await _repository.DeleteAsync(id);
            await _logService.AddAsync(new ChangeLog
            {
                Entity = nameof(Invoice),
                Operation = "Delete",
                Data = JsonSerializer.Serialize(new { Id = id }),
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Active = true
            });
            Log.Information("Invoice {Id} deleted", id);
        }
    }
}
