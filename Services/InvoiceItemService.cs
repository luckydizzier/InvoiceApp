using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using Serilog;

namespace InvoiceApp.Services
{
    public class InvoiceItemService : BaseService<InvoiceItem>, IInvoiceItemService
    {
        private readonly IInvoiceItemRepository _repository;

        public InvoiceItemService(IInvoiceItemRepository repository, IChangeLogService logService)
            : base(repository, logService)
        {
            _repository = repository;
        }

        public Task<IEnumerable<InvoiceItem>> GetAllAsync()
        {
            Log.Debug("InvoiceItemService.GetAllAsync called");
            return _repository.GetAllAsync();
        }

        public Task<InvoiceItem?> GetByIdAsync(int id)
        {
            Log.Debug("InvoiceItemService.GetByIdAsync called with {Id}", id);
            return _repository.GetByIdAsync(id);
        }

        // SaveAsync and DeleteAsync provided by BaseService
    }
}
