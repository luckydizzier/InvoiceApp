using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Domain;
using InvoiceApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Infrastructure.Data;
using Serilog;

namespace InvoiceApp.Services
{
    public class SupplierService : BaseService<Supplier>, ISupplierService
    {
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public SupplierService(
            ISupplierRepository repository,
            IChangeLogService logService,
            IDbContextFactory<InvoiceContext> contextFactory)
            : base(repository, logService)
        {
            _contextFactory = contextFactory;
        }

        protected override async Task ValidateAsync(Supplier entity)
        {
            using var ctx = _contextFactory.CreateDbContext();

            var nameExists = await ctx.Suppliers
                .AnyAsync(s => s.Id != entity.Id && s.Name.ToLower() == entity.Name.ToLower());
            if (nameExists)
            {
                throw new BusinessRuleViolationException($"Supplier '{entity.Name}' already exists.");
            }

            if (!string.IsNullOrWhiteSpace(entity.TaxId))
            {
                // simple tax id format: at least 8 chars, digits or letters
                if (entity.TaxId.Length < 8)
                {
                    throw new BusinessRuleViolationException("Invalid Tax ID format.");
                }
            }
        }

        public override async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var hasInvoices = await ctx.Invoices.AnyAsync(i => i.SupplierId == id);
            if (hasInvoices)
            {
                throw new BusinessRuleViolationException("Supplier has invoices and cannot be deleted.");
            }

            await base.DeleteAsync(id);
        }
    }
}
