using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;

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

        public Task<IEnumerable<Supplier>> GetAllAsync() => _repository.GetAllAsync();

        public Task<Supplier?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

        public async Task SaveAsync(Supplier supplier)
        {
            if (supplier == null) throw new ArgumentNullException(nameof(supplier));

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
            }
        }

        public async Task DeleteAsync(int id)
        {
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
        }
    }
}
