using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;

namespace InvoiceApp.Repositories
{
    public interface IInvoiceItemRepository : ICrudRepository<InvoiceItem>
    {
        Task<IEnumerable<InvoiceItem>> GetAllAsync();
        Task<InvoiceItem?> GetByIdAsync(int id);
    }
}
