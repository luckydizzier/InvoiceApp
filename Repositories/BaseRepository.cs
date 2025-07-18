using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;
using Serilog;

namespace InvoiceApp.Repositories
{
    public abstract class BaseRepository<T> : ICrudRepository<T> where T : Base
    {
        private readonly IDbContextFactory<InvoiceContext> _contextFactory;
        protected IDbContextFactory<InvoiceContext> ContextFactory => _contextFactory;

        protected BaseRepository(IDbContextFactory<InvoiceContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        protected virtual IQueryable<T> QueryWithIncludes(InvoiceContext context)
        {
            return context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await QueryWithIncludes(ctx).ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await QueryWithIncludes(ctx).FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task AddAsync(T entity)
        {
            Log.Debug("{Repo}.AddAsync called for {Id}", GetType().Name, entity.Id);
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.Set<T>().AddAsync(entity);
            await ctx.SaveChangesAsync();
            Log.Information("{Entity} {Id} inserted", typeof(T).Name, entity.Id);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            Log.Debug("{Repo}.UpdateAsync called for {Id}", GetType().Name, entity.Id);
            using var ctx = _contextFactory.CreateDbContext();
            ctx.Set<T>().Update(entity);
            await ctx.SaveChangesAsync();
            Log.Information("{Entity} {Id} updated", typeof(T).Name, entity.Id);
        }

        public virtual async Task DeleteAsync(int id)
        {
            Log.Debug("{Repo}.DeleteAsync called for {Id}", GetType().Name, id);
            using var ctx = _contextFactory.CreateDbContext();
            var entity = await ctx.Set<T>().FindAsync(id);
            if (entity != null)
            {
                ctx.Set<T>().Remove(entity);
                await ctx.SaveChangesAsync();
                Log.Information("{Entity} {Id} deleted", typeof(T).Name, id);
            }
        }
    }
}
