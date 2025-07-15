using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;

namespace InvoiceApp.Services
{
    public interface ITaxRateService
    {
        Task<IEnumerable<TaxRate>> GetAllAsync();
        Task<TaxRate?> GetByIdAsync(int id);
        Task SaveAsync(TaxRate rate);
        Task DeleteAsync(int id);
    }
}
