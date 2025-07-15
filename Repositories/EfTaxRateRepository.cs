using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;
using Serilog;

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
            Log.Debug("EfTaxRateRepository.AddAsync called for {Id}", rate.Id);
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.TaxRates.AddAsync(rate);
            await ctx.SaveChangesAsync();
            Log.Information("TaxRate {Id} inserted", rate.Id);
        }

        public async Task UpdateAsync(TaxRate rate)
        {
            Log.Debug("EfTaxRateRepository.UpdateAsync called for {Id}", rate.Id);
            using var ctx = _contextFactory.CreateDbContext();
            ctx.TaxRates.Update(rate);
            await ctx.SaveChangesAsync();
            Log.Information("TaxRate {Id} updated", rate.Id);
        }

        public async Task DeleteAsync(int id)
        {
            Log.Debug("EfTaxRateRepository.DeleteAsync called for {Id}", id);
            using var ctx = _contextFactory.CreateDbContext();
            var entity = await ctx.TaxRates.FindAsync(id);
            if (entity != null)
            {
                ctx.TaxRates.Remove(entity);
                await ctx.SaveChangesAsync();
                Log.Information("TaxRate {Id} deleted", id);
            }
        }
    }
}
