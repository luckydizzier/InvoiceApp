using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;
using Serilog;

namespace InvoiceApp.Repositories
{
    public class EfProductGroupRepository : BaseRepository<ProductGroup>, IProductGroupRepository
    {
        public EfProductGroupRepository(IDbContextFactory<InvoiceContext> contextFactory)
            : base(contextFactory)
        {
        }

        public override async Task<IEnumerable<ProductGroup>> GetAllAsync()
        {
            Log.Debug("EfProductGroupRepository.GetAllAsync called");
            using var ctx = ContextFactory.CreateDbContext();
            var list = await ctx.ProductGroups.ToListAsync();
            Log.Debug("EfProductGroupRepository.GetAllAsync returning {Count} items", list.Count);
            return list;
        }

        public override async Task<ProductGroup?> GetByIdAsync(int id)
        {
            Log.Debug("EfProductGroupRepository.GetByIdAsync called with {Id}", id);
            using var ctx = ContextFactory.CreateDbContext();
            var entity = await ctx.ProductGroups.FindAsync(id).AsTask();
            Log.Debug(entity != null ? "EfProductGroupRepository.GetByIdAsync found {Id}" : "EfProductGroupRepository.GetByIdAsync no entity for {Id}", entity?.Id ?? id);
            return entity;
        }

    }
}
