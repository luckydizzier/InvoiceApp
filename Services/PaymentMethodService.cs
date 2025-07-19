using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using InvoiceApp.DTOs;
using InvoiceApp.Mappers;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Data;
using Serilog;

namespace InvoiceApp.Services
{
    public class PaymentMethodService : BaseService<PaymentMethod>, IPaymentMethodService
    {
        private readonly IValidator<PaymentMethodDto> _validator;
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public PaymentMethodService(
            IPaymentMethodRepository repository,
            IChangeLogService logService,
            IValidator<PaymentMethodDto> validator,
            IDbContextFactory<InvoiceContext> contextFactory)
            : base(repository, logService)
        {
            _validator = validator;
            _contextFactory = contextFactory;
        }

        protected override async Task ValidateAsync(PaymentMethod entity)
        {
            await _validator.ValidateAndThrowAsync(entity.ToDto());

            using var ctx = _contextFactory.CreateDbContext();
            var nameExists = await ctx.PaymentMethods
                .AnyAsync(p => p.Id != entity.Id &&
                    p.Name.ToLowerInvariant() == entity.Name.ToLowerInvariant());
            if (nameExists)
            {
                throw new BusinessRuleViolationException($"Payment method '{entity.Name}' already exists.");
            }
        }

        public override async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var used = await ctx.Invoices.AnyAsync(i => i.PaymentMethodId == id);
            if (used)
            {
                throw new BusinessRuleViolationException("Payment method is referenced by invoices and cannot be deleted.");
            }

            await base.DeleteAsync(id);
        }
    }
}
