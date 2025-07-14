using System;

using System.IO;
using System.Linq;
using System.Windows;
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

            services.AddSingleton<IInvoiceService, InvoiceService>();
            services.AddSingleton<ViewModels.InvoiceViewModel>();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/log.txt")
                .CreateLogger();

            var provider = services.BuildServiceProvider();
            InitializeDatabase(provider);
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

        private static void InitializeDatabase(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<InvoiceContext>>();
            using var ctx = factory.CreateDbContext();
            ctx.Database.Migrate();

            if (!ctx.Invoices.Any())
            {
                var result = MessageBox.Show("Telepíti a mintaadatokat?", "Telepítés", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    ctx.Invoices.Add(new Invoice { Number = "INV-001", Date = DateTime.Today, Amount = 100 });
                    ctx.SaveChanges();
                }
            }
        }
    }
}