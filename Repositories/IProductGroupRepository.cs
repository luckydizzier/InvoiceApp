using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;

namespace InvoiceApp.Repositories
{
    public interface IProductGroupRepository
    {
        Task<IEnumerable<ProductGroup>> GetAllAsync();
        Task<ProductGroup?> GetByIdAsync(int id);
        Task AddAsync(ProductGroup group);
        Task UpdateAsync(ProductGroup group);
        Task DeleteAsync(int id);
    }
}
