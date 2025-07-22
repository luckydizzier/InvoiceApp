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

        [TestMethod]
        public async Task SaveAsync_PersistsItemsOnce()
        {
            var factory = CreateFactory();
            var repo = new EfInvoiceRepository(factory);
            var changeRepo = new EfChangeLogRepository(factory);
            var logService = new ChangeLogService(changeRepo);
            var validator = new InvoiceDtoValidator();
            var service = new InvoiceService(repo, logService, validator);

            var invoice = CreateInvoice("INV-2");
            invoice.Items.Add(new InvoiceItem
            {
                Product = invoice.Items[0].Product,
                TaxRate = invoice.Items[0].TaxRate,
                Quantity = 2,
                UnitPrice = 20m
            });

            var expectedCount = invoice.Items.Count;

            await service.SaveAsync(invoice);

            // Save again to update without adding new items
            invoice.Number = "INV-2A";
            await service.SaveAsync(invoice);

            using var ctx = factory.CreateDbContext();
            var saved = ctx.Invoices.Include(i => i.Items).First();
            Assert.AreEqual(expectedCount, saved.Items.Count);
        }

        [TestMethod]
        public async Task SaveAsync_PersistsAllItems()
        {
            var factory = CreateFactory();
            var repo = new EfInvoiceRepository(factory);
            var changeRepo = new EfChangeLogRepository(factory);
            var logService = new ChangeLogService(changeRepo);
            var validator = new InvoiceDtoValidator();
            var service = new InvoiceService(repo, logService, validator);

            var invoice = CreateInvoice("INV-3");
            invoice.Items.Add(new InvoiceItem
            {
                Product = invoice.Items[0].Product,
                TaxRate = invoice.Items[0].TaxRate,
                Quantity = 5,
                UnitPrice = 50m
            });

            await service.SaveAsync(invoice);

            using var ctx = factory.CreateDbContext();
            var saved = ctx.Invoices.Include(i => i.Items).First();
            Assert.AreEqual(invoice.Items.Count, saved.Items.Count);
        }

        [TestMethod]
        public async Task SaveAsync_DoesNotDuplicateItemsAcrossSaves()
        {
            var factory = CreateFactory();
            var repo = new EfInvoiceRepository(factory);
            var changeRepo = new EfChangeLogRepository(factory);
            var logService = new ChangeLogService(changeRepo);
            var validator = new InvoiceDtoValidator();
            var service = new InvoiceService(repo, logService, validator);

            var invoice = CreateInvoice("INV-CTX");
            invoice.Items.Add(new InvoiceItem
            {
                Product = invoice.Items[0].Product,
                TaxRate = invoice.Items[0].TaxRate,
                Quantity = 3,
                UnitPrice = 30m
            });

            await service.SaveAsync(invoice);

            var firstItemIds = invoice.Items.Select(i => i.Id).ToList();
            Assert.IsTrue(firstItemIds.All(id => id > 0));

            invoice.Number = "INV-CTX-2";
            invoice.Items[0].Quantity = 2;

            await service.SaveAsync(invoice);

            using var ctx = factory.CreateDbContext();
            var saved = ctx.Invoices.Include(i => i.Items).First();
            var savedItemIds = saved.Items.Select(i => i.Id).ToList();

            CollectionAssert.AreEquivalent(firstItemIds, savedItemIds);
        }

        [TestMethod]
        public async Task SaveAsync_Throws_WhenDateInFuture()
        {
            var factory = CreateFactory();
            var repo = new EfInvoiceRepository(factory);
            var changeRepo = new EfChangeLogRepository(factory);
            var logService = new ChangeLogService(changeRepo);
            var validator = new InvoiceDtoValidator();
            var service = new InvoiceService(repo, logService, validator);

            var invoice = CreateInvoice("INV-FUT");
            invoice.Date = DateTime.Today.AddDays(1);

            await Assert.ThrowsExceptionAsync<BusinessRuleViolationException>(() => service.SaveAsync(invoice));
        }

        [TestMethod]
        public async Task SaveAsync_Throws_WhenNoItems()
        {
            var factory = CreateFactory();
            var repo = new EfInvoiceRepository(factory);
            var changeRepo = new EfChangeLogRepository(factory);
            var logService = new ChangeLogService(changeRepo);
            var validator = new InvoiceDtoValidator();
            var service = new InvoiceService(repo, logService, validator);

            var invoice = CreateInvoice("INV-EMPTY");
            invoice.Items.Clear();

            await Assert.ThrowsExceptionAsync<BusinessRuleViolationException>(() => service.SaveAsync(invoice));
        }

        [TestMethod]
        public void AggregateItems_GroupsSameProducts()
        {
            var factory = CreateFactory();
            var repo = new EfInvoiceRepository(factory);
            var changeRepo = new EfChangeLogRepository(factory);
            var logService = new ChangeLogService(changeRepo);
            var validator = new InvoiceDtoValidator();
            var service = new InvoiceService(repo, logService, validator);

            var tax = new TaxRate { Name = "A", Percentage = 27m };
            var product = new Product { Name = "Widget", Unit = new Unit { Name = "pc" }, ProductGroup = new ProductGroup { Name = "General" }, TaxRate = tax };

            var items = new List<InvoiceItem>
            {
                new InvoiceItem { Product = product, ProductId = 1, TaxRate = tax, TaxRateId = 1, Quantity = 1m, UnitPrice = 10m },
                new InvoiceItem { Product = product, ProductId = 1, TaxRate = tax, TaxRateId = 1, Quantity = 2m, UnitPrice = 10m }
            };

            var result = service.AggregateItems(items, false).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(3m, result[0].Quantity);
            Assert.IsTrue(result[0].IsAggregated);
        }
    }
}

