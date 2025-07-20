using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Domain;

namespace InvoiceApp.Services
{
    public interface IProductGroupService
    {
        Task<IEnumerable<ProductGroup>> GetAllAsync();
        Task<ProductGroup?> GetByIdAsync(int id);
        Task SaveAsync(ProductGroup group);
        Task DeleteAsync(int id);
    }
}
