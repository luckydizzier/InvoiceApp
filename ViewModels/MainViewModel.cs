using System;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp;

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
        public RelayCommand NavigateUpCommand { get; }
        public RelayCommand NavigateDownCommand { get; }
        public RelayCommand EnterCommand { get; }
        public RelayCommand DeleteInvoiceCommand { get; }
        public RelayCommand ItemsEnterCommand { get; }
        public RelayCommand ItemsDeleteCommand { get; }
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
        public RelayCommand ShowInvoiceListCommand { get; }
        public RelayCommand StartInvoiceEditorCommand { get; }
        public RelayCommand SwitchPaymentMethodsCommand { get; }
        public RelayCommand SwitchSuppliersCommand { get; }
        public RelayCommand SwitchProductGroupsCommand { get; }
        public RelayCommand SwitchTaxRatesCommand { get; }
        public RelayCommand SwitchUnitsCommand { get; }
        public RelayCommand SwitchProductsCommand { get; }

        public MainViewModel(INavigationService navigation, InvoiceViewModel invoiceViewModel)
        {
            _navigation = navigation;
            InvoiceViewModel = invoiceViewModel;
            _current = _navigation.CurrentState;
            _navigation.StateChanged += (_, state) => CurrentState = state;

            BackCommand = new RelayCommand(_ => _navigation.Pop());
            NavigateUpCommand = new RelayCommand(_ => InvoiceViewModel.SelectPreviousInvoice());
            NavigateDownCommand = new RelayCommand(_ => InvoiceViewModel.SelectNextInvoice());
            EnterCommand = new RelayCommand(_ => InvoiceViewModel.ToggleRowDetails());
            DeleteInvoiceCommand = new RelayCommand(_ => InvoiceViewModel.DeleteCurrentInvoice());
            ItemsEnterCommand = new RelayCommand(_ =>
            {
                InvoiceViewModel.SaveCurrentItem();
                _navigation.PushSubstate(AppState.InvoiceSummary);
            });
            ItemsDeleteCommand = new RelayCommand(_ => InvoiceViewModel.DeleteCurrentItem());
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
                _navigation.PushSubstate(AppState.InvoiceItems);
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
                _navigation.PopSubstate();
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
            ShowPaymentMethodsCommand = new RelayCommand(_ => _navigation.Push(AppState.PaymentMethodView));
            ShowSuppliersCommand = new RelayCommand(_ => _navigation.Push(AppState.Supplier));
            ShowProductGroupsCommand = new RelayCommand(_ => _navigation.Push(AppState.ProductGroup));
            ShowTaxRatesCommand = new RelayCommand(_ => _navigation.Push(AppState.TaxRate));
            ShowUnitsCommand = new RelayCommand(_ => _navigation.Push(AppState.Unit));
            ShowProductsCommand = new RelayCommand(_ => _navigation.Push(AppState.ProductView));
            ShowDashboardCommand = new RelayCommand(_ => _navigation.SwitchRoot(AppState.Dashboard));
            ShowInvoiceListCommand = new RelayCommand(_ => _navigation.SwitchRoot(AppState.InvoiceList));
            StartInvoiceEditorCommand = new RelayCommand(_ =>
            {
                _navigation.SwitchRoot(AppState.InvoiceEditor);
                _navigation.PushSubstate(AppState.InvoiceHeader);
            });
            SwitchPaymentMethodsCommand = new RelayCommand(_ => _navigation.SwitchRoot(AppState.PaymentMethodView));
            SwitchSuppliersCommand = new RelayCommand(_ => _navigation.SwitchRoot(AppState.Supplier));
            SwitchProductGroupsCommand = new RelayCommand(_ => _navigation.SwitchRoot(AppState.ProductGroup));
            SwitchTaxRatesCommand = new RelayCommand(_ => _navigation.SwitchRoot(AppState.TaxRate));
            SwitchUnitsCommand = new RelayCommand(_ => _navigation.SwitchRoot(AppState.Unit));
            SwitchProductsCommand = new RelayCommand(_ => _navigation.SwitchRoot(AppState.ProductView));
        }
    }
}
