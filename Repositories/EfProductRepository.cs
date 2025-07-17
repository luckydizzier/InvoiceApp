using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;
using Serilog;

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
            return await ctx.Products
                .Include(p => p.Unit)
                .Include(p => p.ProductGroup)
                .Include(p => p.TaxRate)
                .ToListAsync();
        }

        public Task<Product?> GetByIdAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return ctx.Products
                .Include(p => p.Unit)
                .Include(p => p.ProductGroup)
                .Include(p => p.TaxRate)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product)
        {
            Log.Debug("EfProductRepository.AddAsync called for {Id}", product.Id);
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.Products.AddAsync(product);
            await ctx.SaveChangesAsync();
            Log.Information("Product {Id} inserted", product.Id);
        }

        public async Task UpdateAsync(Product product)
        {
            Log.Debug("EfProductRepository.UpdateAsync called for {Id}", product.Id);
            using var ctx = _contextFactory.CreateDbContext();
            ctx.Products.Update(product);
            await ctx.SaveChangesAsync();
            Log.Information("Product {Id} updated", product.Id);
        }

        public async Task DeleteAsync(int id)
        {
            Log.Debug("EfProductRepository.DeleteAsync called for {Id}", id);
            using var ctx = _contextFactory.CreateDbContext();
            var entity = await ctx.Products.FindAsync(id);
            if (entity != null)
            {
                ctx.Products.Remove(entity);
                await ctx.SaveChangesAsync();
                Log.Information("Product {Id} deleted", id);
            }
        }
    }
}
