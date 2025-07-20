using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Domain;
using InvoiceApp.Infrastructure.Data;
using Serilog;

namespace InvoiceApp.Infrastructure.Repositories
{
    public class EfProductRepository : BaseRepository<Product>, IProductRepository
    {
        public EfProductRepository(IDbContextFactory<InvoiceContext> contextFactory)
            : base(contextFactory)
        {
        }

        public override async Task<IEnumerable<Product>> GetAllAsync()
        {
            Log.Debug("EfProductRepository.GetAllAsync called");
            using var ctx = ContextFactory.CreateDbContext();
            var list = await ctx.Products.ToListAsync();
            Log.Debug("EfProductRepository.GetAllAsync returning {Count} items", list.Count);
            return list;
        }

        public override async Task<Product?> GetByIdAsync(int id)
        {
            Log.Debug("EfProductRepository.GetByIdAsync called with {Id}", id);
            using var ctx = ContextFactory.CreateDbContext();
            var entity = await ctx.Products
                .FirstOrDefaultAsync(p => p.Id == id);
            Log.Debug(entity != null ? "EfProductRepository.GetByIdAsync found {Id}" : "EfProductRepository.GetByIdAsync no entity for {Id}", entity?.Id ?? id);
            return entity;
        }

    }
}
