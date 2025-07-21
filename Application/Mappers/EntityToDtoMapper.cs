using InvoiceApp.Domain;
using InvoiceApp.Application.DTOs;
using System;
using System.Linq;
using System.Collections.Generic;

namespace InvoiceApp.Application.Mappers
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

        public static InvoiceDisplayDto ToDisplayDto(this Invoice entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new InvoiceDisplayDto
            {
                Id = entity.Id,
                Number = entity.Number,
                Issuer = entity.Issuer,
                Date = entity.Date,
                Amount = entity.Amount,
                Supplier = entity.Supplier?.ToDto(),
                PaymentMethod = entity.PaymentMethod?.ToDto(),
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

        public static Invoice ToEntity(this InvoiceDisplayDto dto)
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
                Supplier = dto.Supplier?.ToEntity(),
                PaymentMethodId = dto.PaymentMethodId,
                PaymentMethod = dto.PaymentMethod?.ToEntity(),
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

        public static ProductDto ToDto(this Product entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new ProductDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Net = entity.Net,
                Gross = entity.Gross,
                UnitId = entity.UnitId,
                ProductGroupId = entity.ProductGroupId,
                TaxRateId = entity.TaxRateId,
                IsLocked = entity.IsLocked
            };
        }

        public static Product ToEntity(this ProductDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new Product
            {
                Id = dto.Id,
                Name = dto.Name,
                Net = dto.Net,
                Gross = dto.Gross,
                UnitId = dto.UnitId,
                ProductGroupId = dto.ProductGroupId,
                TaxRateId = dto.TaxRateId,
                IsLocked = dto.IsLocked
            };
        }

        public static ProductGroupDto ToDto(this ProductGroup entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new ProductGroupDto
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        public static ProductGroup ToEntity(this ProductGroupDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new ProductGroup
            {
                Id = dto.Id,
                Name = dto.Name
            };
        }

        public static UnitDto ToDto(this Unit entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new UnitDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name
            };
        }

        public static Unit ToEntity(this UnitDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new Unit
            {
                Id = dto.Id,
                Code = dto.Code,
                Name = dto.Name
            };
        }

        public static PaymentMethodDto ToDto(this PaymentMethod entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new PaymentMethodDto
            {
                Id = entity.Id,
                Name = entity.Name,
                DueInDays = entity.DueInDays
            };
        }

        public static PaymentMethod ToEntity(this PaymentMethodDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new PaymentMethod
            {
                Id = dto.Id,
                Name = dto.Name,
                DueInDays = dto.DueInDays
            };
        }

        public static ChangeLogDto ToDto(this ChangeLog entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new ChangeLogDto
            {
                Id = entity.Id,
                Entity = entity.Entity,
                Operation = entity.Operation,
                Data = entity.Data,
                DateCreated = entity.DateCreated
            };
        }

        public static ChangeLog ToEntity(this ChangeLogDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new ChangeLog
            {
                Id = dto.Id,
                Entity = dto.Entity,
                Operation = dto.Operation,
                Data = dto.Data,
                DateCreated = dto.DateCreated
            };
        }
    }
}
