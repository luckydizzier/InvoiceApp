using System;
using InvoiceApp.Repositories;
using InvoiceApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace InvoiceApp
{
    public static class StartupOrchestrator
    {
        public static IServiceProvider Configure()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IInvoiceRepository, MockInvoiceRepository>();
            services.AddSingleton<IInvoiceService, InvoiceService>();
            services.AddSingleton<ViewModels.InvoiceViewModel>();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/log.txt")
                .CreateLogger();

            return services.BuildServiceProvider();
        }
    }
}
