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
        public IEnumerable<Invoice> Headers { get; set; } = Array.Empty<Invoice>();
        public Invoice? Detail { get; set; }

        public Task<IEnumerable<Invoice>> GetAllAsync() => Task.FromResult<IEnumerable<Invoice>>(Headers);
        public Task<IEnumerable<Invoice>> GetHeadersAsync() => Task.FromResult(Headers);
        public Task<Invoice?> GetByIdAsync(int id) => Task.FromResult(Detail);
        public Task<Invoice?> GetDetailsAsync(int id) => Task.FromResult(Detail);
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
        public async Task SaveInvoiceWithItemsAsync_BatchInsertAndUpdate_Succeeds()
        {
            var service = CreateService(nameof(SaveInvoiceWithItemsAsync_BatchInsertAndUpdate_Succeeds));
            var supplier = new Supplier { Name = "Test" };
            var invoice = new Invoice { Number = "1", Date = DateTime.Today, Supplier = supplier, PaymentMethodId = 0 };
            var item1 = new InvoiceItem { Quantity = 1, UnitPrice = 10 };
            var item2 = new InvoiceItem { Quantity = 2, UnitPrice = 20 };
            invoice.Items.Add(item1);
            invoice.Items.Add(item2);
            await service.SaveInvoiceWithItemsAsync(invoice, new[] { item1, item2 });

            // modify existing items and save again to test batch updates
            item1.Quantity = 3;
            item2.Quantity = 4;
            await service.SaveInvoiceWithItemsAsync(invoice, new[] { item1, item2 });

            using var ctx = ((PooledDbContextFactory<InvoiceContext>)service.GetType().GetField("_contextFactory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(service) as IDbContextFactory<InvoiceContext>)!.CreateDbContext();
            Assert.AreEqual(1, ctx.Invoices.Count());
            Assert.AreEqual(2, ctx.InvoiceItems.Count());
            Assert.AreEqual(3, ctx.InvoiceItems.First(i => i.Id == item1.Id).Quantity);
            Assert.AreEqual(4, ctx.InvoiceItems.First(i => i.Id == item2.Id).Quantity);
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

        [TestMethod]
        public async Task GetHeadersAsync_ReturnsRepositoryData()
        {
            var repo = new StubInvoiceRepository { Headers = new[] { new Invoice { Number = "1" } } };
            var options = new DbContextOptionsBuilder<InvoiceContext>()
                .UseInMemoryDatabase(nameof(GetHeadersAsync_ReturnsRepositoryData))
                .Options;
            var factory = new PooledDbContextFactory<InvoiceContext>(options);
            var service = new InvoiceService(repo, _logService, new InvoiceDtoValidator(), factory);

            var result = await service.GetHeadersAsync();

            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public async Task GetDetailsAsync_ReturnsRepositoryData()
        {
            var repo = new StubInvoiceRepository { Detail = new Invoice { Id = 1, Number = "1" } };
            var options = new DbContextOptionsBuilder<InvoiceContext>()
                .UseInMemoryDatabase(nameof(GetDetailsAsync_ReturnsRepositoryData))
                .Options;
            var factory = new PooledDbContextFactory<InvoiceContext>(options);
            var service = new InvoiceService(repo, _logService, new InvoiceDtoValidator(), factory);

            var result = await service.GetDetailsAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result!.Id);
        }
    }
}
