using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using Serilog;

namespace InvoiceApp.Services
{
    public class SupplierService : BaseService<Supplier>, ISupplierService
    {
        private readonly ISupplierRepository _repository;

        public SupplierService(ISupplierRepository repository, IChangeLogService logService)
            : base(repository, logService)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Supplier>> GetAllAsync()
        {
            Log.Debug("SupplierService.GetAllAsync called");
            return _repository.GetAllAsync();
        }

        public Task<Supplier?> GetByIdAsync(int id)
        {
            Log.Debug("SupplierService.GetByIdAsync called with {Id}", id);
            return _repository.GetByIdAsync(id);
        }

        // SaveAsync and DeleteAsync provided by BaseService
    }
}
