using System.Threading.Tasks;
using InvoiceApp.Domain;
using InvoiceApp.Infrastructure.Data;

namespace InvoiceApp.Application.Services
{
    public interface IChangeLogService
    {
        Task AddAsync(ChangeLog log);
        Task AddAsync(ChangeLog log, InvoiceContext context);
        Task<ChangeLog?> GetLatestAsync();
    }
}
