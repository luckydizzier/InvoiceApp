using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Domain;

namespace InvoiceApp.Application.Services
{
    public interface IInvoiceService
    {
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task<IEnumerable<Invoice>> GetHeadersAsync();
        Task<Invoice?> GetByIdAsync(int id);
        Task<Invoice?> GetDetailsAsync(int id);
        Task<Invoice?> GetLatestForSupplierAsync(int supplierId);
        Task<Invoice?> GetLatestAsync();
        Task SaveAsync(Invoice invoice);
        Task SaveInvoiceWithItemsAsync(Invoice invoice, IEnumerable<InvoiceItem> items);
        Task DeleteAsync(int id);
        Task<string> GetNextNumberAsync(int supplierId);
        bool IsValid(Invoice invoice);
        IEnumerable<VatSummary> CalculateVatSummary(Invoice invoice);
    }
}
