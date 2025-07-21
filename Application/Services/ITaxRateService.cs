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
        /// <summary>
        /// Returns an existing tax rate matching the percentage or creates a new one.
        /// </summary>
        /// <param name="percentage">Percentage value of the tax rate.</param>
        Task<TaxRate> EnsureTaxRateExistsAsync(decimal percentage);
    }
}
