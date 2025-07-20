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
        private static IServiceProvider? ServiceProvider =>
            (Application.Current as App)?.Services;

        public MainViewModel? MainViewModel =>
            ServiceProvider?.GetService<MainViewModel>();
        public DashboardViewModel? DashboardViewModel =>
            ServiceProvider?.GetService<DashboardViewModel>();
        public InvoiceViewModel? InvoiceViewModel =>
            ServiceProvider?.GetService<InvoiceViewModel>();
        public ProductViewModel? ProductViewModel =>
            ServiceProvider?.GetService<ProductViewModel>();
        public PaymentMethodViewModel? PaymentMethodViewModel =>
            ServiceProvider?.GetService<PaymentMethodViewModel>();
        public SupplierViewModel? SupplierViewModel =>
            ServiceProvider?.GetService<SupplierViewModel>();
        public UnitViewModel? UnitViewModel =>
            ServiceProvider?.GetService<UnitViewModel>();
        public ProductGroupViewModel? ProductGroupViewModel =>
            ServiceProvider?.GetService<ProductGroupViewModel>();
        public TaxRateViewModel? TaxRateViewModel =>
            ServiceProvider?.GetService<TaxRateViewModel>();
        public IStatusService? StatusService =>
            ServiceProvider?.GetService<IStatusService>();
    }
}
