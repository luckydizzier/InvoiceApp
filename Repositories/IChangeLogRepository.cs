using System.Threading.Tasks;
using InvoiceApp.Models;

namespace InvoiceApp.Repositories
{
    public interface IChangeLogRepository
    {
        Task AddAsync(ChangeLog log);
        Task<ChangeLog?> GetLatestAsync();
    }
}
