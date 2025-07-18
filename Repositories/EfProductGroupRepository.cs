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
            using var ctx = ContextFactory.CreateDbContext();
            return await ctx.ProductGroups.ToListAsync();
        }

        public override Task<ProductGroup?> GetByIdAsync(int id)
        {
            using var ctx = ContextFactory.CreateDbContext();
            return ctx.ProductGroups.FindAsync(id).AsTask();
        }

    }
}
