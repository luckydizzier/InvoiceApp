using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;
using Serilog;

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
            Log.Debug("EfSupplierRepository.AddAsync called for {Id}", supplier.Id);
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.Suppliers.AddAsync(supplier);
            await ctx.SaveChangesAsync();
            Log.Information("Supplier {Id} inserted", supplier.Id);
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            Log.Debug("EfSupplierRepository.UpdateAsync called for {Id}", supplier.Id);
            using var ctx = _contextFactory.CreateDbContext();
            ctx.Suppliers.Update(supplier);
            await ctx.SaveChangesAsync();
            Log.Information("Supplier {Id} updated", supplier.Id);
        }

        public async Task DeleteAsync(int id)
        {
            Log.Debug("EfSupplierRepository.DeleteAsync called for {Id}", id);
            using var ctx = _contextFactory.CreateDbContext();
            var entity = await ctx.Suppliers.FindAsync(id);
            if (entity != null)
            {
                ctx.Suppliers.Remove(entity);
                await ctx.SaveChangesAsync();
                Log.Information("Supplier {Id} deleted", id);
            }
        }
    }
}
