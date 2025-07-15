using System;

using System.IO;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
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
            services.AddSingleton<IPaymentMethodService, PaymentMethodService>();
            services.AddSingleton<ISupplierService, SupplierService>();
            services.AddSingleton<IUnitService, UnitService>();
            services.AddSingleton<IProductGroupService, ProductGroupService>();
            services.AddSingleton<ITaxRateService, TaxRateService>();
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
                        var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
                        var itemService = scope.ServiceProvider.GetRequiredService<IInvoiceItemService>();
                        var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
                        var unitRepo = scope.ServiceProvider.GetRequiredService<IUnitRepository>();
                        var groupRepo = scope.ServiceProvider.GetRequiredService<IProductGroupRepository>();
                        var taxRepo = scope.ServiceProvider.GetRequiredService<ITaxRateRepository>();
                        var logService = scope.ServiceProvider.GetRequiredService<IChangeLogService>();

                        var createdInvoices = new List<Invoice>();
                        for (int i = 1; i <= 17; i++)
                        {
                            var inv = new Invoice
                            {
                                Number = $"INV-{i:000}",
                                Issuer = "Minta Kft.",
                                Date = DateTime.Today.AddDays(-i),
                                Amount = 100 + i
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
            catch (Microsoft.Data.Sqlite.SqliteException ex)
            {
                Log.Error(ex, "Failed to query Invoices table");
            }
        }
    }
}