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
            Log.Debug("EfSupplierRepository.GetAllAsync called");
            using var ctx = ContextFactory.CreateDbContext();
            var list = await ctx.Suppliers.ToListAsync();
            Log.Debug("EfSupplierRepository.GetAllAsync returning {Count} items", list.Count);
            return list;
        }

        public override async Task<Supplier?> GetByIdAsync(int id)
        {
            Log.Debug("EfSupplierRepository.GetByIdAsync called with {Id}", id);
            using var ctx = ContextFactory.CreateDbContext();
            var entity = await ctx.Suppliers.FindAsync(id);
            Log.Debug(entity != null ? "EfSupplierRepository.GetByIdAsync found {Id}" : "EfSupplierRepository.GetByIdAsync no entity for {Id}", entity?.Id ?? id);
            return entity;
        }

    }
}
