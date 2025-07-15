using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using Serilog;

namespace InvoiceApp.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IPaymentMethodRepository _repository;
        private readonly IChangeLogService _logService;

        public PaymentMethodService(IPaymentMethodRepository repository, IChangeLogService logService)
        {
            _repository = repository;
            _logService = logService;
        }

        public Task<IEnumerable<PaymentMethod>> GetAllAsync()
        {
            Log.Debug("PaymentMethodService.GetAllAsync called");
            return _repository.GetAllAsync();
        }

        public Task<PaymentMethod?> GetByIdAsync(int id)
        {
            Log.Debug("PaymentMethodService.GetByIdAsync called with {Id}", id);
            return _repository.GetByIdAsync(id);
        }

        public async Task SaveAsync(PaymentMethod method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            Log.Debug("PaymentMethodService.SaveAsync called for {Id}", method.Id);

            if (method.Id == 0)
            {
                method.DateCreated = DateTime.Now;
                method.DateUpdated = method.DateCreated;
                method.Active = true;
                await _repository.AddAsync(method);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(PaymentMethod),
                    Operation = "Add",
                    Data = JsonSerializer.Serialize(method),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });
                Log.Information("PaymentMethod {Id} created", method.Id);
            }
            else
            {
                method.DateUpdated = DateTime.Now;
                await _repository.UpdateAsync(method);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(PaymentMethod),
                    Operation = "Update",
                    Data = JsonSerializer.Serialize(method),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });
                Log.Information("PaymentMethod {Id} updated", method.Id);
            }
        }

        public async Task DeleteAsync(int id)
        {
            Log.Debug("PaymentMethodService.DeleteAsync called for {Id}", id);
            await _repository.DeleteAsync(id);
            await _logService.AddAsync(new ChangeLog
            {
                Entity = nameof(PaymentMethod),
                Operation = "Delete",
                Data = JsonSerializer.Serialize(new { Id = id }),
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Active = true
            });
            Log.Information("PaymentMethod {Id} deleted", id);
        }
    }
}
