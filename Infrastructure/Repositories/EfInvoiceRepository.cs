using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Domain;
using InvoiceApp.Infrastructure.Data;
using Serilog;

namespace InvoiceApp.Infrastructure.Repositories
{
    public class EfInvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        public EfInvoiceRepository(IDbContextFactory<InvoiceContext> contextFactory)
            : base(contextFactory)
        {
        }

        public InvoiceContext CreateContext()
        {
            return ContextFactory.CreateDbContext();
        }

        public async Task<IEnumerable<Invoice>> GetHeadersAsync()
        {
            using var ctx = ContextFactory.CreateDbContext();
            return await ctx.Invoices
                .Include(i => i.Supplier)
                .ToListAsync();
        }

        public async Task<Invoice?> GetDetailsAsync(int id)
        {
            using var ctx = ContextFactory.CreateDbContext();
            return await ctx.Invoices
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
        }

        public override async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            try
            {
                using var ctx = ContextFactory.CreateDbContext();
                var invoices = await ctx.Invoices
                    .Include(i => i.Supplier)
                    .Include(i => i.PaymentMethod)
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

        public override async Task<Invoice?> GetByIdAsync(int id)
        {
            using var ctx = ContextFactory.CreateDbContext();
            var invoice = await ctx.Invoices
                .Include(i => i.Supplier)
                .Include(i => i.PaymentMethod)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice != null)
            {
                Serilog.Log.Debug("Loaded invoice {Id}", invoice.Id);
            }
            else
            {
                Serilog.Log.Debug("Invoice {Id} not found", id);
            }

            return invoice;
        }

        public async Task<Invoice?> GetLatestForSupplierAsync(int supplierId)
        {
            using var ctx = ContextFactory.CreateDbContext();
            return await ctx.Invoices
                .Where(i => i.SupplierId == supplierId)
                .OrderByDescending(i => i.Number)
                .FirstOrDefaultAsync();
        }

        public async Task<Invoice?> GetLatestAsync()
        {
            using var ctx = ContextFactory.CreateDbContext();
            return await ctx.Invoices
                .OrderByDescending(i => i.DateCreated)
                .Include(i => i.Supplier)
                .FirstOrDefaultAsync();
        }

        public override async Task AddAsync(Invoice invoice)
        {
            Log.Debug("EfInvoiceRepository.AddAsync called for {Id}", invoice.Id);
            using var ctx = ContextFactory.CreateDbContext();
            AttachRelations(invoice, ctx);
            AttachItems(invoice, ctx);
            await ctx.Invoices.AddAsync(invoice);
            await ctx.SaveChangesAsync();
            Log.Information("Invoice {Id} inserted", invoice.Id);
        }

        public override async Task UpdateAsync(Invoice invoice)
        {
            Log.Debug("EfInvoiceRepository.UpdateAsync called for {Id}", invoice.Id);
            using var ctx = ContextFactory.CreateDbContext();
            AttachRelations(invoice, ctx);
            AttachItems(invoice, ctx);
            ctx.Invoices.Update(invoice);
            await ctx.SaveChangesAsync();
            Log.Information("Invoice {Id} updated in DB", invoice.Id);
        }

        public async Task SaveAsync(Invoice invoice, InvoiceContext context)
        {
            AttachRelations(invoice, context);

            if (invoice.Id == 0)
            {
                await context.Invoices.AddAsync(invoice);
            }
            else
            {
                context.Invoices.Update(invoice);
            }

            await context.SaveChangesAsync();
            Log.Information("Invoice {Id} saved", invoice.Id);
        }

        private static void AttachRelations(Invoice invoice, DbContext ctx)
        {
            if (invoice.Supplier != null)
            {
                ctx.Attach(invoice.Supplier);
            }
            if (invoice.PaymentMethod != null)
            {
                ctx.Attach(invoice.PaymentMethod);
            }
            if (invoice.Items != null)
            {
                foreach (var item in invoice.Items)
                {
                    if (item.Product != null)
                    {
                        ctx.Attach(item.Product);
                    }
                    if (item.TaxRate != null)
                    {
                        ctx.Attach(item.TaxRate);
                    }
                    ctx.Entry(item).State = item.Id == 0 ? EntityState.Added : EntityState.Modified;
                }
            }
        }

        private static void AttachItems(Invoice invoice, DbContext ctx)
        {
            if (invoice.Items == null) return;

            foreach (var item in invoice.Items)
            {
                if (ctx.Entry(item).State == EntityState.Detached)
                {
                    ctx.Attach(item);
                }
            }
        }

    }
}
