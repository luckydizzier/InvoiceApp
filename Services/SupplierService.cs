using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using Serilog;

namespace InvoiceApp.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _repository;
        private readonly IChangeLogService _logService;

        public SupplierService(ISupplierRepository repository, IChangeLogService logService)
        {
            _repository = repository;
            _logService = logService;
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

        public async Task SaveAsync(Supplier supplier)
        {
            if (supplier == null) throw new ArgumentNullException(nameof(supplier));
            Log.Debug("SupplierService.SaveAsync called for {Id}", supplier.Id);

            if (supplier.Id == 0)
            {
                supplier.DateCreated = DateTime.Now;
                supplier.DateUpdated = supplier.DateCreated;
                supplier.Active = true;
                await _repository.AddAsync(supplier);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(Supplier),
                    Operation = "Add",
                    Data = JsonSerializer.Serialize(supplier),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });
                Log.Information("Supplier {Id} created", supplier.Id);
            }
            else
            {
                supplier.DateUpdated = DateTime.Now;
                await _repository.UpdateAsync(supplier);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(Supplier),
                    Operation = "Update",
                    Data = JsonSerializer.Serialize(supplier),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });
                Log.Information("Supplier {Id} updated", supplier.Id);
            }
        }

        public async Task DeleteAsync(int id)
        {
            Log.Debug("SupplierService.DeleteAsync called for {Id}", id);
            await _repository.DeleteAsync(id);
            await _logService.AddAsync(new ChangeLog
            {
                Entity = nameof(Supplier),
                Operation = "Delete",
                Data = JsonSerializer.Serialize(new { Id = id }),
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Active = true
            });
            Log.Information("Supplier {Id} deleted", id);
        }
    }
}
