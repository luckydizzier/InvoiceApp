using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvoiceApp.Data;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp.DTOs;
using InvoiceApp.Repositories;
using InvoiceApp.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvoiceApp.Tests
{
    internal class StubInvoiceRepository : IInvoiceRepository
    {
        public Task<IEnumerable<Invoice>> GetAllAsync() => Task.FromResult<IEnumerable<Invoice>>(Array.Empty<Invoice>());
        public Task<Invoice?> GetByIdAsync(int id) => Task.FromResult<Invoice?>(null);
        public Task AddAsync(Invoice entity) => Task.CompletedTask;
        public Task UpdateAsync(Invoice entity) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
        public Task<Invoice?> GetLatestForSupplierAsync(int supplierId) => Task.FromResult<Invoice?>(null);
        public Task<Invoice?> GetLatestAsync() => Task.FromResult<Invoice?>(null);
    }

    internal class StubChangeLogService : IChangeLogService
    {
        public List<ChangeLog> Logs { get; } = new();
        public Task AddAsync(ChangeLog log)
        {
            Logs.Add(log);
            return Task.CompletedTask;
        }
        public Task<ChangeLog?> GetLatestAsync() => Task.FromResult(Logs.LastOrDefault());
    }

    internal class ThrowEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _items;
        private readonly int _throwAfter;
        public ThrowEnumerable(IEnumerable<T> items, int throwAfter)
        {
            _items = items;
            _throwAfter = throwAfter;
        }
        public IEnumerator<T> GetEnumerator()
        {
            int i = 0;
            foreach (var item in _items)
            {
                if (i++ == _throwAfter) throw new Exception("fail");
                yield return item;
            }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [TestClass]
    public class InvoiceServiceTests
    {
        private StubChangeLogService _logService = new();

        private InvoiceService CreateService(string dbName)
        {
            var options = new DbContextOptionsBuilder<InvoiceContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            var factory = new PooledDbContextFactory<InvoiceContext>(options);
            _logService = new StubChangeLogService();
            return new InvoiceService(new StubInvoiceRepository(), _logService, new InvoiceDtoValidator(), factory);
        }

        [TestMethod]
        public async Task SaveInvoiceWithItemsAsync_SavesAll()
        {
            var service = CreateService(nameof(SaveInvoiceWithItemsAsync_SavesAll));
            var supplier = new Supplier { Name = "Test" };
            var invoice = new Invoice { Number = "1", Date = DateTime.Today, Supplier = supplier, PaymentMethodId = 0 };
            var item = new InvoiceItem { Quantity = 1, UnitPrice = 10 };
            invoice.Items.Add(item);
            await service.SaveInvoiceWithItemsAsync(invoice, new[] { item });

            using var ctx = ((PooledDbContextFactory<InvoiceContext>)service.GetType().GetField("_contextFactory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(service) as IDbContextFactory<InvoiceContext>)!.CreateDbContext();
            Assert.AreEqual(1, ctx.Invoices.Count());
            Assert.AreEqual(1, ctx.InvoiceItems.Count());
            Assert.AreEqual(1, ctx.Suppliers.Count());
        }

        [TestMethod]
        public async Task SaveInvoiceWithItemsAsync_RollsBackOnFailure()
        {
            var service = CreateService(nameof(SaveInvoiceWithItemsAsync_RollsBackOnFailure));
            var supplier = new Supplier { Name = "Test" };
            var invoice = new Invoice { Number = "1", Date = DateTime.Today, Supplier = supplier, PaymentMethodId = 0 };
            var items = new[] { new InvoiceItem { Quantity = 1, UnitPrice = 10 }, new InvoiceItem { Quantity = 2, UnitPrice = 20 } };
            foreach (var it in items) invoice.Items.Add(it);
            var throwing = new ThrowEnumerable<InvoiceItem>(items, 1);
            await Assert.ThrowsExceptionAsync<Exception>(() => service.SaveInvoiceWithItemsAsync(invoice, throwing));

            using var ctx = ((PooledDbContextFactory<InvoiceContext>)service.GetType().GetField("_contextFactory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(service) as IDbContextFactory<InvoiceContext>)!.CreateDbContext();
            Assert.AreEqual(0, ctx.Invoices.Count());
            Assert.AreEqual(0, ctx.InvoiceItems.Count());
            Assert.AreEqual(0, ctx.Suppliers.Count());
        }

        [TestMethod]
        public async Task SaveInvoiceWithItemsAsync_WritesChangeLogs()
        {
            var service = CreateService(nameof(SaveInvoiceWithItemsAsync_WritesChangeLogs));
            var supplier = new Supplier { Name = "Test" };
            var invoice = new Invoice { Number = "1", Date = DateTime.Today, Supplier = supplier, PaymentMethodId = 0 };
            var item = new InvoiceItem { Quantity = 1, UnitPrice = 10 };
            invoice.Items.Add(item);

            await service.SaveInvoiceWithItemsAsync(invoice, new[] { item });

            Assert.AreEqual(3, _logService.Logs.Count);
            Assert.IsTrue(_logService.Logs.Any(l => l.Entity == nameof(Supplier) && l.Operation == "Add"));
            Assert.IsTrue(_logService.Logs.Any(l => l.Entity == nameof(Invoice) && l.Operation == "Add"));
            Assert.IsTrue(_logService.Logs.Any(l => l.Entity == nameof(InvoiceItem) && l.Operation == "Add"));
        }

        [TestMethod]
        public async Task SaveInvoiceWithItemsAsync_ThrowsWhenNoItems()
        {
            var service = CreateService(nameof(SaveInvoiceWithItemsAsync_ThrowsWhenNoItems));
            var supplier = new Supplier { Name = "Test" };
            var invoice = new Invoice { Number = "1", Date = DateTime.Today, Supplier = supplier, PaymentMethodId = 0 };

            await Assert.ThrowsExceptionAsync<ValidationException>(
                () => service.SaveInvoiceWithItemsAsync(invoice, Array.Empty<InvoiceItem>()));
        }
    }
}
