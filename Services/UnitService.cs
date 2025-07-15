using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;

namespace InvoiceApp.Services
{
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _repository;
        private readonly IChangeLogService _logService;

        public UnitService(IUnitRepository repository, IChangeLogService logService)
        {
            _repository = repository;
            _logService = logService;
        }

        public Task<IEnumerable<Unit>> GetAllAsync() => _repository.GetAllAsync();

        public Task<Unit?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

        public async Task SaveAsync(Unit unit)
        {
            if (unit == null) throw new ArgumentNullException(nameof(unit));

            if (unit.Id == 0)
            {
                unit.DateCreated = DateTime.Now;
                unit.DateUpdated = unit.DateCreated;
                unit.Active = true;
                await _repository.AddAsync(unit);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(Unit),
                    Operation = "Add",
                    Data = JsonSerializer.Serialize(unit),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });
            }
            else
            {
                unit.DateUpdated = DateTime.Now;
                await _repository.UpdateAsync(unit);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(Unit),
                    Operation = "Update",
                    Data = JsonSerializer.Serialize(unit),
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
                Entity = nameof(Unit),
                Operation = "Delete",
                Data = JsonSerializer.Serialize(new { Id = id }),
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Active = true
            });
        }
    }
}
