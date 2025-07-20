using System.Threading.Tasks;
using InvoiceApp.Domain;
using InvoiceApp.Infrastructure.Repositories;
using InvoiceApp.Infrastructure.Data;
using Serilog;

namespace InvoiceApp.Services
{
    public class ChangeLogService : IChangeLogService
    {
        private readonly IChangeLogRepository _repository;

        public ChangeLogService(IChangeLogRepository repository)
        {
            _repository = repository;
        }

        public Task AddAsync(ChangeLog log)
        {
            Log.Debug("ChangeLogService.AddAsync called for {Entity}", log.Entity);
            return _repository.AddAsync(log);
        }

        public Task AddAsync(ChangeLog log, InvoiceContext context)
        {
            Log.Debug("ChangeLogService.AddAsync(scope) called for {Entity}", log.Entity);
            return _repository.AddAsync(log, context);
        }

        public Task<ChangeLog?> GetLatestAsync()
        {
            Log.Debug("ChangeLogService.GetLatestAsync called");
            return _repository.GetLatestAsync();
        }
    }
}
