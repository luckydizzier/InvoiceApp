using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using System.Text.Json;

namespace InvoiceApp.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _repository;
        private readonly IChangeLogService _logService;

        public InvoiceService(IInvoiceRepository repository, IChangeLogService logService)
        {
            _repository = repository;
            _logService = logService;
        }

        public Task<IEnumerable<Invoice>> GetAllAsync() => _repository.GetAllAsync();

        public Task<Invoice?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

        public Task<Invoice?> GetLatestForSupplierAsync(int supplierId) =>
            _repository.GetLatestForSupplierAsync(supplierId);

        public async Task SaveAsync(Invoice invoice)
        {
            if (invoice == null) throw new ArgumentNullException(nameof(invoice));

            if (invoice.Id == 0)
            {
                invoice.DateCreated = DateTime.Now;
                invoice.DateUpdated = invoice.DateCreated;
                invoice.Active = true;
                await _repository.AddAsync(invoice);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(Invoice),
                    Operation = "Add",
                    Data = JsonSerializer.Serialize(invoice),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });
            }
            else
            {
                invoice.DateUpdated = DateTime.Now;
                await _repository.UpdateAsync(invoice);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(Invoice),
                    Operation = "Update",
                    Data = JsonSerializer.Serialize(invoice),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });
            }
        }

        public async Task DeleteAsync(int id)
        {
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
        }
    }
}
