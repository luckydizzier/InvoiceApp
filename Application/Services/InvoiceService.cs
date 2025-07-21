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
    }
}
