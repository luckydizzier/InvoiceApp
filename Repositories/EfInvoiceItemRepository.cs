using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;

namespace InvoiceApp.Repositories
{
    public class EfInvoiceItemRepository : IInvoiceItemRepository
    {
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public EfInvoiceItemRepository(IDbContextFactory<InvoiceContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<InvoiceItem>> GetAllAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.InvoiceItems
                .Include(i => i.Product)
                    .ThenInclude(p => p.Unit)
                .Include(i => i.Product)
                    .ThenInclude(p => p.ProductGroup)
                .Include(i => i.Product)
                    .ThenInclude(p => p.TaxRate)
                .Include(i => i.TaxRate)
                .ToListAsync();
        }

        public Task<InvoiceItem?> GetByIdAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return ctx.InvoiceItems
                .Include(i => i.Product)
                    .ThenInclude(p => p.Unit)
                .Include(i => i.Product)
                    .ThenInclude(p => p.ProductGroup)
                .Include(i => i.Product)
                    .ThenInclude(p => p.TaxRate)
                .Include(i => i.TaxRate)
                .FirstOrDefaultAsync(i => i.Id == id)
                .AsTask();
        }

        public async Task AddAsync(InvoiceItem item)
        {
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.InvoiceItems.AddAsync(item);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(InvoiceItem item)
        {
            using var ctx = _contextFactory.CreateDbContext();
            ctx.InvoiceItems.Update(item);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var entity = await ctx.InvoiceItems.FindAsync(id);
            if (entity != null)
            {
                ctx.InvoiceItems.Remove(entity);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
