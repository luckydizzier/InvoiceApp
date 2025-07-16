using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvoiceApp.Models;

namespace InvoiceApp.Repositories
{
    public class MockInvoiceRepository : IInvoiceRepository
    {
        private readonly List<Invoice> _storage = new();

        public Task<IEnumerable<Invoice>> GetAllAsync() => Task.FromResult<IEnumerable<Invoice>>(_storage);

        public Task<Invoice?> GetByIdAsync(int id)
        {
            var item = _storage.FirstOrDefault(i => i.Id == id);
            return Task.FromResult<Invoice?>(item);
        }

        public Task<Invoice?> GetLatestForSupplierAsync(int supplierId)
        {
            var item = _storage
                .Where(i => i.SupplierId == supplierId)
                .OrderByDescending(i => i.Number)
                .FirstOrDefault();
            return Task.FromResult<Invoice?>(item);
        }

        public Task<Invoice?> GetLatestAsync()
        {
            var item = _storage
                .OrderByDescending(i => i.DateCreated)
                .FirstOrDefault();
            return Task.FromResult<Invoice?>(item);
        }

        public Task AddAsync(Invoice invoice)
        {
            invoice.Id = _storage.Count + 1;
            _storage.Add(invoice);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Invoice invoice)
        {
            var existing = _storage.FirstOrDefault(i => i.Id == invoice.Id);
            if (existing != null)
            {
                existing.Number = invoice.Number;
                existing.Date = invoice.Date;
                existing.Amount = invoice.Amount;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var item = _storage.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                _storage.Remove(item);
            }
            return Task.CompletedTask;
        }
    }
}
