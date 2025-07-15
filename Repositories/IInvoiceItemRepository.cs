using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;

namespace InvoiceApp.Repositories
{
    public interface IInvoiceItemRepository
    {
        Task<IEnumerable<InvoiceItem>> GetAllAsync();
        Task<InvoiceItem?> GetByIdAsync(int id);
        Task AddAsync(InvoiceItem item);
        Task UpdateAsync(InvoiceItem item);
        Task DeleteAsync(int id);
    }
}
