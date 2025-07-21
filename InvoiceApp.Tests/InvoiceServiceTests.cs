using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Domain;
using InvoiceApp.Application.Services;
using InvoiceApp.Infrastructure.Data;
using InvoiceApp.Infrastructure.Repositories;
using InvoiceApp.Application.Validators;
using InvoiceApp.Application.DTOs;

namespace InvoiceApp.Tests
{
    [TestClass]
    public class InvoiceServiceTests
    {
        private static IDbContextFactory<InvoiceContext> CreateFactory()
        {
            var options = new DbContextOptionsBuilder<InvoiceContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new PooledDbContextFactory<InvoiceContext>(options);
        }

        private static Invoice CreateInvoice(string number)
        {
            var tax = new TaxRate { Name = "A", Percentage = 27m, EffectiveFrom = DateTime.Today };
            var product = new Product
            {
                Name = "Widget",
                Unit = new Unit { Name = "pc" },
                ProductGroup = new ProductGroup { Name = "General" },
                TaxRate = tax
            };
            var item = new InvoiceItem
            {
                Product = product,
                TaxRate = tax,
                Quantity = 1,
                UnitPrice = 10m
            };
            return new Invoice
            {
                Number = number,
                Date = DateTime.Today,
                Supplier = new Supplier { Name = "Supp" },
                PaymentMethod = new PaymentMethod { Name = "Cash", DueInDays = 0 },
                Items = new List<InvoiceItem> { item }
            };
        }

        [TestMethod]
        public async Task SaveAsync_Throws_WhenNumberDuplicate()
        {
            var factory = CreateFactory();
            var repo = new EfInvoiceRepository(factory);
            var changeRepo = new EfChangeLogRepository(factory);
            var logService = new ChangeLogService(changeRepo);
            var validator = new InvoiceDtoValidator();
            var service = new InvoiceService(repo, logService, validator);

            var first = CreateInvoice("INV-1");
            await service.SaveAsync(first);

            var duplicate = CreateInvoice("INV-1");
            await Assert.ThrowsExceptionAsync<BusinessRuleViolationException>(() => service.SaveAsync(duplicate));
        }
    }
}

