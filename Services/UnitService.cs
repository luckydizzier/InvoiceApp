using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using Serilog;

namespace InvoiceApp.Services
{
    public class UnitService : BaseService<Unit>, IUnitService
    {
        private readonly IUnitRepository _repository;

        public UnitService(IUnitRepository repository, IChangeLogService logService)
            : base(repository, logService)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Unit>> GetAllAsync()
        {
            Log.Debug("UnitService.GetAllAsync called");
            return _repository.GetAllAsync();
        }

        public Task<Unit?> GetByIdAsync(int id)
        {
            Log.Debug("UnitService.GetByIdAsync called with {Id}", id);
            return _repository.GetByIdAsync(id);
        }

        // SaveAsync and DeleteAsync provided by BaseService
    }
}
