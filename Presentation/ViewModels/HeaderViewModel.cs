using System;
using System.Collections.ObjectModel;
using InvoiceApp.Domain;

namespace InvoiceApp.ViewModels
{
    /// <summary>
    /// View model for editing invoice header information.
    /// </summary>
    public class HeaderViewModel : ViewModelBase
    {
        private readonly SupplierViewModel _supplierViewModel;
        private readonly Action<string> _showStatus;
        private readonly Func<System.Threading.Tasks.Task> _suggestNumber;
        private readonly Action _raiseSaveChanged;
        private readonly Action _markDirty;
        private readonly Action<bool> _grossChanged;
        private ObservableCollection<Supplier> _suppliers = new();
        private ObservableCollection<PaymentMethod> _paymentMethods = new();
        private Invoice? _selectedInvoice;

        public HeaderViewModel(SupplierViewModel supplierViewModel,
            Action<string> showStatus,
            Func<System.Threading.Tasks.Task> suggestNumber,
            Action raiseSaveChanged,
            Action markDirty,
            Action<bool> grossChanged)
        {
            _supplierViewModel = supplierViewModel;
            _showStatus = showStatus;
            _suggestNumber = suggestNumber;
            _raiseSaveChanged = raiseSaveChanged;
            _markDirty = markDirty;
            _grossChanged = grossChanged;
            AddSupplierCommand = new RelayCommand(_ => AddSupplier());
        }

        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set { _suppliers = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PaymentMethod> PaymentMethods
        {
            get => _paymentMethods;
            set { _paymentMethods = value; OnPropertyChanged(); }
        }

        public Invoice? SelectedInvoice
        {
            get => _selectedInvoice;
            set
            {
                _selectedInvoice = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedSupplier));
                OnPropertyChanged(nameof(SelectedPaymentMethod));
                OnPropertyChanged(nameof(IsGrossCalculation));
            }
        }

        public Supplier? SelectedSupplier
        {
            get => _selectedInvoice?.Supplier;
            set
            {
                if (_selectedInvoice != null && _selectedInvoice.Supplier != value)
                {
                    _selectedInvoice.Supplier = value;
                    _selectedInvoice.SupplierId = value?.Id ?? 0;
                    OnPropertyChanged();
                    _ = _suggestNumber();
                    _raiseSaveChanged();
                    _markDirty();
                }
            }
        }

        public PaymentMethod? SelectedPaymentMethod
        {
            get => _selectedInvoice?.PaymentMethod;
            set
            {
                if (_selectedInvoice != null && _selectedInvoice.PaymentMethod != value)
                {
                    _selectedInvoice.PaymentMethod = value;
                    _selectedInvoice.PaymentMethodId = value?.Id ?? 0;
                    OnPropertyChanged();
                    _raiseSaveChanged();
                    _markDirty();
                }
            }
        }

        public bool IsGrossCalculation
        {
            get => _selectedInvoice?.IsGross ?? false;
            set
            {
                if (_selectedInvoice != null && _selectedInvoice.IsGross != value)
                {
                    _selectedInvoice.IsGross = value;
                    OnPropertyChanged();
                    _grossChanged(value);
                    _markDirty();
                }
            }
        }

        public RelayCommand AddSupplierCommand { get; }

        private void AddSupplier()
        {
            var supplier = _supplierViewModel.AddSupplier();
            Suppliers.Add(supplier);
            SelectedSupplier = supplier;
            _showStatus("Új szállító hozzáadva");
        }

        public void EnsureSupplierExists(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            var existing = Suppliers.FirstOrDefault(s =>
                string.Equals(s.Name, text, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                SelectedSupplier = existing;
                return;
            }
            var supplier = new Supplier
            {
                Name = text,
                Active = true,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };
            Suppliers.Add(supplier);
            SelectedSupplier = supplier;
            _showStatus("Új szállító hozzáadva");
        }
    }
}
