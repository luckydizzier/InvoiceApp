using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using Serilog;

namespace InvoiceApp.Services
{
    public class TaxRateService : ITaxRateService
    {
        private readonly ITaxRateRepository _repository;
        private readonly IChangeLogService _logService;

        public TaxRateService(ITaxRateRepository repository, IChangeLogService logService)
        {
            _repository = repository;
            _logService = logService;
        }

        public Task<IEnumerable<TaxRate>> GetAllAsync()
        {
            Log.Debug("TaxRateService.GetAllAsync called");
            return _repository.GetAllAsync();
        }

        public Task<TaxRate?> GetByIdAsync(int id)
        {
            Log.Debug("TaxRateService.GetByIdAsync called with {Id}", id);
            return _repository.GetByIdAsync(id);
        }

        public async Task SaveAsync(TaxRate rate)
        {
            if (rate == null) throw new ArgumentNullException(nameof(rate));
            Log.Debug("TaxRateService.SaveAsync called for {Id}", rate.Id);

            if (rate.Id == 0)
            {
                rate.DateCreated = DateTime.Now;
                rate.DateUpdated = rate.DateCreated;
                rate.Active = true;
                await _repository.AddAsync(rate);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(TaxRate),
                    Operation = "Add",
                    Data = JsonSerializer.Serialize(rate),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });
                Log.Information("TaxRate {Id} created", rate.Id);
            }
            else
            {
                rate.DateUpdated = DateTime.Now;
                await _repository.UpdateAsync(rate);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(TaxRate),
                    Operation = "Update",
                    Data = JsonSerializer.Serialize(rate),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });
                Log.Information("TaxRate {Id} updated", rate.Id);
            }
        }

        public async Task DeleteAsync(int id)
        {
            Log.Debug("TaxRateService.DeleteAsync called for {Id}", id);
            await _repository.DeleteAsync(id);
            await _logService.AddAsync(new ChangeLog
            {
                Entity = nameof(TaxRate),
                Operation = "Delete",
                Data = JsonSerializer.Serialize(new { Id = id }),
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Active = true
            });
            Log.Information("TaxRate {Id} deleted", id);
        }
    }
}
