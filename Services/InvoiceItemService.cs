using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using InvoiceApp.DTOs;
using InvoiceApp.Mappers;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Data;
using FluentValidation;
using Serilog;

namespace InvoiceApp.Services
{
    public class InvoiceItemService : BaseService<InvoiceItem>, IInvoiceItemService
    {
        private readonly IValidator<InvoiceItemDto> _validator;
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public InvoiceItemService(
            IInvoiceItemRepository repository,
            IChangeLogService logService,
            IValidator<InvoiceItemDto> validator,
            IDbContextFactory<InvoiceContext> contextFactory)
            : base(repository, logService)
        {
            _validator = validator;
            _contextFactory = contextFactory;
        }

        protected override async Task ValidateAsync(InvoiceItem entity)
        {
            await _validator.ValidateAndThrowAsync(entity.ToDto());

            using var ctx = _contextFactory.CreateDbContext();
            var productExists = await ctx.Products.AnyAsync(p => p.Id == entity.ProductId);
            if (!productExists)
            {
                throw new BusinessRuleViolationException("Invalid product reference.");
            }
            var rateExists = await ctx.TaxRates.AnyAsync(t => t.Id == entity.TaxRateId);
            if (!rateExists)
            {
                throw new BusinessRuleViolationException("Invalid tax rate reference.");
            }
        }

        // CRUD methods provided by BaseService
    }
}
