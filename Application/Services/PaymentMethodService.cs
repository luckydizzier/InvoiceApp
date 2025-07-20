using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Domain;
using InvoiceApp.Infrastructure.Repositories;
using InvoiceApp.Application.DTOs;
using InvoiceApp.Application.Mappers;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Infrastructure.Data;
using Serilog;

namespace InvoiceApp.Application.Services
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
                .AnyAsync(p => p.Id != entity.Id && p.Name.ToLower() == entity.Name.ToLower());
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
