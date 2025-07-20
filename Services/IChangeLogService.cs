using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Data;

namespace InvoiceApp.Services
{
    public interface IChangeLogService
    {
        Task AddAsync(ChangeLog log);
        Task AddAsync(ChangeLog log, InvoiceContext context);
        Task<ChangeLog?> GetLatestAsync();
    }
}
