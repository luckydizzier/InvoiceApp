using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using InvoiceApp.DTOs;
using InvoiceApp.Mappers;
using FluentValidation;
using Serilog;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Data;

namespace InvoiceApp.Services
{
    public class InvoiceService : BaseService<Invoice>, IInvoiceService
    {
        private readonly IInvoiceRepository _repository;
        private readonly IValidator<InvoiceDto> _validator;
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public InvoiceService(IInvoiceRepository repository, IChangeLogService logService, IValidator<InvoiceDto> validator, IDbContextFactory<InvoiceContext> contextFactory)
            : base(repository, logService)
        {
            _repository = repository;
            _validator = validator;
            _contextFactory = contextFactory;
        }


        public Task<Invoice?> GetLatestForSupplierAsync(int supplierId)
        {
            Log.Debug("InvoiceService.GetLatestForSupplierAsync called with {SupplierId}", supplierId);
            return _repository.GetLatestForSupplierAsync(supplierId);
        }

        public Task<Invoice?> GetLatestAsync()
        {
            Log.Debug("InvoiceService.GetLatestAsync called");
            return _repository.GetLatestAsync();
        }

        protected override Task ValidateAsync(Invoice entity)
        {
            return _validator.ValidateAndThrowAsync(entity.ToDto());
        }

        public bool IsValid(Invoice invoice)
        {
            return _validator.Validate(invoice.ToDto()).IsValid;
        }

        public async Task SaveInvoiceWithItemsAsync(Invoice invoice, IEnumerable<InvoiceItem> items)
        {
            if (invoice == null) throw new ArgumentNullException(nameof(invoice));
            if (items == null) throw new ArgumentNullException(nameof(items));

            await ValidateAsync(invoice);

            using var ctx = _contextFactory.CreateDbContext();
            using var trx = await ctx.Database.BeginTransactionAsync();
            try
            {
                if (invoice.Supplier != null)
                {
                    if (invoice.Supplier.Id == 0)
                    {
                        invoice.Supplier.DateCreated = DateTime.Now;
                        invoice.Supplier.DateUpdated = invoice.Supplier.DateCreated;
                        invoice.Supplier.Active = true;
                        await ctx.Suppliers.AddAsync(invoice.Supplier);
                    }
                    else
                    {
                        ctx.Suppliers.Update(invoice.Supplier);
                    }
                }

                if (invoice.PaymentMethod != null)
                {
                    ctx.Attach(invoice.PaymentMethod);
                }

                if (invoice.Id == 0)
                {
                    invoice.DateCreated = DateTime.Now;
                    invoice.DateUpdated = invoice.DateCreated;
                    invoice.Active = true;
                    await ctx.Invoices.AddAsync(invoice);
                }
                else
                {
                    invoice.DateUpdated = DateTime.Now;
                    ctx.Invoices.Update(invoice);
                }

                foreach (var item in items)
                {
                    if (item.Product != null)
                    {
                        ctx.Attach(item.Product);
                    }
                    if (item.TaxRate != null)
                    {
                        ctx.Attach(item.TaxRate);
                    }
                    item.Invoice = invoice;
                    item.InvoiceId = invoice.Id;

                    if (item.Id == 0)
                    {
                        item.DateCreated = DateTime.Now;
                        item.DateUpdated = item.DateCreated;
                        item.Active = true;
                        await ctx.InvoiceItems.AddAsync(item);
                    }
                    else
                    {
                        item.DateUpdated = DateTime.Now;
                        ctx.InvoiceItems.Update(item);
                    }
                }

                await ctx.SaveChangesAsync();
                await trx.CommitAsync();
            }
            catch
            {
                await trx.RollbackAsync();
                throw;
            }
        }
    }
}
