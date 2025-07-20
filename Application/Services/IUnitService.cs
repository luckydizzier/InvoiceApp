using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Domain;

namespace InvoiceApp.Services
{
    public interface IUnitService
    {
        Task<IEnumerable<Unit>> GetAllAsync();
        Task<Unit?> GetByIdAsync(int id);
        Task SaveAsync(Unit unit);
        Task DeleteAsync(int id);
    }
}
