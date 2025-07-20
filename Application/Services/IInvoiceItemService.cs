using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;

namespace InvoiceApp.Services
{
    public interface IInvoiceItemService
    {
        Task<IEnumerable<InvoiceItem>> GetAllAsync();
        Task<InvoiceItem?> GetByIdAsync(int id);
        Task SaveAsync(InvoiceItem item);
        Task DeleteAsync(int id);
    }
}
