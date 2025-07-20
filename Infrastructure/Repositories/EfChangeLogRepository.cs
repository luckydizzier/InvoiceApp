using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Infrastructure.Data;
using InvoiceApp.Domain;
using Serilog;

namespace InvoiceApp.Infrastructure.Repositories
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
            Log.Debug("EfChangeLogRepository.AddAsync called for {Entity}", log.Entity);
            using var ctx = _contextFactory.CreateDbContext();
            await AddAsync(log, ctx);
        }

        public async Task AddAsync(ChangeLog log, InvoiceContext context)
        {
            await context.ChangeLogs.AddAsync(log);
            await context.SaveChangesAsync();
            Log.Information("ChangeLog entry added for {Entity}", log.Entity);
        }

        public async Task<ChangeLog?> GetLatestAsync()
        {
            Log.Debug("EfChangeLogRepository.GetLatestAsync called");
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.ChangeLogs
                .OrderByDescending(c => c.DateCreated)
                .FirstOrDefaultAsync();
        }
    }
}
