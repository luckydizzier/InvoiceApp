using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;

namespace InvoiceApp.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IChangeLogService _logService;

        public ProductService(IProductRepository repository, IChangeLogService logService)
        {
            _repository = repository;
            _logService = logService;
        }

        public Task<IEnumerable<Product>> GetAllAsync() => _repository.GetAllAsync();

        public Task<Product?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

        public async Task SaveAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            if (product.Id == 0)
            {
                product.DateCreated = DateTime.Now;
                product.DateUpdated = product.DateCreated;
                product.Active = true;
                await _repository.AddAsync(product);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(Product),
                    Operation = "Add",
                    Data = JsonSerializer.Serialize(product),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });
            }
            else
            {
                product.DateUpdated = DateTime.Now;
                await _repository.UpdateAsync(product);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(Product),
                    Operation = "Update",
                    Data = JsonSerializer.Serialize(product),
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
                Entity = nameof(Product),
                Operation = "Delete",
                Data = JsonSerializer.Serialize(new { Id = id }),
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Active = true
            });
        }
    }
}
