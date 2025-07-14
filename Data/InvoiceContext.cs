using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;

namespace InvoiceApp.Data
{
    public class InvoiceContext : DbContext
    {
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<ChangeLog> ChangeLogs => Set<ChangeLog>();

        public InvoiceContext(DbContextOptions<InvoiceContext> options)
            : base(options)
        {
        }
    }
}
