using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;

namespace InvoiceApp.Repositories
{
    public class EfTaxRateRepository : ITaxRateRepository
    {
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public EfTaxRateRepository(IDbContextFactory<InvoiceContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<TaxRate>> GetAllAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.TaxRates.ToListAsync();
        }

        public Task<TaxRate?> GetByIdAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return ctx.TaxRates.FindAsync(id).AsTask();
        }

        public async Task AddAsync(TaxRate rate)
        {
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.TaxRates.AddAsync(rate);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaxRate rate)
        {
            using var ctx = _contextFactory.CreateDbContext();
            ctx.TaxRates.Update(rate);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var entity = await ctx.TaxRates.FindAsync(id);
            if (entity != null)
            {
                ctx.TaxRates.Remove(entity);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
