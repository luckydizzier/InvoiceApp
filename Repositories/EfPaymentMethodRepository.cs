using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;
using Serilog;

namespace InvoiceApp.Repositories
{
    public class EfPaymentMethodRepository : BaseRepository<PaymentMethod>, IPaymentMethodRepository
    {
        public EfPaymentMethodRepository(IDbContextFactory<InvoiceContext> contextFactory)
            : base(contextFactory)
        {
        }

        public override async Task<IEnumerable<PaymentMethod>> GetAllAsync()
        {
            Log.Debug("EfPaymentMethodRepository.GetAllAsync called");
            using var ctx = ContextFactory.CreateDbContext();
            var list = await ctx.PaymentMethods.ToListAsync();
            Log.Debug("EfPaymentMethodRepository.GetAllAsync returning {Count} items", list.Count);
            return list;
        }

        public override async Task<PaymentMethod?> GetByIdAsync(int id)
        {
            Log.Debug("EfPaymentMethodRepository.GetByIdAsync called with {Id}", id);
            using var ctx = ContextFactory.CreateDbContext();
            var entity = await ctx.PaymentMethods.FindAsync(id);
            Log.Debug(entity != null ? "EfPaymentMethodRepository.GetByIdAsync found {Id}" : "EfPaymentMethodRepository.GetByIdAsync no entity for {Id}", entity?.Id ?? id);
            return entity;
        }

    }
}
