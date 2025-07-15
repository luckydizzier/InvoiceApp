using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;

namespace InvoiceApp.Services
{
    public interface IInvoiceService
    {
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task<Invoice?> GetByIdAsync(int id);
        Task<Invoice?> GetLatestForSupplierAsync(int supplierId);
        Task SaveAsync(Invoice invoice);
        Task DeleteAsync(int id);
    }
}
