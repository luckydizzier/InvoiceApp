using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Domain;
using InvoiceApp.Infrastructure.Data;
using Serilog;

namespace InvoiceApp.Infrastructure.Repositories
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
            Log.Debug("{Repo}.GetAllAsync called", GetType().Name);
            using var ctx = _contextFactory.CreateDbContext();
            var list = await QueryWithIncludes(ctx).ToListAsync();
            Log.Debug("{Repo}.GetAllAsync returning {Count} items", GetType().Name, list.Count);
            return list;
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            Log.Debug("{Repo}.GetByIdAsync called with {Id}", GetType().Name, id);
            using var ctx = _contextFactory.CreateDbContext();
            var entity = await QueryWithIncludes(ctx).FirstOrDefaultAsync(e => e.Id == id);
            if (entity != null)
            {
                Log.Debug("{Repo}.GetByIdAsync found {Id}", GetType().Name, entity.Id);
            }
            else
            {
                Log.Debug("{Repo}.GetByIdAsync no entity for {Id}", GetType().Name, id);
            }
            return entity;
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
