using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;

namespace InvoiceApp.Repositories
{
    public class EfPaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public EfPaymentMethodRepository(IDbContextFactory<InvoiceContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<PaymentMethod>> GetAllAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.PaymentMethods.ToListAsync();
        }

        public Task<PaymentMethod?> GetByIdAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return ctx.PaymentMethods.FindAsync(id).AsTask();
        }

        public async Task AddAsync(PaymentMethod method)
        {
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.PaymentMethods.AddAsync(method);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(PaymentMethod method)
        {
            using var ctx = _contextFactory.CreateDbContext();
            ctx.PaymentMethods.Update(method);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var entity = await ctx.PaymentMethods.FindAsync(id);
            if (entity != null)
            {
                ctx.PaymentMethods.Remove(entity);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
