using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Data;
using Serilog;

namespace InvoiceApp.Services
{
    public class UnitService : BaseService<Unit>, IUnitService
    {
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public UnitService(IUnitRepository repository,
            IChangeLogService logService,
            IDbContextFactory<InvoiceContext> contextFactory)
            : base(repository, logService)
        {
            _contextFactory = contextFactory;
        }

        protected override async Task ValidateAsync(Unit entity)
        {
            using var ctx = _contextFactory.CreateDbContext();

            var nameExists = await ctx.Units
                .AnyAsync(u => u.Id != entity.Id &&
                    u.Name.ToLowerInvariant() == entity.Name.ToLowerInvariant());
            if (nameExists)
            {
                throw new BusinessRuleViolationException($"Unit name '{entity.Name}' already exists.");
            }

            if (!string.IsNullOrWhiteSpace(entity.Code))
            {
                var codeExists = await ctx.Units
                    .AnyAsync(u => u.Id != entity.Id && u.Code != null &&
                        u.Code.ToLowerInvariant() == entity.Code.ToLowerInvariant());
                if (codeExists)
                {
                    throw new BusinessRuleViolationException($"Unit code '{entity.Code}' already exists.");
                }
            }
        }

        public override async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var used = await ctx.Products.AnyAsync(p => p.UnitId == id);
            if (used)
            {
                throw new BusinessRuleViolationException("Unit is referenced by products and cannot be deleted.");
            }

            await base.DeleteAsync(id);
        }
    }
}
