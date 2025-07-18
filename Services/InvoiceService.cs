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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InvoiceApp.Services
{
    public class InvoiceService : BaseService<Invoice>, IInvoiceService
    {
        private readonly IInvoiceRepository _repository;
        private readonly IValidator<InvoiceDto> _validator;
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;
        private readonly IChangeLogService _logService;

        public InvoiceService(IInvoiceRepository repository, IChangeLogService logService, IValidator<InvoiceDto> validator, IDbContextFactory<InvoiceContext> contextFactory)
            : base(repository, logService)
        {
            _repository = repository;
            _validator = validator;
            _contextFactory = contextFactory;
            _logService = logService;
        }

        public Task<IEnumerable<Invoice>> GetHeadersAsync()
        {
            Log.Debug("InvoiceService.GetHeadersAsync called");
            return _repository.GetHeadersAsync();
        }

        public Task<Invoice?> GetDetailsAsync(int id)
        {
            Log.Debug("InvoiceService.GetDetailsAsync called for {Id}", id);
            return _repository.GetDetailsAsync(id);
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

                var itemOps = new List<(InvoiceItem Item, string Operation)>();
                foreach (var item in items)
                {
                    if (item.Product != null)
                    {
                        if (item.Product.Id == 0)
                        {
                            item.Product.DateCreated = DateTime.Now;
                            item.Product.DateUpdated = item.Product.DateCreated;
                            item.Product.Active = true;
                            await ctx.Products.AddAsync(item.Product);
                        }
                        else
                        {
                            ctx.Products.Update(item.Product);
                        }
                    }

                    if (item.TaxRate != null)
                    {
                        if (item.TaxRate.Id == 0)
                        {
                            item.TaxRate.DateCreated = DateTime.Now;
                            item.TaxRate.DateUpdated = item.TaxRate.DateCreated;
                            item.TaxRate.Active = true;
                            await ctx.TaxRates.AddAsync(item.TaxRate);
                        }
                        else
                        {
                            ctx.TaxRates.Update(item.TaxRate);
                        }
                    }

                    item.Invoice = invoice;
                    item.InvoiceId = invoice.Id;

                    string itemOp;
                    if (item.Id == 0)
                    {
                        item.DateCreated = DateTime.Now;
                        item.DateUpdated = item.DateCreated;
                        item.Active = true;
                        await ctx.InvoiceItems.AddAsync(item);
                        itemOp = "Add";
                    }
                    else
                    {
                        item.DateUpdated = DateTime.Now;
                        ctx.InvoiceItems.Update(item);
                        itemOp = "Update";
                    }

                    itemOps.Add((item, itemOp));
                }

                var invoiceOp = invoice.Id == 0 ? "Add" : "Update";
                var supplierOp = invoice.Supplier != null ? (invoice.Supplier.Id == 0 ? "Add" : "Update") : string.Empty;

                await ctx.SaveChangesAsync();
                await trx.CommitAsync();

                var options = new System.Text.Json.JsonSerializerOptions { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve };
                if (invoice.Supplier != null)
                {
                    await _logService.AddAsync(new ChangeLog
                    {
                        Entity = nameof(Supplier),
                        Operation = supplierOp,
                        Data = System.Text.Json.JsonSerializer.Serialize(invoice.Supplier, options),
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        Active = true
                    });
                }

                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(Invoice),
                    Operation = invoiceOp,
                    Data = System.Text.Json.JsonSerializer.Serialize(invoice, options),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });

                foreach (var (it, op) in itemOps)
                {
                    await _logService.AddAsync(new ChangeLog
                    {
                        Entity = nameof(InvoiceItem),
                        Operation = op,
                        Data = System.Text.Json.JsonSerializer.Serialize(it, options),
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        Active = true
                    });
                }
            }
            catch
            {
                await trx.RollbackAsync();
                throw;
            }
        }
    }
}
