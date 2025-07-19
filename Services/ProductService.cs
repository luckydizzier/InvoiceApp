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
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public ProductService(
            IProductRepository repository,
            IChangeLogService logService,
            IDbContextFactory<InvoiceContext> contextFactory)
            : base(repository, logService)
        {
            _contextFactory = contextFactory;
        }

        protected override async Task ValidateAsync(Product entity)
        {
            using var ctx = _contextFactory.CreateDbContext();

            var nameExists = await ctx.Products
                .AnyAsync(p => p.Id != entity.Id && p.SupplierId == entity.SupplierId &&
                    p.Name.ToLowerInvariant() == entity.Name.ToLowerInvariant());
            if (nameExists)
            {
                throw new BusinessRuleViolationException($"Product '{entity.Name}' already exists for this supplier.");
            }

            if (!await ctx.Units.AnyAsync(u => u.Id == entity.UnitId))
            {
                throw new BusinessRuleViolationException("Invalid unit reference.");
            }
            if (!await ctx.TaxRates.AnyAsync(t => t.Id == entity.TaxRateId))
            {
                throw new BusinessRuleViolationException("Invalid tax rate reference.");
            }
            if (!await ctx.ProductGroups.AnyAsync(pg => pg.Id == entity.ProductGroupId))
            {
                throw new BusinessRuleViolationException("Invalid product group reference.");
            }

            if (entity.Id != 0)
            {
                var used = await ctx.InvoiceItems.AnyAsync(ii => ii.ProductId == entity.Id);
                if (used)
                {
                    var existing = await ctx.Products.AsNoTracking().FirstAsync(p => p.Id == entity.Id);
                    if (existing.UnitId != entity.UnitId ||
                        existing.TaxRateId != entity.TaxRateId ||
                        !string.Equals(existing.Name, entity.Name, StringComparison.Ordinal))
                    {
                        throw new BusinessRuleViolationException(
                            "Product is referenced in invoices and cannot change name, unit, or tax rate.");
                    }
                }
            }
        }

        public override async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var used = await ctx.InvoiceItems.AnyAsync(ii => ii.ProductId == id);
            if (used)
            {
                throw new BusinessRuleViolationException("Product is referenced by invoice items and cannot be deleted.");
            }

            await base.DeleteAsync(id);
        }
    }
}
