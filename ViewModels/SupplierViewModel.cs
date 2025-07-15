using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp;

namespace InvoiceApp.ViewModels
{
    public class SupplierViewModel : ViewModelBase
    {
        private readonly ISupplierService _service;
        private ObservableCollection<Supplier> _suppliers = new();
        private Supplier? _selectedSupplier;

        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set { _suppliers = value; OnPropertyChanged(); }
        }

        public Supplier? SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                _selectedSupplier = value;
                OnPropertyChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }

        public SupplierViewModel(ISupplierService service)
        {
            _service = service;
            AddCommand = new RelayCommand(_ => AddSupplier());
            DeleteCommand = new RelayCommand(async obj =>
            {
                if (obj is Supplier supplier && DialogHelper.ConfirmDeletion("szállítót"))
                {
                    await DeleteSupplierAsync(supplier);
                    DialogHelper.ShowInfo("Törlés sikeres.");
                }
            }, _ => SelectedSupplier != null);
            SaveCommand = new RelayCommand(async _ =>
            {
                await SaveSelectedAsync();
                DialogHelper.ShowInfo("Mentés kész.");
            }, _ => SelectedSupplier != null && !string.IsNullOrWhiteSpace(SelectedSupplier?.Name));
        }

        public async Task LoadAsync()
        {
            var items = await _service.GetAllAsync();
            Suppliers = new ObservableCollection<Supplier>(items);
        }

        private void AddSupplier()
        {
            var supplier = new Supplier();
            Suppliers.Add(supplier);
            SelectedSupplier = supplier;
        }

        private async Task DeleteSupplierAsync(Supplier supplier)
        {
            await _service.DeleteAsync(supplier.Id);
            Suppliers.Remove(supplier);
        }

        private async Task SaveSelectedAsync()
        {
            if (SelectedSupplier != null)
            {
                await _service.SaveAsync(SelectedSupplier);
            }
        }
    }
}
