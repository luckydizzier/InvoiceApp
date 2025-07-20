using System;

using System.IO;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using InvoiceApp.Data;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using InvoiceApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using InvoiceApp.DTOs;
using InvoiceApp.Validators;
using Serilog;

namespace InvoiceApp
{
    public static class StartupOrchestrator
    {
        private static readonly string[] ExpectedTables = new[]
        {
            "Invoices",
            "ChangeLogs",
            "InvoiceItems",
            "Products",
            "Suppliers",
            "PaymentMethods",
            "Units",
            "ProductGroups",
            "TaxRates"
        };

        public static bool IsNewDatabase { get; private set; }

        public static async Task<IServiceProvider> Configure()
        {
            EnsureConfig();

            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connection = config["ConnectionString"] ?? $"Data Source={Path.Combine(AppContext.BaseDirectory, "invoice.db")}";

            var services = new ServiceCollection();
            services.AddDbContextFactory<InvoiceContext>(o =>
                o.UseLazyLoadingProxies()
                 .UseSqlite(connection));
            services.AddSingleton<IInvoiceRepository, EfInvoiceRepository>();
            services.AddSingleton<IChangeLogRepository, EfChangeLogRepository>();
            services.AddSingleton<IInvoiceItemRepository, EfInvoiceItemRepository>();
            services.AddSingleton<IProductRepository, EfProductRepository>();
            services.AddSingleton<IPaymentMethodRepository, EfPaymentMethodRepository>();
            services.AddSingleton<ISupplierRepository, EfSupplierRepository>();
            services.AddSingleton<IUnitRepository, EfUnitRepository>();
            services.AddSingleton<IProductGroupRepository, EfProductGroupRepository>();
            services.AddSingleton<ITaxRateRepository, EfTaxRateRepository>();

            services.AddSingleton<IValidator<InvoiceDto>, InvoiceDtoValidator>();
            services.AddSingleton<IValidator<PaymentMethodDto>, PaymentMethodDtoValidator>();
            services.AddSingleton<IValidator<ProductDto>, ProductDtoValidator>();
            services.AddSingleton<IValidator<ProductGroupDto>, ProductGroupDtoValidator>();
            services.AddSingleton<IValidator<TaxRateDto>, TaxRateDtoValidator>();
            services.AddSingleton<IValidator<InvoiceItemDto>, InvoiceItemDtoValidator>();
            services.AddSingleton<IValidator<UnitDto>, UnitDtoValidator>();
            services.AddSingleton<IValidator<SupplierDto>, SupplierDtoValidator>();

            services.AddSingleton<IChangeLogService, ChangeLogService>();
            services.AddSingleton<IInvoiceService, InvoiceService>();
            services.AddSingleton<IInvoiceItemService, InvoiceItemService>();
            services.AddSingleton<IProductService, ProductService>();
            services.AddSingleton<IPaymentMethodService, PaymentMethodService>();
            services.AddSingleton<ISupplierService, SupplierService>();
            services.AddSingleton<IUnitService, UnitService>();
            services.AddSingleton<IProductGroupService, ProductGroupService>();
            services.AddSingleton<ITaxRateService, TaxRateService>();
            services.AddSingleton<IStatusService, StatusService>();

            services.AddSingleton<ViewModels.InvoiceViewModel>();
            services.AddSingleton<ViewModels.ProductViewModel>();
            services.AddSingleton<ViewModels.UnitViewModel>();
            services.AddSingleton<ViewModels.PaymentMethodViewModel>();
            services.AddSingleton<ViewModels.SupplierViewModel>();
            services.AddSingleton<ViewModels.ProductGroupViewModel>();
            services.AddSingleton<ViewModels.TaxRateViewModel>();
            services.AddSingleton<ViewModels.DashboardViewModel>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ViewModels.MainViewModel>();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            var provider = services.BuildServiceProvider();
            return provider;
        }

