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
            using var ctx = ContextFactory.CreateDbContext();
            return await ctx.TaxRates.ToListAsync();
        }

        public override Task<TaxRate?> GetByIdAsync(int id)
        {
            using var ctx = ContextFactory.CreateDbContext();
            return ctx.TaxRates.FindAsync(id).AsTask();
        }

    }
}
