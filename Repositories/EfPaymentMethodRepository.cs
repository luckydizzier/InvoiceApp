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
            using var ctx = ContextFactory.CreateDbContext();
            return await ctx.PaymentMethods.ToListAsync();
        }

        public override Task<PaymentMethod?> GetByIdAsync(int id)
        {
            using var ctx = ContextFactory.CreateDbContext();
            return ctx.PaymentMethods.FindAsync(id).AsTask();
        }

    }
}
