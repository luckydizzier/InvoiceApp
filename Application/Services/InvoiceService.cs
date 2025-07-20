using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using InvoiceApp.DTOs;
using InvoiceApp.Mappers;
using FluentValidation;
using System.Linq;
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

        public async Task<string> GetNextNumberAsync(int supplierId)
        {
            Log.Debug("InvoiceService.GetNextNumberAsync called with {SupplierId}", supplierId);
            var latest = await GetLatestForSupplierAsync(supplierId);
            return IncrementNumber(latest?.Number);
        }

        private static string IncrementNumber(string? lastNumber)
        {
            if (string.IsNullOrWhiteSpace(lastNumber)) return "1";

            var digits = new string(lastNumber.Reverse().TakeWhile(char.IsDigit).Reverse().ToArray());
            if (digits.Length > 0 && int.TryParse(digits, out var n))
            {
                var prefix = lastNumber.Substring(0, lastNumber.Length - digits.Length);
                return prefix + (n + 1).ToString($"D{digits.Length}");
            }

            if (int.TryParse(lastNumber, out var value))
            {
                return (value + 1).ToString();
            }

            return lastNumber;
        }

        public Task<Invoice?> GetLatestAsync()
        {
            Log.Debug("InvoiceService.GetLatestAsync called");
            return _repository.GetLatestAsync();
        }

        public IEnumerable<VatSummary> CalculateVatSummary(Invoice invoice)
        {
            if (invoice.Items == null) return Enumerable.Empty<VatSummary>();

            var groups = invoice.Items
                .GroupBy(i => i.TaxRate?.Percentage ?? 0m)
                .Select(g =>
                {
                    decimal net = 0m;
                    decimal vat = 0m;
                    foreach (var item in g)
                    {
                        var amounts = Helpers.AmountCalculator.Calculate(
                            item.Quantity,
                            item.UnitPrice,
                            g.Key,
                            invoice.IsGross);
                        net += amounts.Net;
                        vat += amounts.Vat;
                    }
                    return new VatSummary { Rate = g.Key, Net = net, Vat = vat };
                });

            return groups.ToList();
        }

        protected override async Task ValidateAsync(Invoice entity)
        {
            await _validator.ValidateAndThrowAsync(entity.ToDto());

            using var ctx = _contextFactory.CreateDbContext();

            if (entity.Date > DateTime.Now)
            {
                throw new BusinessRuleViolationException("Invoice date cannot be in the future.");
            }

            var numberExists = await ctx.Invoices.AnyAsync(i => i.Id != entity.Id && i.SupplierId == entity.SupplierId && i.Number == entity.Number);
            if (numberExists)
            {
                throw new BusinessRuleViolationException($"Invoice number '{entity.Number}' already exists for this supplier.");
            }

            if (entity.Items == null || entity.Items.Count == 0)
            {
                throw new BusinessRuleViolationException("Invoice must contain at least one item.");
            }

            var breakdown = CalculateVatSummary(entity);
            entity.Amount = breakdown.Sum(v => v.Gross);
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
                var newProducts = new List<Product>();
                var updateProducts = new List<Product>();
                var newTaxRates = new List<TaxRate>();
                var updateTaxRates = new List<TaxRate>();
                var newItems = new List<InvoiceItem>();
                var updateItems = new List<InvoiceItem>();

                foreach (var item in items)
                {
                    if (item.Product != null)
                    {
                        if (item.Product.Id == 0)
                        {
                            item.Product.DateCreated = DateTime.Now;
                            item.Product.DateUpdated = item.Product.DateCreated;
                            item.Product.Active = true;
                            newProducts.Add(item.Product);
                        }
                        else
                        {
                            updateProducts.Add(item.Product);
                        }
                    }

                    if (item.TaxRate != null)
                    {
                        if (item.TaxRate.Id == 0)
                        {
                            item.TaxRate.DateCreated = DateTime.Now;
                            item.TaxRate.DateUpdated = item.TaxRate.DateCreated;
                            item.TaxRate.Active = true;
                            newTaxRates.Add(item.TaxRate);
                        }
                        else
                        {
                            updateTaxRates.Add(item.TaxRate);
                        }
                    }

                    item.Invoice = invoice;
                    item.InvoiceId = invoice.Id;

                    if (item.Id == 0)
                    {
                        item.DateCreated = DateTime.Now;
                        item.DateUpdated = item.DateCreated;
                        item.Active = true;
                        newItems.Add(item);
                        itemOps.Add((item, "Add"));
                    }
                    else
                    {
                        item.DateUpdated = DateTime.Now;
                        updateItems.Add(item);
                        itemOps.Add((item, "Update"));
                    }
                }

                await ctx.Products.AddRangeAsync(newProducts);
                ctx.Products.UpdateRange(updateProducts);

                await ctx.TaxRates.AddRangeAsync(newTaxRates.DistinctBy(t => t.Id));
                ctx.TaxRates.UpdateRange(updateTaxRates.DistinctBy(t => t.Id));

                await ctx.InvoiceItems.AddRangeAsync(newItems);
                ctx.InvoiceItems.UpdateRange(updateItems);

                var invoiceOp = invoice.Id == 0 ? "Add" : "Update";
                var supplierOp = invoice.Supplier != null ? (invoice.Supplier.Id == 0 ? "Add" : "Update") : string.Empty;

                await ctx.SaveChangesAsync();

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
                    }, ctx);
                }

                await _logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(Invoice),
                    Operation = invoiceOp,
                    Data = System.Text.Json.JsonSerializer.Serialize(invoice, options),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                }, ctx);

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
                    }, ctx);
                }

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
