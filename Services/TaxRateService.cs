using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using Serilog;

namespace InvoiceApp.Services
{
    public class TaxRateService : BaseService<TaxRate>, ITaxRateService
    {
        private readonly ITaxRateRepository _repository;

        public TaxRateService(ITaxRateRepository repository, IChangeLogService logService)
            : base(repository, logService)
        {
            _repository = repository;
        }

        public Task<IEnumerable<TaxRate>> GetAllAsync()
        {
            Log.Debug("TaxRateService.GetAllAsync called");
            return _repository.GetAllAsync();
        }

        public Task<TaxRate?> GetByIdAsync(int id)
        {
            Log.Debug("TaxRateService.GetByIdAsync called with {Id}", id);
            return _repository.GetByIdAsync(id);
        }

        // SaveAsync and DeleteAsync provided by BaseService
    }
}
