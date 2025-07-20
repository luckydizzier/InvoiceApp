using System;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using InvoiceApp.Domain;
using InvoiceApp.Application.Services;
using InvoiceApp.Shared;

namespace InvoiceApp.Presentation.ViewModels
{
    /// <summary>
    /// Command that dispatches the Insert key to the appropriate "add" command
    /// based on the current <see cref="AppState"/>.
    /// </summary>
    public sealed class StateCommand : ICommand
    {
        private readonly INavigationService _navigation;
        private readonly IServiceProvider _provider;

        public StateCommand(INavigationService navigation, IServiceProvider provider)
        {
            _navigation = navigation;
            _provider = provider;
        }

        public event EventHandler? CanExecuteChanged { add { } remove { } }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            var state = _navigation.CurrentState;
            switch (state)
            {
                case AppState.MainWindow:
                case AppState.Dashboard:
                    _provider.GetRequiredService<InvoiceViewModel>().NewInvoiceCommand.Execute(null);
                    break;
                case AppState.ItemList:
                    _provider.GetRequiredService<InvoiceViewModel>().AddItemCommand.Execute(null);
                    break;
                case AppState.Products:
                    _provider.GetRequiredService<ProductViewModel>().AddCommand.Execute(null);
                    break;
                case AppState.ProductGroups:
                    _provider.GetRequiredService<ProductGroupViewModel>().AddCommand.Execute(null);
                    break;
                case AppState.Suppliers:
                    _provider.GetRequiredService<SupplierViewModel>().AddCommand.Execute(null);
                    break;
                case AppState.TaxRates:
                    _provider.GetRequiredService<TaxRateViewModel>().AddCommand.Execute(null);
                    break;
                case AppState.Units:
                    _provider.GetRequiredService<UnitViewModel>().AddCommand.Execute(null);
                    break;
                case AppState.PaymentMethods:
                    _provider.GetRequiredService<PaymentMethodViewModel>().AddCommand.Execute(null);
                    break;
                default:
                    break;
            }
        }
    }
}
