using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using Serilog;

namespace InvoiceApp.Services
{
    public class ProductGroupService : BaseService<ProductGroup>, IProductGroupService
    {
        private readonly IProductGroupRepository _repository;

        public ProductGroupService(IProductGroupRepository repository, IChangeLogService logService)
            : base(repository, logService)
        {
            _repository = repository;
        }

        public Task<IEnumerable<ProductGroup>> GetAllAsync()
        {
            Log.Debug("ProductGroupService.GetAllAsync called");
            return _repository.GetAllAsync();
        }

        public Task<ProductGroup?> GetByIdAsync(int id)
        {
            Log.Debug("ProductGroupService.GetByIdAsync called with {Id}", id);
            return _repository.GetByIdAsync(id);
        }

        // SaveAsync and DeleteAsync provided by BaseService
    }
}
