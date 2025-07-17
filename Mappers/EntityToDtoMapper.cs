using InvoiceApp.Models;
using InvoiceApp.DTOs;
using System;
using System.Linq;
using System.Collections.Generic;

namespace InvoiceApp.Mappers
{
    public static class EntityToDtoMapper
    {
        public static InvoiceDto ToDto(this Invoice entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new InvoiceDto
            {
                Id = entity.Id,
                Number = entity.Number,
                Issuer = entity.Issuer,
                Date = entity.Date,
                Amount = entity.Amount,
                SupplierId = entity.SupplierId,
                PaymentMethodId = entity.PaymentMethodId,
                IsGross = entity.IsGross,
                Items = entity.Items?.Select(i => i.ToDto()).ToList() ?? new List<InvoiceItemDto>()
            };
        }

        public static InvoiceItemDto ToDto(this InvoiceItem entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new InvoiceItemDto
            {
                Id = entity.Id,
                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice,
                InvoiceId = entity.InvoiceId,
                ProductId = entity.ProductId,
                TaxRateId = entity.TaxRateId
            };
        }

        public static SupplierDto ToDto(this Supplier entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new SupplierDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Address = entity.Address,
                TaxId = entity.TaxId,
                BankAccntNr = entity.BankAccntNr
            };
        }

        public static TaxRateDto ToDto(this TaxRate entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new TaxRateDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Percentage = entity.Percentage,
                EffectiveFrom = entity.EffectiveFrom,
                EffectiveTo = entity.EffectiveTo
            };
        }

        public static Invoice ToEntity(this InvoiceDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new Invoice
            {
                Id = dto.Id,
                Number = dto.Number,
                Issuer = dto.Issuer,
                Date = dto.Date,
                Amount = dto.Amount,
                SupplierId = dto.SupplierId,
                PaymentMethodId = dto.PaymentMethodId,
                IsGross = dto.IsGross,
                Items = dto.Items?.Select(i => i.ToEntity()).ToList() ?? new List<InvoiceItem>()
            };
        }

        public static InvoiceItem ToEntity(this InvoiceItemDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new InvoiceItem
            {
                Id = dto.Id,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                InvoiceId = dto.InvoiceId,
                ProductId = dto.ProductId,
                TaxRateId = dto.TaxRateId
            };
        }

        public static Supplier ToEntity(this SupplierDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new Supplier
            {
                Id = dto.Id,
                Name = dto.Name,
                Address = dto.Address,
                TaxId = dto.TaxId,
                BankAccntNr = dto.BankAccntNr
            };
        }

        public static TaxRate ToEntity(this TaxRateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new TaxRate
            {
                Id = dto.Id,
                Name = dto.Name,
                Percentage = dto.Percentage,
                EffectiveFrom = dto.EffectiveFrom,
                EffectiveTo = dto.EffectiveTo
            };
        }
    }
}