        private static void EnsureConfig()
        {
            var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "InvoiceApp", "logs");
            Directory.CreateDirectory(logDir);
            if (!File.Exists(configPath))
            {
                var defaultConfig = @"{
  ""ConnectionString"": ""Data Source=invoice.db"",
  ""Serilog"": {
    ""MinimumLevel"": ""Debug"",
    ""WriteTo"": [
      {
        ""Name"": ""File"",
        ""Args"": {
          ""path"": ""%LOCALAPPDATA%\\InvoiceApp\\logs\\log-.json"",
          ""rollingInterval"": ""Day"",
          ""formatter"": ""Serilog.Formatting.Json.JsonFormatter, Serilog"",
          ""fileSizeLimitBytes"": 5242880,
          ""rollOnFileSizeLimit"": true,
          ""retainedFileCountLimit"": 5
        }
      }
    ]
  }
}";
                File.WriteAllText(configPath, defaultConfig);
            }
        }

        private static void BackupDatabase(string dbPath)
        {
            if (!File.Exists(dbPath))
                return;

            var backupDir = Path.Combine(Path.GetDirectoryName(dbPath)!, "backups");
            Directory.CreateDirectory(backupDir);
            var backupFile = Path.Combine(backupDir, $"invoice_{DateTime.Now:yyyyMMddHHmmss}.db");
            File.Copy(dbPath, backupFile, true);

            var oldBackups = Directory.GetFiles(backupDir)
                .OrderByDescending(f => f)
                .Skip(5)
                .ToList();
            foreach (var old in oldBackups)
            {
                File.Delete(old);
            }

            Log.Information($"Database backup created: {backupFile}");
        }

        private static bool TableExists(DbConnection connection, string name)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name=$name";
            var param = cmd.CreateParameter();
            param.ParameterName = "$name";
            param.Value = name;
            cmd.Parameters.Add(param);
            return cmd.ExecuteScalar() != null;
        }

        private static bool IndexExists(DbConnection connection, string name)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='index' AND name=$name";
            var param = cmd.CreateParameter();
            param.ParameterName = "$name";
            param.Value = name;
            cmd.Parameters.Add(param);
            return cmd.ExecuteScalar() != null;
        }

        private static void EnsureIndex(DbConnection conn, string table, string column)
        {
            var indexName = $"IX_{table}_{column}";
            if (!IndexExists(conn, indexName))
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"CREATE INDEX IF NOT EXISTS {indexName} ON {table}({column});";
                cmd.ExecuteNonQuery();
                Log.Information($"Index {indexName} created.");
            }
        }

        private static void RepairTables(InvoiceContext ctx, string dbPath)
        {
            using var conn = ctx.Database.GetDbConnection();
            conn.Open();

            foreach (var table in ExpectedTables)
            {
                if (!TableExists(conn, table))
                {
                    Log.Warning($"Table {table} missing. Recreating via EF.");
                    ctx.Database.EnsureCreated();
                    Log.Information($"Table {table} ensured.");
                }

                EnsureIndex(conn, table, "Id");

                if (table == "Invoices")
                {
                    EnsureIndex(conn, table, "SupplierId");
                    EnsureIndex(conn, table, "Date");
                }
            }

            conn.Close();
        }

        public static async Task InitializeDatabaseAsync(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<InvoiceContext>>();
            using var ctx = factory.CreateDbContext();

            var builder = new SqliteConnectionStringBuilder(ctx.Database.GetDbConnection().ConnectionString);
            var dbPath = Path.GetFullPath(builder.DataSource);

            var isNew = !File.Exists(dbPath);
            if (isNew)
            {
                Log.Warning($"Database file missing at {dbPath}. It will be created.");
            }
            else
            {
                BackupDatabase(dbPath);
            }

            if (ctx.Database.GetMigrations().Any())
            {
                await ctx.Database.MigrateAsync();
            }
            else
            {
                await ctx.Database.EnsureCreatedAsync();
            }

            RepairTables(ctx, dbPath);

            IsNewDatabase = isNew;
        }

        public static bool DatabaseFileExists(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<InvoiceContext>>();
            using var ctx = factory.CreateDbContext();

            var builder = new SqliteConnectionStringBuilder(ctx.Database.GetDbConnection().ConnectionString);
            var dbPath = Path.GetFullPath(builder.DataSource);
            return File.Exists(dbPath);
        }

        public static async Task PopulateSampleDataAsync(IServiceProvider provider, SampleDataOptions options)
        {
            using var scope = provider.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<InvoiceContext>>();
            using var ctx = factory.CreateDbContext();

            if (ctx.Invoices.Any())
            {
                return;
            }

            await SampleDataSeeder.GenerateAsync(provider, options);
        }
    }
}