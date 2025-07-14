using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;

namespace InvoiceApp.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _repository;

        public InvoiceService(IInvoiceRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Invoice>> GetAllAsync() => _repository.GetAllAsync();

        public Task<Invoice?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

        public async Task SaveAsync(Invoice invoice)
        {
            if (invoice.Id == 0)
            {
                await _repository.AddAsync(invoice);
            }
            else
            {
                await _repository.UpdateAsync(invoice);
            }
        }

        public Task DeleteAsync(int id) => _repository.DeleteAsync(id);
    }
}
