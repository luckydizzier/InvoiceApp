using System;
using System.Linq;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace InvoiceApp.ViewModels
{
    /// <summary>
    /// Root view model providing application level navigation commands.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigation;
        private readonly IStatusService _statusService;
        private AppState _current;
        private string _currentStateDescription = string.Empty;
        private string _breadcrumb = string.Empty;
        public InvoiceViewModel InvoiceViewModel { get; }

        public AppState CurrentState
        {
            get => _current;
            private set
            {
                _current = value;
                OnPropertyChanged();
                CurrentStateDescription = value.GetDescription();
            }
        }

        public string CurrentStateDescription
        {
            get => _currentStateDescription;
            private set
            {
                _currentStateDescription = value;
                OnPropertyChanged();
            }
        }

        public string Breadcrumb
        {
            get => _breadcrumb;
            private set
            {
                _breadcrumb = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand BackCommand { get; }
        public RelayCommand NavigateUpCommand { get; }
        public RelayCommand NavigateDownCommand { get; }
        public RelayCommand EnterCommand { get; }
        public RelayCommand DeleteInvoiceCommand { get; }
        public RelayCommand ItemsEnterCommand { get; }
        public RelayCommand ItemsUpCommand { get; }
        public RelayCommand ItemsDownCommand { get; }
        public RelayCommand ItemsEscapeCommand { get; }
        public RelayCommand HeaderEnterCommand { get; }
        public RelayCommand HeaderEscapeCommand { get; }
        public RelayCommand HeaderUpCommand { get; }
        public RelayCommand HeaderDownCommand { get; }
        public RelayCommand SummaryEnterCommand { get; }
        public RelayCommand SummaryEscapeCommand { get; }
        public RelayCommand SummaryUpCommand { get; }
        public RelayCommand SummaryDownCommand { get; }
        public RelayCommand ShowPaymentMethodsCommand { get; }
        public RelayCommand ShowSuppliersCommand { get; }
        public RelayCommand ShowProductGroupsCommand { get; }
        public RelayCommand ShowTaxRatesCommand { get; }
        public RelayCommand ShowUnitsCommand { get; }
        public RelayCommand ShowProductsCommand { get; }
        public RelayCommand ShowDashboardCommand { get; }
        public RelayCommand SwitchPaymentMethodsCommand { get; }
        public RelayCommand SwitchSuppliersCommand { get; }
        public RelayCommand SwitchProductGroupsCommand { get; }
        public RelayCommand SwitchTaxRatesCommand { get; }
        public RelayCommand SwitchUnitsCommand { get; }
        public RelayCommand SwitchProductsCommand { get; }

        public MainViewModel(INavigationService navigation,
                             InvoiceViewModel invoiceViewModel,
                             IStatusService statusService)
        {
            _navigation = navigation;
            InvoiceViewModel = invoiceViewModel;
            _statusService = statusService;
            _current = _navigation.CurrentState;
            _currentStateDescription = _current.GetDescription();
            UpdateBreadcrumb();
            _navigation.StateChanged += OnStateChanged;

            BackCommand = new RelayCommand(_ =>
            {
                var active = GetActiveViewModel();
                if (active is IHasChanges changeTracker && changeTracker.HasChanges)
                {
                    if (!DialogHelper.ShowConfirmation("Mentés nélkül kilép?", "Megerősítés"))
                        return;
                }
                _navigation.Pop();
            });
            NavigateUpCommand = new RelayCommand(_ => InvoiceViewModel.SelectPreviousInvoice());
            NavigateDownCommand = new RelayCommand(_ => InvoiceViewModel.SelectNextInvoice());
            EnterCommand = new RelayCommand(_ => InvoiceViewModel.ToggleRowDetails());
            DeleteInvoiceCommand = new RelayCommand(_ => InvoiceViewModel.DeleteCurrentInvoice());
            ItemsEnterCommand = new RelayCommand(_ =>
            {
                InvoiceViewModel.SaveCurrentItem();
                _navigation.PushSubstate(AppState.Summary);
            });
            ItemsUpCommand = new RelayCommand(_ => InvoiceViewModel.SelectPreviousItem());
            ItemsDownCommand = new RelayCommand(_ => InvoiceViewModel.SelectNextItem());
            ItemsEscapeCommand = new RelayCommand(_ =>
            {
                if (InvoiceViewModel.HasChanges)
                {
                    if (!DialogHelper.ShowConfirmation("Mentés nélkül kilép?", "Megerősítés"))
                        return;
                }
                InvoiceViewModel.CancelItemEdit();
                _navigation.PopSubstate();
            });
            HeaderEnterCommand = new RelayCommand(_ =>
            {
                InvoiceViewModel.SaveCurrentInvoice();
                _navigation.PushSubstate(AppState.ItemList);
            });
            HeaderEscapeCommand = new RelayCommand(_ =>
            {
                if (InvoiceViewModel.HasChanges)
                {
                    if (!DialogHelper.ShowConfirmation("Mentés nélkül kilép?", "Megerősítés"))
                        return;
                }
                InvoiceViewModel.CancelHeaderOrSummary();
                _navigation.PopSubstate();
            });
            HeaderUpCommand = new RelayCommand(_ => InvoiceViewModel.SelectPreviousInvoice());
            HeaderDownCommand = new RelayCommand(_ => InvoiceViewModel.SelectNextInvoice());
            SummaryEnterCommand = new RelayCommand(_ =>
            {
                InvoiceViewModel.SaveCurrentInvoice();
                _navigation.ClearSubstates();
            });
            SummaryEscapeCommand = new RelayCommand(_ =>
            {
                if (InvoiceViewModel.HasChanges)
                {
                    if (!DialogHelper.ShowConfirmation("Mentés nélkül kilép?", "Megerősítés"))
                        return;
                }
                InvoiceViewModel.CancelHeaderOrSummary();
                _navigation.PopSubstate();
            });
            SummaryUpCommand = new RelayCommand(_ => InvoiceViewModel.SelectPreviousInvoice());
            SummaryDownCommand = new RelayCommand(_ => InvoiceViewModel.SelectNextInvoice());
            ShowPaymentMethodsCommand = new RelayCommand(_ => _navigation.Push(AppState.PaymentMethods));
            ShowSuppliersCommand = new RelayCommand(_ => _navigation.Push(AppState.Suppliers));
            ShowProductGroupsCommand = new RelayCommand(_ => _navigation.Push(AppState.ProductGroups));
            ShowTaxRatesCommand = new RelayCommand(_ => _navigation.Push(AppState.TaxRates));
            ShowUnitsCommand = new RelayCommand(_ => _navigation.Push(AppState.Units));
            ShowProductsCommand = new RelayCommand(_ => _navigation.Push(AppState.Products));
            ShowDashboardCommand = new RelayCommand(_ => _navigation.SwitchRoot(AppState.Dashboard));
            SwitchPaymentMethodsCommand = new RelayCommand(_ => _navigation.SwitchRoot(AppState.PaymentMethods));
            SwitchSuppliersCommand = new RelayCommand(_ => _navigation.SwitchRoot(AppState.Suppliers));
            SwitchProductGroupsCommand = new RelayCommand(_ => _navigation.SwitchRoot(AppState.ProductGroups));
            SwitchTaxRatesCommand = new RelayCommand(_ => _navigation.SwitchRoot(AppState.TaxRates));
            SwitchUnitsCommand = new RelayCommand(_ => _navigation.SwitchRoot(AppState.Units));
            SwitchProductsCommand = new RelayCommand(_ => _navigation.SwitchRoot(AppState.Products));
        }

        private void OnStateChanged(object? sender, AppState state)
        {
            CurrentState = state;
            UpdateBreadcrumb();
            InvoiceViewModel.UpdateNavigationStatus(state);
            UpdateStatusHint(state);
        }

        private void UpdateStatusHint(AppState state)
        {
            switch (state)
            {
                case AppState.Products:
                case AppState.ProductGroups:
                case AppState.Suppliers:
                case AppState.TaxRates:
                case AppState.Units:
                case AppState.PaymentMethods:
                    _statusService.Show("Nyomja meg az Esc-et a visszal\u00e9p\u00e9shez");
                    break;
                case AppState.Dashboard:
                case AppState.MainWindow:
                    _statusService.Show(string.Empty);
                    break;
            }
        }

        private void UpdateBreadcrumb()
        {
            var path = _navigation.GetStatePath().ToList();
            if (path.Count == 0)
            {
                Breadcrumb = CurrentStateDescription;
            }
            else
            {
                Breadcrumb = string.Join(" \u203a ", path.Select(s => s.GetDescription()));
            }
        }

        private object? GetActiveViewModel()
        {
            var provider = ((App)Application.Current).Services;
            return CurrentState switch
            {
                AppState.PaymentMethods => provider.GetRequiredService<PaymentMethodViewModel>(),
                AppState.Suppliers => provider.GetRequiredService<SupplierViewModel>(),
                AppState.ProductGroups => provider.GetRequiredService<ProductGroupViewModel>(),
                AppState.TaxRates => provider.GetRequiredService<TaxRateViewModel>(),
                AppState.Units => provider.GetRequiredService<UnitViewModel>(),
                AppState.Products => provider.GetRequiredService<ProductViewModel>(),
                _ => null
            };
        }
    }
}
