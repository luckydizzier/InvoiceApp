using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;
using Serilog;

namespace InvoiceApp.Repositories
{
    public class EfInvoiceItemRepository : BaseRepository<InvoiceItem>, IInvoiceItemRepository
    {
        public EfInvoiceItemRepository(IDbContextFactory<InvoiceContext> contextFactory)
            : base(contextFactory)
        {
        }

        public override async Task<IEnumerable<InvoiceItem>> GetAllAsync()
        {
            Log.Debug("EfInvoiceItemRepository.GetAllAsync called");
            using var ctx = ContextFactory.CreateDbContext();
            var list = await ctx.InvoiceItems
                .Include(i => i.Product!)
                    .ThenInclude(p => p!.Unit)
                .Include(i => i.Product!)
                    .ThenInclude(p => p!.ProductGroup)
                .Include(i => i.Product!)
                    .ThenInclude(p => p!.TaxRate)
                .Include(i => i.TaxRate)
                .ToListAsync();
            Log.Debug("EfInvoiceItemRepository.GetAllAsync returning {Count} items", list.Count);
            return list;
        }

        public override async Task<InvoiceItem?> GetByIdAsync(int id)
        {
            Log.Debug("EfInvoiceItemRepository.GetByIdAsync called with {Id}", id);
            using var ctx = ContextFactory.CreateDbContext();
            var entity = await ctx.InvoiceItems
                .Include(i => i.Product!)
                    .ThenInclude(p => p!.Unit)
                .Include(i => i.Product!)
                    .ThenInclude(p => p!.ProductGroup)
                .Include(i => i.Product!)
                    .ThenInclude(p => p!.TaxRate)
                .Include(i => i.TaxRate)
                .FirstOrDefaultAsync(i => i.Id == id);
            Log.Debug(entity != null ? "EfInvoiceItemRepository.GetByIdAsync found {Id}" : "EfInvoiceItemRepository.GetByIdAsync no entity for {Id}", entity?.Id ?? id);
            return entity;
        }

        public override async Task AddAsync(InvoiceItem item)
        {
            Log.Debug("EfInvoiceItemRepository.AddAsync called for {Id}", item.Id);
            using var ctx = ContextFactory.CreateDbContext();
            if (item.Product != null)
            {
                if (item.Product.Unit != null)
                {
                    var existingUnit = ctx.Units.Local
                        .FirstOrDefault(u => u.Id == item.Product.Unit.Id);
                    item.Product.Unit = existingUnit ?? item.Product.Unit;
                    if (existingUnit == null)
                    {
                        ctx.Attach(item.Product.Unit);
                    }
                }
                if (item.Product.ProductGroup != null)
                {
                    var existingGroup = ctx.ProductGroups.Local
                        .FirstOrDefault(g => g.Id == item.Product.ProductGroup.Id);
                    item.Product.ProductGroup = existingGroup ?? item.Product.ProductGroup;
                    if (existingGroup == null)
                    {
                        ctx.Attach(item.Product.ProductGroup);
                    }
                }
                ctx.Attach(item.Product);
            }
            if (item.TaxRate != null)
            {
                var existingRate = ctx.TaxRates.Local
                    .FirstOrDefault(t => t.Id == item.TaxRate.Id);
                item.TaxRate = existingRate ?? item.TaxRate;
                if (existingRate == null)
                {
                    ctx.Attach(item.TaxRate);
                }
            }
            await ctx.InvoiceItems.AddAsync(item);
            await ctx.SaveChangesAsync();
            Log.Information("InvoiceItem {Id} inserted", item.Id);
        }

        public override async Task UpdateAsync(InvoiceItem item)
        {
            Log.Debug("EfInvoiceItemRepository.UpdateAsync called for {Id}", item.Id);
            using var ctx = ContextFactory.CreateDbContext();
            if (item.Product != null)
            {
                if (item.Product.Unit != null)
                {
                    var existingUnit = ctx.Units.Local
                        .FirstOrDefault(u => u.Id == item.Product.Unit.Id);
                    item.Product.Unit = existingUnit ?? item.Product.Unit;
                    if (existingUnit == null)
                    {
                        ctx.Attach(item.Product.Unit);
                    }
                }
                if (item.Product.ProductGroup != null)
                {
                    var existingGroup = ctx.ProductGroups.Local
                        .FirstOrDefault(g => g.Id == item.Product.ProductGroup.Id);
                    item.Product.ProductGroup = existingGroup ?? item.Product.ProductGroup;
                    if (existingGroup == null)
                    {
                        ctx.Attach(item.Product.ProductGroup);
                    }
                }
                ctx.Attach(item.Product);
            }
            if (item.TaxRate != null)
            {
                var existingRate = ctx.TaxRates.Local
                    .FirstOrDefault(t => t.Id == item.TaxRate.Id);
                item.TaxRate = existingRate ?? item.TaxRate;
                if (existingRate == null)
                {
                    ctx.Attach(item.TaxRate);
                }
            }
            ctx.InvoiceItems.Update(item);
            await ctx.SaveChangesAsync();
            Log.Information("InvoiceItem {Id} updated", item.Id);
        }

    }
}
