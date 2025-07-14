using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Data;
using InvoiceApp.Models;

namespace InvoiceApp.Repositories
{
    public class EfChangeLogRepository : IChangeLogRepository
    {
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;

        public EfChangeLogRepository(IDbContextFactory<InvoiceContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task AddAsync(ChangeLog log)
        {
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.ChangeLogs.AddAsync(log);
            await ctx.SaveChangesAsync();
        }

        public async Task<ChangeLog?> GetLatestAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.ChangeLogs
                .OrderByDescending(c => c.DateCreated)
                .FirstOrDefaultAsync();
        }
    }
}
