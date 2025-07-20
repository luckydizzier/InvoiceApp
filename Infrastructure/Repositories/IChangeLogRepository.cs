using System.Threading.Tasks;
using InvoiceApp.Domain;
using InvoiceApp.Infrastructure.Data;

namespace InvoiceApp.Infrastructure.Repositories
{
    public interface IChangeLogRepository
    {
        Task AddAsync(ChangeLog log);
        Task AddAsync(ChangeLog log, InvoiceContext context);
        Task<ChangeLog?> GetLatestAsync();
    }
}
