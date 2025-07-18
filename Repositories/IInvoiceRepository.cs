using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;

namespace InvoiceApp.Repositories
{
    public interface IInvoiceRepository : ICrudRepository<Invoice>
    {
        Task<Invoice?> GetLatestForSupplierAsync(int supplierId);
        Task<Invoice?> GetLatestAsync();
    }
}
