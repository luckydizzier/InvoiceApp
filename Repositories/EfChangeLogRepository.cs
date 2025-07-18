using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Data;
using InvoiceApp.Models;
using Serilog;

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
            Log.Debug("EfChangeLogRepository.AddAsync called for {Entity}", log.Entity);
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.ChangeLogs.AddAsync(log);
            await ctx.SaveChangesAsync();
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
