using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;

namespace InvoiceApp.Services
{
    public class ProductGroupService : IProductGroupService
    {
        private readonly IProductGroupRepository _repository;
        private readonly IChangeLogService _logService;

        public ProductGroupService(IProductGroupRepository repository, IChangeLogService logService)
        {
            _repository = repository;
            _logService = logService;
        }

        public Task<IEnumerable<ProductGroup>> GetAllAsync() => _repository.GetAllAsync();

        public Task<ProductGroup?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

        public async Task SaveAsync(ProductGroup group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));

            if (group.Id == 0)
            {
                group.DateCreated = DateTime.Now;
                group.DateUpdated = group.DateCreated;
                group.Active = true;
                await _repository.AddAsync(group);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(ProductGroup),
                    Operation = "Add",
                    Data = JsonSerializer.Serialize(group),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });
            }
            else
            {
                group.DateUpdated = DateTime.Now;
                await _repository.UpdateAsync(group);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(ProductGroup),
                    Operation = "Update",
                    Data = JsonSerializer.Serialize(group),
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
                Entity = nameof(ProductGroup),
                Operation = "Delete",
                Data = JsonSerializer.Serialize(new { Id = id }),
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Active = true
            });
        }
    }
}
