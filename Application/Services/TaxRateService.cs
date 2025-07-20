using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Domain;
using InvoiceApp.Infrastructure.Repositories;
using InvoiceApp.Application.DTOs;
using InvoiceApp.Application.Mappers;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Infrastructure.Data;
using FluentValidation;
using Serilog;

namespace InvoiceApp.Application.Services
{
    public class TaxRateService : BaseService<TaxRate>, ITaxRateService
    {
        private readonly IValidator<TaxRateDto> _validator;
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public TaxRateService(
            ITaxRateRepository repository,
            IChangeLogService logService,
            IValidator<TaxRateDto> validator,
            IDbContextFactory<InvoiceContext> contextFactory)
            : base(repository, logService)
        {
            _validator = validator;
            _contextFactory = contextFactory;
        }

        protected override async Task ValidateAsync(TaxRate entity)
        {
            await _validator.ValidateAndThrowAsync(entity.ToDto());

            if (entity.Percentage > 100m)
            {
                throw new BusinessRuleViolationException("Tax rate percentage cannot exceed 100.");
            }

            using var ctx = _contextFactory.CreateDbContext();

            if (entity.Id != 0)
            {
                var used = await ctx.InvoiceItems.AnyAsync(i => i.TaxRateId == entity.Id) ||
                           await ctx.Products.AnyAsync(p => p.TaxRateId == entity.Id);

                if (used)
                {
                    var existing = await ctx.TaxRates.AsNoTracking().FirstOrDefaultAsync(t => t.Id == entity.Id);
                    if (existing != null && (
                        existing.Percentage != entity.Percentage ||
                        existing.EffectiveFrom != entity.EffectiveFrom ||
                        existing.EffectiveTo != entity.EffectiveTo ||
                        !string.Equals(existing.Name, entity.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new BusinessRuleViolationException("Tax rate is immutable once used.");
                    }
                }
            }
        }

        public override async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var used = await ctx.InvoiceItems.AnyAsync(i => i.TaxRateId == id) ||
                       await ctx.Products.AnyAsync(p => p.TaxRateId == id);
            if (used)
            {
                throw new BusinessRuleViolationException("Tax rate is referenced and cannot be deleted.");
            }

            await base.DeleteAsync(id);
        }
    }
}
