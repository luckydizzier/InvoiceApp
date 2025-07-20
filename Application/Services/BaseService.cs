using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using InvoiceApp.Domain;
using InvoiceApp.Infrastructure.Repositories;
using Serilog;

namespace InvoiceApp.Services
{
    public abstract class BaseService<T> where T : Base
    {
        protected readonly ICrudRepository<T> Repository;
        private readonly IChangeLogService _logService;

        protected BaseService(ICrudRepository<T> repository, IChangeLogService logService)
        {
            Repository = repository;
            _logService = logService;
        }

        protected virtual Task ValidateAsync(T entity) => Task.CompletedTask;

        public virtual Task<IEnumerable<T>> GetAllAsync()
        {
            Log.Debug("{Service}.GetAllAsync called", GetType().Name);
            return Repository.GetAllAsync();
        }

        public virtual Task<T?> GetByIdAsync(int id)
        {
            Log.Debug("{Service}.GetByIdAsync called with {Id}", GetType().Name, id);
            return Repository.GetByIdAsync(id);
        }

        public virtual async Task SaveAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            Log.Debug("{Service}.SaveAsync called for {Id}", typeof(T).Name, entity.Id);

            await ValidateAsync(entity);

            var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };

            if (entity.Id == 0)
            {
                entity.DateCreated = DateTime.Now;
                entity.DateUpdated = entity.DateCreated;
                entity.Active = true;
                await Repository.AddAsync(entity);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = typeof(T).Name,
                    Operation = "Add",
                    Data = JsonSerializer.Serialize(entity, options),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });
                Log.Information("{Entity} {Id} created", typeof(T).Name, entity.Id);
            }
            else
            {
                entity.DateUpdated = DateTime.Now;
                await Repository.UpdateAsync(entity);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = typeof(T).Name,
                    Operation = "Update",
                    Data = JsonSerializer.Serialize(entity, options),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });
                Log.Information("{Entity} {Id} updated", typeof(T).Name, entity.Id);
            }
        }

        public virtual async Task DeleteAsync(int id)
        {
            Log.Debug("{Service}.DeleteAsync called for {Id}", typeof(T).Name, id);
            await Repository.DeleteAsync(id);
            await _logService.AddAsync(new ChangeLog
            {
                Entity = typeof(T).Name,
                Operation = "Delete",
                Data = JsonSerializer.Serialize(new { Id = id }),
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Active = true
            });
            Log.Information("{Entity} {Id} deleted", typeof(T).Name, id);
        }
    }
}
