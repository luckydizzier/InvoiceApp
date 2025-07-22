using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Domain;
using InvoiceApp.Infrastructure.Repositories;
using InvoiceApp.Application.DTOs;
using InvoiceApp.Application.Mappers;
using FluentValidation;
using System.Linq;
using Serilog;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Infrastructure.Data;
using InvoiceApp.Shared.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InvoiceApp.Application.Services
{
    public class InvoiceService : BaseService<Invoice>, IInvoiceService
    {
        private readonly IInvoiceRepository _repository;
        private readonly IValidator<InvoiceDto> _validator;
        private readonly IChangeLogService _logService;

        public InvoiceService(IInvoiceRepository repository, IChangeLogService logService, IValidator<InvoiceDto> validator)
            : base(repository, logService)
        {
            _repository = repository;
            _validator = validator;
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

        /// <summary>
        /// Groups invoice items by product, unit price and tax rate.
        /// Aggregated items have their quantity summed and flagged via <see cref="InvoiceItem.IsAggregated"/>.
        /// </summary>
        public IEnumerable<InvoiceItem> AggregateItems(IEnumerable<InvoiceItem> items, bool isGross)
        {
            var groups = items
                .GroupBy(i => new { i.ProductId, i.UnitPrice, Rate = i.TaxRate?.Percentage ?? 0m });

            foreach (var group in groups)
            {
                var first = group.First();
                var qty = group.Sum(i => i.Quantity);
                var amounts = AmountCalculator.Calculate(qty, first.UnitPrice, group.Key.Rate, isGross);

                yield return new InvoiceItem
                {
                    Id = first.Id,
                    Quantity = qty,
                    UnitPrice = first.UnitPrice,
                    InvoiceId = first.InvoiceId,
                    Invoice = first.Invoice,
                    ProductId = first.ProductId,
                    Product = first.Product,
                    TaxRateId = first.TaxRateId,
                    TaxRate = first.TaxRate,
                    IsAggregated = group.Count() > 1
                };
            }
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
                        var amounts = AmountCalculator.Calculate(
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

        public override async Task SaveAsync(Invoice entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using var ctx = _repository.CreateContext();
            await ValidateAsync(entity, ctx);

            var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };

            if (entity.Id == 0)
            {
                entity.DateCreated = DateTime.Now;
                entity.DateUpdated = entity.DateCreated;
                entity.Active = true;
                await _repository.SaveAsync(entity, ctx);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = typeof(Invoice).Name,
                    Operation = "Add",
                    Data = JsonSerializer.Serialize(entity, options),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                }, ctx);
                Log.Information("{Entity} {Id} created", typeof(Invoice).Name, entity.Id);
            }
            else
            {
                entity.DateUpdated = DateTime.Now;
                await _repository.SaveAsync(entity, ctx);
                await _logService.AddAsync(new ChangeLog
                {
                    Entity = typeof(Invoice).Name,
                    Operation = "Update",
                    Data = JsonSerializer.Serialize(entity, options),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                }, ctx);
                Log.Information("{Entity} {Id} updated", typeof(Invoice).Name, entity.Id);
            }
        }

        private async Task ValidateAsync(Invoice entity, InvoiceContext ctx)
        {
            await _validator.ValidateAndThrowAsync(entity.ToDto());

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
    }
}
