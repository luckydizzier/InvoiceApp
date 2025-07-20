using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Domain;

namespace InvoiceApp.Application.Services
{
    public interface ITaxRateService
    {
        Task<IEnumerable<TaxRate>> GetAllAsync();
        Task<TaxRate?> GetByIdAsync(int id);
        Task SaveAsync(TaxRate rate);
        Task DeleteAsync(int id);
    }
}
