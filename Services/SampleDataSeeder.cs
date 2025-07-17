using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using InvoiceApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApp.Services
{
    public static class SampleDataSeeder
    {
        public static async Task GenerateAsync(IServiceProvider provider, SampleDataOptions options)
        {
            using var scope = provider.CreateScope();

            var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
            var itemService = scope.ServiceProvider.GetRequiredService<IInvoiceItemService>();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            var unitRepo = scope.ServiceProvider.GetRequiredService<IUnitRepository>();
            var groupRepo = scope.ServiceProvider.GetRequiredService<IProductGroupRepository>();
            var taxRepo = scope.ServiceProvider.GetRequiredService<ITaxRateRepository>();
            var supplierService = scope.ServiceProvider.GetRequiredService<ISupplierService>();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentMethodService>();
            var logService = scope.ServiceProvider.GetRequiredService<IChangeLogService>();

            var faker = new Faker("hu");

            var payment = new PaymentMethod
            {
                Name = "Átutalás",
                DueInDays = 8,
                Active = true,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };
            await paymentService.SaveAsync(payment);

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
                Data = System.Text.Json.JsonSerializer.Serialize(unit),
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
                Data = System.Text.Json.JsonSerializer.Serialize(tax),
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Active = true
            });

            var groups = new List<ProductGroup>();
            for (int i = 0; i < options.ProductGroupCount; i++)
            {
                var group = new ProductGroup
                {
                    Name = faker.Commerce.Categories(1)[0],
                    Active = true,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now
                };
                await groupRepo.AddAsync(group);
                await logService.AddAsync(new ChangeLog
                {
                    Entity = nameof(ProductGroup),
                    Operation = "Add",
                    Data = System.Text.Json.JsonSerializer.Serialize(group),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Active = true
                });
                groups.Add(group);
            }

            var suppliers = new List<Supplier>();
            for (int i = 0; i < options.SupplierCount; i++)
            {
                var supplier = new Supplier
                {
                    Name = faker.Company.CompanyName(),
                    Address = faker.Address.FullAddress(),
                    TaxId = faker.Random.Replace("######-#-##"),
                    BankAccntNr = faker.Finance.Iban(),
                    Active = true,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now
                };
                await supplierService.SaveAsync(supplier);
                suppliers.Add(supplier);
            }

            var products = new List<Product>();
            for (int i = 0; i < options.ProductCount; i++)
            {
                var group = faker.PickRandom(groups);
                var net = faker.Random.Decimal(1000, 10000) / 100m;
                var product = new Product
                {
                    Name = faker.Commerce.ProductName(),
                    Net = net,
                    Gross = Math.Round(net * 1.27m, 2),
                    UnitId = unit.Id,
                    ProductGroupId = group.Id,
                    TaxRateId = tax.Id,
                    Active = true,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now
                };
                await productService.SaveAsync(product);
                products.Add(product);
            }

            for (int i = 1; i <= options.InvoiceCount; i++)
            {
                var supplier = faker.PickRandom(suppliers);
                var inv = new Invoice
                {
                    Number = $"INV-{i:0000}",
                    Issuer = "Minta Kft.",
                    Date = DateTime.Today.AddDays(-i),
                    SupplierId = supplier.Id,
                    Supplier = supplier,
                    PaymentMethodId = payment.Id,
                    PaymentMethod = payment,
                    Active = true,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now
                };
                await invoiceService.SaveAsync(inv);
                decimal amount = 0m;
                for (int j = 0; j < options.ItemsPerInvoice; j++)
                {
                    var product = faker.PickRandom(products);
                    var qty = faker.Random.Decimal(options.ItemQuantityMin, options.ItemQuantityMax);
                    await itemService.SaveAsync(new InvoiceItem
                    {
                        InvoiceId = inv.Id,
                        ProductId = product.Id,
                        TaxRateId = product.TaxRateId,
                        Description = product.Name,
                        Quantity = qty,
                        UnitPrice = product.Gross,
                        Active = true,
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now
                    });
                    amount += qty * product.Gross;
                }
                inv.Amount = amount;
                await invoiceService.SaveAsync(inv);
            }
        }
    }
}
