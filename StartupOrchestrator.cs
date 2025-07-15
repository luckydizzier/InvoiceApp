using System;

using System.IO;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using InvoiceApp.Data;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using InvoiceApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace InvoiceApp
{
    public static class StartupOrchestrator
    {
        public static IServiceProvider Configure()
        {
            EnsureConfig();

            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connection = config["ConnectionString"] ?? $"Data Source={Path.Combine(AppContext.BaseDirectory, "invoice.db")}";

            var services = new ServiceCollection();
            services.AddDbContextFactory<InvoiceContext>(o => o.UseSqlite(connection));
            services.AddSingleton<IInvoiceRepository, EfInvoiceRepository>();
            services.AddSingleton<IChangeLogRepository, EfChangeLogRepository>();
            services.AddSingleton<IInvoiceItemRepository, EfInvoiceItemRepository>();
            services.AddSingleton<IProductRepository, EfProductRepository>();
            services.AddSingleton<IPaymentMethodRepository, EfPaymentMethodRepository>();
            services.AddSingleton<ISupplierRepository, EfSupplierRepository>();
            services.AddSingleton<IUnitRepository, EfUnitRepository>();
            services.AddSingleton<IProductGroupRepository, EfProductGroupRepository>();
            services.AddSingleton<ITaxRateRepository, EfTaxRateRepository>();

            services.AddSingleton<IChangeLogService, ChangeLogService>();
            services.AddSingleton<IInvoiceService, InvoiceService>();
            services.AddSingleton<IInvoiceItemService, InvoiceItemService>();
            services.AddSingleton<IProductService, ProductService>();
            services.AddSingleton<ViewModels.InvoiceViewModel>();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/log.txt")
                .CreateLogger();

            var provider = services.BuildServiceProvider();
            InitializeDatabase(provider).GetAwaiter().GetResult();
            return provider;
        }

        private static void EnsureConfig()
        {
            var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, "{\n  \"ConnectionString\": \"Data Source=invoice.db\"\n}");
            }
        }

        private static async Task InitializeDatabase(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<InvoiceContext>>();
            using var ctx = factory.CreateDbContext();

            if (ctx.Database.GetMigrations().Any())
            {
                ctx.Database.Migrate();
            }
            else
            {
                ctx.Database.EnsureCreated();
            }

            try
            {
                if (!ctx.Invoices.Any())
                {
                    var result = MessageBox.Show("Telepíti a mintaadatokat?", "Telepítés", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        var service = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
                        for (int i = 1; i <= 17; i++)
                        {
                            await service.SaveAsync(new Invoice
                            {
                                Number = $"INV-{i:000}",
                                Issuer = "Minta Kft.",
                                Date = DateTime.Today.AddDays(-i),
                                Amount = 100 + i
                            });
                        }
                    }
                }
            }
            catch (Microsoft.Data.Sqlite.SqliteException ex)
            {
                Log.Error(ex, "Failed to query Invoices table");
            }
        }
    }
}