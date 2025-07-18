using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;
using Serilog;

namespace InvoiceApp.Repositories
{
    public class EfSupplierRepository : BaseRepository<Supplier>, ISupplierRepository
    {
        public EfSupplierRepository(IDbContextFactory<InvoiceContext> contextFactory)
            : base(contextFactory)
        {
        }

        public override async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            using var ctx = ContextFactory.CreateDbContext();
            return await ctx.Suppliers.ToListAsync();
        }

        public override Task<Supplier?> GetByIdAsync(int id)
        {
            using var ctx = ContextFactory.CreateDbContext();
            return ctx.Suppliers.FindAsync(id).AsTask();
        }

    }
}
