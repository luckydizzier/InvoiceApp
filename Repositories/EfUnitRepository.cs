using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;
using Serilog;

namespace InvoiceApp.Repositories
{
    public class EfUnitRepository : BaseRepository<Unit>, IUnitRepository
    {
        public EfUnitRepository(IDbContextFactory<InvoiceContext> contextFactory)
            : base(contextFactory)
        {
        }

        public override async Task<IEnumerable<Unit>> GetAllAsync()
        {
            using var ctx = ContextFactory.CreateDbContext();
            return await ctx.Units.ToListAsync();
        }

        public override Task<Unit?> GetByIdAsync(int id)
        {
            using var ctx = ContextFactory.CreateDbContext();
            return ctx.Units.FindAsync(id).AsTask();
        }

    }
}
