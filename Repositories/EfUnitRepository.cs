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
            Log.Debug("EfUnitRepository.GetAllAsync called");
            using var ctx = ContextFactory.CreateDbContext();
            var list = await ctx.Units.ToListAsync();
            Log.Debug("EfUnitRepository.GetAllAsync returning {Count} items", list.Count);
            return list;
        }

        public override async Task<Unit?> GetByIdAsync(int id)
        {
            Log.Debug("EfUnitRepository.GetByIdAsync called with {Id}", id);
            using var ctx = ContextFactory.CreateDbContext();
            var entity = await ctx.Units.FindAsync(id).AsTask();
            Log.Debug(entity != null ? "EfUnitRepository.GetByIdAsync found {Id}" : "EfUnitRepository.GetByIdAsync no entity for {Id}", entity?.Id ?? id);
            return entity;
        }

    }
}
