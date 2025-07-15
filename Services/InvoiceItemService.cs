using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using Serilog;

namespace InvoiceApp.Services
{
    public class InvoiceItemService : IInvoiceItemService
    {
        private readonly IInvoiceItemRepository _repository;
        private readonly IChangeLogService _logService;

        public InvoiceItemService(IInvoiceItemRepository repository, IChangeLogService logService)
        {
            _repository = repository;
            _logService = logService;
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

        public async Task SaveAsync(InvoiceItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            Log.Debug("InvoiceItemService.SaveAsync called for {Id}", item.Id);

            if (item.Id == 0)
            {
                item.DateCreated = DateTime.Now;
                item.DateUpdated = item.DateCreated;
                item.Active = true;
                await _repository.AddAsync(item);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(InvoiceItem),
                    Operation = "Add",
                    Data = JsonSerializer.Serialize(item),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });
                Log.Information("InvoiceItem {Id} created", item.Id);
            }
            else
            {
                item.DateUpdated = DateTime.Now;
                await _repository.UpdateAsync(item);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(InvoiceItem),
                    Operation = "Update",
                    Data = JsonSerializer.Serialize(item),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });
                Log.Information("InvoiceItem {Id} updated", item.Id);
            }
        }

        public async Task DeleteAsync(int id)
        {
            Log.Debug("InvoiceItemService.DeleteAsync called for {Id}", id);
            await _repository.DeleteAsync(id);
            await _logService.AddAsync(new ChangeLog
            {
                Entity = nameof(InvoiceItem),
                Operation = "Delete",
                Data = JsonSerializer.Serialize(new { Id = id }),
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Active = true
            });
            Log.Information("InvoiceItem {Id} deleted", id);
        }
    }
}
