using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Domain;

namespace InvoiceApp.Infrastructure.Repositories
{
    public interface ICrudRepository<T> where T : Base
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}
