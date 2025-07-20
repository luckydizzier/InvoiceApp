using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using InvoiceApp.Services;

namespace InvoiceApp.ViewModels
{
    /// <summary>
    /// Resolves view models from the application's service provider.
    /// </summary>
    public class ViewModelLocator
    {
        private static IServiceProvider ServiceProvider => ((App)Application.Current).Services;

        public MainViewModel MainViewModel => ServiceProvider.GetRequiredService<MainViewModel>();
        public DashboardViewModel DashboardViewModel => ServiceProvider.GetRequiredService<DashboardViewModel>();
        public InvoiceViewModel InvoiceViewModel => ServiceProvider.GetRequiredService<InvoiceViewModel>();
        public ProductViewModel ProductViewModel => ServiceProvider.GetRequiredService<ProductViewModel>();
        public PaymentMethodViewModel PaymentMethodViewModel => ServiceProvider.GetRequiredService<PaymentMethodViewModel>();
        public SupplierViewModel SupplierViewModel => ServiceProvider.GetRequiredService<SupplierViewModel>();
        public UnitViewModel UnitViewModel => ServiceProvider.GetRequiredService<UnitViewModel>();
        public ProductGroupViewModel ProductGroupViewModel => ServiceProvider.GetRequiredService<ProductGroupViewModel>();
        public TaxRateViewModel TaxRateViewModel => ServiceProvider.GetRequiredService<TaxRateViewModel>();
        public IStatusService StatusService => ServiceProvider.GetRequiredService<IStatusService>();
    }
}
