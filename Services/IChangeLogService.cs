using System.Threading.Tasks;
using InvoiceApp.Models;

namespace InvoiceApp.Services
{
    public interface IChangeLogService
    {
        Task AddAsync(ChangeLog log);
        Task<ChangeLog?> GetLatestAsync();
    }
}
