using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;
using Serilog;

namespace InvoiceApp.Repositories
{
    public class EfTaxRateRepository : BaseRepository<TaxRate>, ITaxRateRepository
    {
        public EfTaxRateRepository(IDbContextFactory<InvoiceContext> contextFactory)
            : base(contextFactory)
        {
        }

        public override async Task<IEnumerable<TaxRate>> GetAllAsync()
        {
            Log.Debug("EfTaxRateRepository.GetAllAsync called");
            using var ctx = ContextFactory.CreateDbContext();
            var list = await ctx.TaxRates.ToListAsync();
            Log.Debug("EfTaxRateRepository.GetAllAsync returning {Count} items", list.Count);
            return list;
        }

        public override async Task<TaxRate?> GetByIdAsync(int id)
        {
            Log.Debug("EfTaxRateRepository.GetByIdAsync called with {Id}", id);
            using var ctx = ContextFactory.CreateDbContext();
            var entity = await ctx.TaxRates.FindAsync(id);

            Log.Debug(entity != null ? "EfTaxRateRepository.GetByIdAsync found {Id}" : "EfTaxRateRepository.GetByIdAsync no entity for {Id}", entity?.Id ?? id);
            return entity;
        }

    }
}
