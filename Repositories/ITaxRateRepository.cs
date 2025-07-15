using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;

namespace InvoiceApp.Repositories
{
    public interface ITaxRateRepository
    {
        Task<IEnumerable<TaxRate>> GetAllAsync();
        Task<TaxRate?> GetByIdAsync(int id);
        Task AddAsync(TaxRate rate);
        Task UpdateAsync(TaxRate rate);
        Task DeleteAsync(int id);
    }
}
