using System;
using InvoiceApp.Models;
using InvoiceApp.Services;

namespace InvoiceApp.ViewModels
{
    /// <summary>
    /// Root view model providing application level navigation commands.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigation;
        private AppState _current;
        public InvoiceViewModel InvoiceViewModel { get; }

        public AppState CurrentState
        {
            get => _current;
            private set
            {
                _current = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand BackCommand { get; }
        public RelayCommand ShowPaymentMethodsCommand { get; }
        public RelayCommand ShowSuppliersCommand { get; }
        public RelayCommand ShowProductGroupsCommand { get; }
        public RelayCommand ShowTaxRatesCommand { get; }
        public RelayCommand ShowUnitsCommand { get; }
        public RelayCommand ShowProductsCommand { get; }
        public RelayCommand ShowInvoicesCommand { get; }

        public MainViewModel(INavigationService navigation, InvoiceViewModel invoiceViewModel)
        {
            _navigation = navigation;
            InvoiceViewModel = invoiceViewModel;
            _current = _navigation.CurrentState;
            _navigation.StateChanged += (_, state) => CurrentState = state;

            BackCommand = new RelayCommand(_ => _navigation.Pop());
            ShowInvoicesCommand = new RelayCommand(_ => _navigation.SwitchRoot(AppState.MainWindow));
            ShowPaymentMethodsCommand = new RelayCommand(_ => _navigation.Push(AppState.PaymentMethod));
            ShowSuppliersCommand = new RelayCommand(_ => _navigation.Push(AppState.Supplier));
            ShowProductGroupsCommand = new RelayCommand(_ => _navigation.Push(AppState.ProductGroup));
            ShowTaxRatesCommand = new RelayCommand(_ => _navigation.Push(AppState.TaxRate));
            ShowUnitsCommand = new RelayCommand(_ => _navigation.Push(AppState.Unit));
            ShowProductsCommand = new RelayCommand(_ => _navigation.Push(AppState.Product));
        }
    }
}
