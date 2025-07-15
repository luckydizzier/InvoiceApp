using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;

namespace InvoiceApp.Repositories
{
    public class EfUnitRepository : IUnitRepository
    {
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public EfUnitRepository(IDbContextFactory<InvoiceContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Unit>> GetAllAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Units.ToListAsync();
        }

        public Task<Unit?> GetByIdAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return ctx.Units.FindAsync(id).AsTask();
        }

        public async Task AddAsync(Unit unit)
        {
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.Units.AddAsync(unit);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(Unit unit)
        {
            using var ctx = _contextFactory.CreateDbContext();
            ctx.Units.Update(unit);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var entity = await ctx.Units.FindAsync(id);
            if (entity != null)
            {
                ctx.Units.Remove(entity);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
