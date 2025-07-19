using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;
using Serilog;

namespace InvoiceApp.Repositories
{
    public class EfInvoiceItemRepository : BaseRepository<InvoiceItem>, IInvoiceItemRepository
    {
        public EfInvoiceItemRepository(IDbContextFactory<InvoiceContext> contextFactory)
            : base(contextFactory)
        {
        }

        public override async Task<IEnumerable<InvoiceItem>> GetAllAsync()
        {
            using var ctx = ContextFactory.CreateDbContext();
            return await ctx.InvoiceItems
                .Include(i => i.Product!)
                    .ThenInclude(p => p!.Unit)
                .Include(i => i.Product!)
                    .ThenInclude(p => p!.ProductGroup)
                .Include(i => i.Product!)
                    .ThenInclude(p => p!.TaxRate)
                .Include(i => i.TaxRate)
                .ToListAsync();
        }

        public override Task<InvoiceItem?> GetByIdAsync(int id)
        {
            using var ctx = ContextFactory.CreateDbContext();
            return ctx.InvoiceItems
                .Include(i => i.Product!)
                    .ThenInclude(p => p!.Unit)
                .Include(i => i.Product!)
                    .ThenInclude(p => p!.ProductGroup)
                .Include(i => i.Product!)
                    .ThenInclude(p => p!.TaxRate)
                .Include(i => i.TaxRate)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public override async Task AddAsync(InvoiceItem item)
        {
            Log.Debug("EfInvoiceItemRepository.AddAsync called for {Id}", item.Id);
            using var ctx = ContextFactory.CreateDbContext();
            if (item.Product != null)
            {
                ctx.Attach(item.Product);
            }
            if (item.TaxRate != null)
            {
                ctx.Attach(item.TaxRate);
            }
            await ctx.InvoiceItems.AddAsync(item);
            await ctx.SaveChangesAsync();
            Log.Information("InvoiceItem {Id} inserted", item.Id);
        }

        public override async Task UpdateAsync(InvoiceItem item)
        {
            Log.Debug("EfInvoiceItemRepository.UpdateAsync called for {Id}", item.Id);
            using var ctx = ContextFactory.CreateDbContext();
            if (item.Product != null)
            {
                ctx.Attach(item.Product);
            }
            if (item.TaxRate != null)
            {
                ctx.Attach(item.TaxRate);
            }
            ctx.InvoiceItems.Update(item);
            await ctx.SaveChangesAsync();
            Log.Information("InvoiceItem {Id} updated", item.Id);
        }

    }
}
