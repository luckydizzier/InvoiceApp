using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;
using Serilog;

namespace InvoiceApp.Repositories
{
    public class EfInvoiceRepository : IInvoiceRepository
    {
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public EfInvoiceRepository(IDbContextFactory<InvoiceContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            try
            {
                using var ctx = _contextFactory.CreateDbContext();
                var invoices = await ctx.Invoices
                    .Include(i => i.Supplier)
                    .Include(i => i.PaymentMethod)
                    .Include(i => i.Items)
                        .ThenInclude(it => it.Product)
                            .ThenInclude(p => p.Unit)
                    .Include(i => i.Items)
                        .ThenInclude(it => it.Product)
                            .ThenInclude(p => p.ProductGroup)
                    .Include(i => i.Items)
                        .ThenInclude(it => it.Product)
                            .ThenInclude(p => p.TaxRate)
                    .Include(i => i.Items)
                        .ThenInclude(it => it.TaxRate)
                    .ToListAsync();

                Serilog.Log.Debug("Loaded {Count} invoices with navigation properties", invoices.Count);
                return invoices;
            }
            catch (DbUpdateException ex)
            {
                Serilog.Log.Error(ex, "Failed reading invoices");
                return new List<Invoice>();
            }
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var invoice = await ctx.Invoices
                .Include(i => i.Supplier)
                .Include(i => i.PaymentMethod)
                .Include(i => i.Items)
                    .ThenInclude(it => it.Product)
                        .ThenInclude(p => p.Unit)
                .Include(i => i.Items)
                    .ThenInclude(it => it.Product)
                        .ThenInclude(p => p.ProductGroup)
                .Include(i => i.Items)
                    .ThenInclude(it => it.Product)
                        .ThenInclude(p => p.TaxRate)
                .Include(i => i.Items)
                    .ThenInclude(it => it.TaxRate)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice != null)
            {
                Serilog.Log.Debug(
                    "Loaded invoice {Id} with {Items} items and navigation properties",
                    invoice.Id,
                    invoice.Items?.Count ?? 0);
            }
            else
            {
                Serilog.Log.Debug("Invoice {Id} not found", id);
            }

            return invoice;
        }

        public async Task<Invoice?> GetLatestForSupplierAsync(int supplierId)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Invoices
                .Where(i => i.SupplierId == supplierId)
                .OrderByDescending(i => i.Number)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Invoice invoice)
        {
            Log.Debug("EfInvoiceRepository.AddAsync called for {Id}", invoice.Id);
            using var ctx = _contextFactory.CreateDbContext();
            if (invoice.Supplier != null)
            {
                ctx.Attach(invoice.Supplier);
            }
            if (invoice.PaymentMethod != null)
            {
                ctx.Attach(invoice.PaymentMethod);
            }
            await ctx.Invoices.AddAsync(invoice);
            await ctx.SaveChangesAsync();
            Log.Information("Invoice {Id} inserted", invoice.Id);
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            Log.Debug("EfInvoiceRepository.UpdateAsync called for {Id}", invoice.Id);
            using var ctx = _contextFactory.CreateDbContext();
            if (invoice.Supplier != null)
            {
                ctx.Attach(invoice.Supplier);
            }
            if (invoice.PaymentMethod != null)
            {
                ctx.Attach(invoice.PaymentMethod);
            }
            ctx.Invoices.Update(invoice);
            await ctx.SaveChangesAsync();
            Log.Information("Invoice {Id} updated in DB", invoice.Id);
        }

        public async Task DeleteAsync(int id)
        {
            Log.Debug("EfInvoiceRepository.DeleteAsync called for {Id}", id);
            using var ctx = _contextFactory.CreateDbContext();
            var entity = await ctx.Invoices.FindAsync(id);
            if (entity != null)
            {
                ctx.Invoices.Remove(entity);
                await ctx.SaveChangesAsync();
                Log.Information("Invoice {Id} deleted from DB", id);
            }
        }
    }
}
