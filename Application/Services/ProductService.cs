using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Domain;
using InvoiceApp.Infrastructure.Repositories;
using InvoiceApp.DTOs;
using InvoiceApp.Mappers;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Infrastructure.Data;
using FluentValidation;
using Serilog;

namespace InvoiceApp.Services
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly IValidator<ProductDto> _validator;
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public ProductService(
            IProductRepository repository,
            IChangeLogService logService,
            IValidator<ProductDto> validator,
            IDbContextFactory<InvoiceContext> contextFactory)
            : base(repository, logService)
        {
            _validator = validator;
            _contextFactory = contextFactory;
        }

        protected override async Task ValidateAsync(Product entity)
        {
            await _validator.ValidateAndThrowAsync(entity.ToDto());

            using var ctx = _contextFactory.CreateDbContext();

            var nameExists = await ctx.Products
                .AnyAsync(p => p.Id != entity.Id && p.Name.ToLower() == entity.Name.ToLower());
            if (nameExists)
            {
                throw new BusinessRuleViolationException($"Product name '{entity.Name}' already exists.");
            }

            if (entity.Id != 0)
            {
                var hasHistory = await ctx.InvoiceItems.AnyAsync(i => i.ProductId == entity.Id);
                if (hasHistory)
                {
                    var existing = await ctx.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == entity.Id);
                    if (existing != null && (
                        existing.UnitId != entity.UnitId ||
                        existing.TaxRateId != entity.TaxRateId ||
                        !string.Equals(existing.Name, entity.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new BusinessRuleViolationException("Product cannot change Unit, TaxRate or Name once invoiced.");
                    }
                }
            }

            var rate = await ctx.TaxRates.FirstOrDefaultAsync(r => r.Id == entity.TaxRateId);
            var percent = rate?.Percentage ?? 0m;
            if (entity.Net == 0m && entity.Gross != 0m)
            {
                entity.Net = percent == 0m
                    ? entity.Gross
                    : Math.Round(entity.Gross / (1 + percent / 100m), 2);
            }
            else
            {
                entity.Gross = Math.Round(entity.Net * (1 + percent / 100m), 2);
            }
        }

        public override async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var used = await ctx.InvoiceItems.AnyAsync(i => i.ProductId == id);
            if (used)
            {
                throw new BusinessRuleViolationException("Product is referenced by invoice items and cannot be deleted.");
            }

            await base.DeleteAsync(id);
        }
    }
}
