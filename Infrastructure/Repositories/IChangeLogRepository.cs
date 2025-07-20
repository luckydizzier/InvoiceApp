using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Data;

namespace InvoiceApp.Repositories
{
    public interface IChangeLogRepository
    {
        Task AddAsync(ChangeLog log);
        Task AddAsync(ChangeLog log, InvoiceContext context);
        Task<ChangeLog?> GetLatestAsync();
    }
}
