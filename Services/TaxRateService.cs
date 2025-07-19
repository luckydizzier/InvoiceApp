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
    public class TaxRateService : BaseService<TaxRate>, ITaxRateService
    {
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public TaxRateService(
            ITaxRateRepository repository,
            IChangeLogService logService,
            IDbContextFactory<InvoiceContext> contextFactory)
            : base(repository, logService)
        {
            _contextFactory = contextFactory;
        }

        protected override async Task ValidateAsync(TaxRate entity)
        {
            using var ctx = _contextFactory.CreateDbContext();

            if (entity.Percentage < 0 || entity.Percentage > 100)
            {
                throw new BusinessRuleViolationException("Tax rate percentage must be between 0 and 100.");
            }

            if (entity.EffectiveTo.HasValue && entity.EffectiveTo <= entity.EffectiveFrom)
            {
                throw new BusinessRuleViolationException("EffectiveTo must be after EffectiveFrom.");
            }

            if (entity.Id != 0)
            {
                var used = await ctx.Products.AnyAsync(p => p.TaxRateId == entity.Id) ||
                           await ctx.InvoiceItems.AnyAsync(i => i.TaxRateId == entity.Id);
                if (used)
                {
                    throw new BusinessRuleViolationException("Tax rate has been used and cannot be modified.");
                }
            }
        }

        public override async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var used = await ctx.Products.AnyAsync(p => p.TaxRateId == id) ||
                       await ctx.InvoiceItems.AnyAsync(i => i.TaxRateId == id);
            if (used)
            {
                throw new BusinessRuleViolationException("Tax rate is referenced and cannot be deleted.");
            }

            await base.DeleteAsync(id);
        }
    }
}
