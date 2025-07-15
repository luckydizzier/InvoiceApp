using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using InvoiceApp.Models;
using InvoiceApp.Services;

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
            set { _selectedSupplier = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public SupplierViewModel(ISupplierService service)
        {
            _service = service;
            AddCommand = new RelayCommand(_ => AddSupplier());
            DeleteCommand = new RelayCommand(async obj =>
            {
                if (obj is Supplier supplier)
                {
                    await DeleteSupplierAsync(supplier);
                }
            });
            SaveCommand = new RelayCommand(async _ => await SaveSelectedAsync());
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
