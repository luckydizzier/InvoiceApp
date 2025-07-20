using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Domain;

namespace InvoiceApp.Infrastructure.Repositories
{
    public interface IInvoiceRepository : ICrudRepository<Invoice>
    {
        Task<IEnumerable<Invoice>> GetHeadersAsync();
        Task<Invoice?> GetDetailsAsync(int id);
        Task<Invoice?> GetLatestForSupplierAsync(int supplierId);
        Task<Invoice?> GetLatestAsync();
    }
}
