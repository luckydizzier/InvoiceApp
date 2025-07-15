using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;

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

        public Task<IEnumerable<PaymentMethod>> GetAllAsync() => _repository.GetAllAsync();

        public Task<PaymentMethod?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

        public async Task SaveAsync(PaymentMethod method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

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
            }
        }

        public async Task DeleteAsync(int id)
        {
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
        }
    }
}
