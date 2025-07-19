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
    public class ProductGroupService : BaseService<ProductGroup>, IProductGroupService
    {
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public ProductGroupService(
            IProductGroupRepository repository,
            IChangeLogService logService,
            IDbContextFactory<InvoiceContext> contextFactory)
            : base(repository, logService)
        {
            _contextFactory = contextFactory;
        }

        protected override async Task ValidateAsync(ProductGroup entity)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var nameExists = await ctx.ProductGroups
                .AnyAsync(g => g.Id != entity.Id &&
                    g.Name.ToLowerInvariant() == entity.Name.ToLowerInvariant());
            if (nameExists)
            {
                throw new BusinessRuleViolationException($"Product group '{entity.Name}' already exists.");
            }
        }

        public override async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var used = await ctx.Products.AnyAsync(p => p.ProductGroupId == id);
            if (used)
            {
                throw new BusinessRuleViolationException("Product group is referenced by products and cannot be deleted.");
            }

            await base.DeleteAsync(id);
        }
    }
}
