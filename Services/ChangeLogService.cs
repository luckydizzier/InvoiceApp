using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;

namespace InvoiceApp.Services
{
    public class ChangeLogService : IChangeLogService
    {
        private readonly IChangeLogRepository _repository;

        public ChangeLogService(IChangeLogRepository repository)
        {
            _repository = repository;
        }

        public Task AddAsync(ChangeLog log) => _repository.AddAsync(log);

        public Task<ChangeLog?> GetLatestAsync() => _repository.GetLatestAsync();
    }
}
