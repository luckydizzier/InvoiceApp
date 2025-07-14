using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;
using InvoiceApp.Data;

namespace InvoiceApp.Repositories
{
    public class EfInvoiceRepository : IInvoiceRepository
    {
        private readonly InvoiceContext _context;

        public EfInvoiceRepository(InvoiceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            try
            {
                return await _context.Invoices.ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                Serilog.Log.Error(ex, "Failed reading invoices");
                return new List<Invoice>();
            }
        }

        public Task<Invoice?> GetByIdAsync(int id) => _context.Invoices.FindAsync(id).AsTask();

        public async Task AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Invoices.FindAsync(id);
            if (entity != null)
            {
                _context.Invoices.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
