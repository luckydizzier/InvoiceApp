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
    public class InvoiceItemService : BaseService<InvoiceItem>, IInvoiceItemService
    {
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public InvoiceItemService(
            IInvoiceItemRepository repository,
            IChangeLogService logService,
            IDbContextFactory<InvoiceContext> contextFactory)
            : base(repository, logService)
        {
            _contextFactory = contextFactory;
        }

        protected override async Task ValidateAsync(InvoiceItem entity)
        {
            using var ctx = _contextFactory.CreateDbContext();

            if (entity.Quantity == 0)
            {
                throw new BusinessRuleViolationException("Quantity must not be zero.");
            }

            if (entity.UnitPrice < 0)
            {
                throw new BusinessRuleViolationException("Unit price cannot be negative.");
            }

            if (!await ctx.Products.AnyAsync(p => p.Id == entity.ProductId))
            {
                throw new BusinessRuleViolationException("Invalid product reference.");
            }

            if (!await ctx.TaxRates.AnyAsync(t => t.Id == entity.TaxRateId))
            {
                throw new BusinessRuleViolationException("Invalid tax rate reference.");
            }
        }
    }
}
