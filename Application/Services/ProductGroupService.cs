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
    public class ProductGroupService : BaseService<ProductGroup>, IProductGroupService
    {
        private readonly IValidator<ProductGroupDto> _validator;
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public ProductGroupService(
            IProductGroupRepository repository,
            IChangeLogService logService,
            IValidator<ProductGroupDto> validator,
            IDbContextFactory<InvoiceContext> contextFactory)
            : base(repository, logService)
        {
            _validator = validator;
            _contextFactory = contextFactory;
        }

        protected override async Task ValidateAsync(ProductGroup entity)
        {
            await _validator.ValidateAndThrowAsync(entity.ToDto());

            using var ctx = _contextFactory.CreateDbContext();
            var nameExists = await ctx.ProductGroups
                .AnyAsync(g => g.Id != entity.Id && g.Name.ToLower() == entity.Name.ToLower());
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
