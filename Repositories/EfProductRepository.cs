using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;

namespace InvoiceApp.Repositories
{
    public class EfProductRepository : IProductRepository
    {
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public EfProductRepository(IDbContextFactory<InvoiceContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Products.ToListAsync();
        }

        public Task<Product?> GetByIdAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return ctx.Products.FindAsync(id).AsTask();
        }

        public async Task AddAsync(Product product)
        {
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.Products.AddAsync(product);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            using var ctx = _contextFactory.CreateDbContext();
            ctx.Products.Update(product);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var entity = await ctx.Products.FindAsync(id);
            if (entity != null)
            {
                ctx.Products.Remove(entity);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
