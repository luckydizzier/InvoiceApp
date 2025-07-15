using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;

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
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.ProductGroups.AddAsync(group);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductGroup group)
        {
            using var ctx = _contextFactory.CreateDbContext();
            ctx.ProductGroups.Update(group);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var entity = await ctx.ProductGroups.FindAsync(id);
            if (entity != null)
            {
                ctx.ProductGroups.Remove(entity);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
