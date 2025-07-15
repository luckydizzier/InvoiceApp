using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;

namespace InvoiceApp.Data
{
    public class InvoiceContext : DbContext
    {
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<ChangeLog> ChangeLogs => Set<ChangeLog>();
        public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
        public DbSet<Unit> Units => Set<Unit>();
        public DbSet<ProductGroup> ProductGroups => Set<ProductGroup>();
        public DbSet<TaxRate> TaxRates => Set<TaxRate>();

        public InvoiceContext(DbContextOptions<InvoiceContext> options)
            : base(options)
        {
        }
    }
}
