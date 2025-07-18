using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;
using Serilog;

namespace InvoiceApp.Repositories
{
    public class EfProductRepository : BaseRepository<Product>, IProductRepository
    {
        public EfProductRepository(IDbContextFactory<InvoiceContext> contextFactory)
            : base(contextFactory)
        {
        }

        public override async Task<IEnumerable<Product>> GetAllAsync()
        {
            using var ctx = ContextFactory.CreateDbContext();
            return await ctx.Products
                .Include(p => p.Unit)
                .Include(p => p.ProductGroup)
                .Include(p => p.TaxRate)
                .ToListAsync();
        }

        public override Task<Product?> GetByIdAsync(int id)
        {
            using var ctx = ContextFactory.CreateDbContext();
            return ctx.Products
                .Include(p => p.Unit)
                .Include(p => p.ProductGroup)
                .Include(p => p.TaxRate)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

    }
}
