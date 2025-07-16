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
            services.AddSingleton<IPaymentMethodService, PaymentMethodService>();
            services.AddSingleton<ISupplierService, SupplierService>();
            services.AddSingleton<IUnitService, UnitService>();
            services.AddSingleton<IProductGroupService, ProductGroupService>();
            services.AddSingleton<ITaxRateService, TaxRateService>();

            services.AddSingleton<ViewModels.InvoiceViewModel>();
            services.AddSingleton<ViewModels.ProductViewModel>();
            services.AddSingleton<ViewModels.UnitViewModel>();
            services.AddSingleton<ViewModels.PaymentMethodViewModel>();
            services.AddSingleton<ViewModels.SupplierViewModel>();
            services.AddSingleton<ViewModels.ProductGroupViewModel>();
            services.AddSingleton<ViewModels.TaxRateViewModel>();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            var provider = services.BuildServiceProvider();
            InitializeDatabase(provider).GetAwaiter().GetResult();
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

                var indexName = $"IX_{table}_Id";
                if (!IndexExists(conn, indexName))
                {
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = $"CREATE INDEX IF NOT EXISTS {indexName} ON {table}(Id);";
                    cmd.ExecuteNonQuery();
                    Log.Information($"Index {indexName} created.");
                }
            }

            conn.Close();
        }

        private static async Task InitializeDatabase(IServiceProvider provider)
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
                ctx.Database.Migrate();
            }
            else
            {
                ctx.Database.EnsureCreated();
            }

            RepairTables(ctx, dbPath);

            IsNewDatabase = isNew;
        }

        public static async Task PopulateSampleDataAsync(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<InvoiceContext>>();
            using var ctx = factory.CreateDbContext();

            if (ctx.Invoices.Any())
            {
                return;
            }

            var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
            var itemService = scope.ServiceProvider.GetRequiredService<IInvoiceItemService>();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            var unitRepo = scope.ServiceProvider.GetRequiredService<IUnitRepository>();
            var groupRepo = scope.ServiceProvider.GetRequiredService<IProductGroupRepository>();
            var taxRepo = scope.ServiceProvider.GetRequiredService<ITaxRateRepository>();
            var supplierService = scope.ServiceProvider.GetRequiredService<ISupplierService>();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentMethodService>();
            var logService = scope.ServiceProvider.GetRequiredService<IChangeLogService>();

            var supplier = new Supplier
            {
                Name = "Minta beszállító",
                Active = true,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };
            await supplierService.SaveAsync(supplier);

            var payment = new PaymentMethod
            {
                Name = "Átutalás",
                DueInDays = 8,
                Active = true,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };
            await paymentService.SaveAsync(payment);

            var createdInvoices = new List<Invoice>();
            for (int i = 1; i <= 17; i++)
            {
                var inv = new Invoice
                {
                    Number = $"INV-{i:000}",
                    Issuer = "Minta Kft.",
                    Date = DateTime.Today.AddDays(-i),
                    Amount = 100 + i,
                    SupplierId = supplier.Id,
                    Supplier = supplier,
                    PaymentMethodId = payment.Id,
                    PaymentMethod = payment
                };

                await invoiceService.SaveAsync(inv);
                createdInvoices.Add(inv);
            }

            var unit = new Unit
            {
                Code = "db",
                Name = "Darab",
                Active = true,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };
            await unitRepo.AddAsync(unit);
            await logService.AddAsync(new ChangeLog
            {
                Entity = nameof(Unit),
                Operation = "Add",
                Data = JsonSerializer.Serialize(unit),
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Active = true
            });

            var group = new ProductGroup
            {
                Name = "Általános",
                Active = true,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };
            await groupRepo.AddAsync(group);
            await logService.AddAsync(new ChangeLog
            {
                Entity = nameof(ProductGroup),
                Operation = "Add",
                Data = JsonSerializer.Serialize(group),
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Active = true
            });

            var tax = new TaxRate
            {
                Name = "ÁFA 27%",
                Percentage = 27,
                EffectiveFrom = DateTime.Today.AddYears(-1),
                Active = true,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };
            await taxRepo.AddAsync(tax);
            await logService.AddAsync(new ChangeLog
            {
                Entity = nameof(TaxRate),
                Operation = "Add",
                Data = JsonSerializer.Serialize(tax),
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Active = true
            });

            var product = new Product
            {
                Name = "Minta termék",
                Net = 100,
                Gross = 127,
                UnitId = unit.Id,
                ProductGroupId = group.Id,
                TaxRateId = tax.Id
            };
            await productService.SaveAsync(product);

            foreach (var inv in createdInvoices)
            {
                await itemService.SaveAsync(new InvoiceItem
                {
                    InvoiceId = inv.Id,
                    ProductId = product.Id,
                    TaxRateId = tax.Id,
                    Description = product.Name,
                    Quantity = 1,
                    UnitPrice = inv.Amount
                });
            }
        }
    }
}