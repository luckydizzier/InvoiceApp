using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;
using Serilog;

namespace InvoiceApp.Repositories
{
    public class EfProductGroupRepository : IProductGroupRepository
    {
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public EfProductGroupRepository(IDbContextFactory<InvoiceContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<ProductGroup>> GetAllAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.ProductGroups.ToListAsync();
        }

        public Task<ProductGroup?> GetByIdAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return ctx.ProductGroups.FindAsync(id).AsTask();
        }

        public async Task AddAsync(ProductGroup group)
        {
            Log.Debug("EfProductGroupRepository.AddAsync called for {Id}", group.Id);
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.ProductGroups.AddAsync(group);
            await ctx.SaveChangesAsync();
            Log.Information("ProductGroup {Id} inserted", group.Id);
        }

        public async Task UpdateAsync(ProductGroup group)
        {
            Log.Debug("EfProductGroupRepository.UpdateAsync called for {Id}", group.Id);
            using var ctx = _contextFactory.CreateDbContext();
            ctx.ProductGroups.Update(group);
            await ctx.SaveChangesAsync();
            Log.Information("ProductGroup {Id} updated", group.Id);
        }

        public async Task DeleteAsync(int id)
        {
            Log.Debug("EfProductGroupRepository.DeleteAsync called for {Id}", id);
            using var ctx = _contextFactory.CreateDbContext();
            var entity = await ctx.ProductGroups.FindAsync(id);
            if (entity != null)
            {
                ctx.ProductGroups.Remove(entity);
                await ctx.SaveChangesAsync();
                Log.Information("ProductGroup {Id} deleted", id);
            }
        }
    }
}
