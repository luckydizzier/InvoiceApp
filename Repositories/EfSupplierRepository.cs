using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;

namespace InvoiceApp.Repositories
{
    public class EfSupplierRepository : ISupplierRepository
    {
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public EfSupplierRepository(IDbContextFactory<InvoiceContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Suppliers.ToListAsync();
        }

        public Task<Supplier?> GetByIdAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return ctx.Suppliers.FindAsync(id).AsTask();
        }

        public async Task AddAsync(Supplier supplier)
        {
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.Suppliers.AddAsync(supplier);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            using var ctx = _contextFactory.CreateDbContext();
            ctx.Suppliers.Update(supplier);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var entity = await ctx.Suppliers.FindAsync(id);
            if (entity != null)
            {
                ctx.Suppliers.Remove(entity);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
