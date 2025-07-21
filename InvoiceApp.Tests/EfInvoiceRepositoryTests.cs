using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Domain;
using InvoiceApp.Infrastructure.Data;
using InvoiceApp.Infrastructure.Repositories;

namespace InvoiceApp.Tests
{
    [TestClass]
    public class EfInvoiceRepositoryTests
    {
        private static IDbContextFactory<InvoiceContext> CreateFactory()
        {
            var options = new DbContextOptionsBuilder<InvoiceContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new PooledDbContextFactory<InvoiceContext>(options);
        }

        private static Invoice CreateInvoice()
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
                Number = "INV-1",
                Date = DateTime.Today,
                Supplier = new Supplier { Name = "Supp" },
                PaymentMethod = new PaymentMethod { Name = "Cash", DueInDays = 0 },
                Items = new List<InvoiceItem> { item }
            };
        }

        [TestMethod]
        public async Task AddAsync_PersistsItems()
        {
            var factory = CreateFactory();
            var repo = new EfInvoiceRepository(factory);
            var invoice = CreateInvoice();

            await repo.AddAsync(invoice);

            using var ctx = factory.CreateDbContext();
            var saved = ctx.Invoices.Include(i => i.Items).First();
            Assert.AreEqual(1, saved.Items.Count);
        }

        [TestMethod]
        public async Task UpdateAsync_PersistsItems()
        {
            var factory = CreateFactory();
            var repo = new EfInvoiceRepository(factory);
            var invoice = CreateInvoice();
            await repo.AddAsync(invoice);

            invoice.Items.Add(new InvoiceItem
            {
                Product = new Product
                {
                    Name = "Widget2",
                    Unit = new Unit { Name = "pc" },
                    ProductGroup = new ProductGroup { Name = "General" },
                    TaxRate = invoice.Items[0].TaxRate
                },
                TaxRate = invoice.Items[0].TaxRate,
                Quantity = 2,
                UnitPrice = 20m
            });

            await repo.UpdateAsync(invoice);

            using var ctx = factory.CreateDbContext();
            var saved = ctx.Invoices.Include(i => i.Items).First();
            Assert.AreEqual(2, saved.Items.Count);
        }
    }
}