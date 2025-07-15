using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;

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
                return await ctx.Invoices
                    .Include(i => i.Items)
                    .ThenInclude(it => it.Product)
                    .ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                Serilog.Log.Error(ex, "Failed reading invoices");
                return new List<Invoice>();
            }
        }

        public Task<Invoice?> GetByIdAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return ctx.Invoices
                .Include(i => i.Items)
                .ThenInclude(it => it.Product)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task AddAsync(Invoice invoice)
        {
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.Invoices.AddAsync(invoice);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            using var ctx = _contextFactory.CreateDbContext();
            ctx.Invoices.Update(invoice);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var entity = await ctx.Invoices.FindAsync(id);
            if (entity != null)
            {
                ctx.Invoices.Remove(entity);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
