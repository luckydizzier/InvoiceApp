using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;

namespace InvoiceApp.Repositories
{
    public interface ITaxRateRepository : ICrudRepository<TaxRate>
    {
        Task<IEnumerable<TaxRate>> GetAllAsync();
        Task<TaxRate?> GetByIdAsync(int id);
    }
}
