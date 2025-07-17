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
        private bool _hasChanges;

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

        public bool HasChanges
        {
            get => _hasChanges;
            private set { _hasChanges = value; OnPropertyChanged(); }
        }

        public void MarkDirty()
        {
            HasChanges = true;
            SaveCommand.RaiseCanExecuteChanged();
        }

        public void ClearChanges()
        {
            HasChanges = false;
            SaveCommand.RaiseCanExecuteChanged();
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
                ClearChanges();
                DialogHelper.ShowInfo("Mentés kész.");
            }, _ => SelectedSupplier != null && !string.IsNullOrWhiteSpace(SelectedSupplier?.Name));

            ClearChanges();
        }

        public async Task LoadAsync()
        {
            var items = await _service.GetAllAsync();
            Suppliers = new ObservableCollection<Supplier>(items);
        }

        public Supplier AddSupplier()
        {
            var supplier = new Supplier
            {
                Active = true,
                DateCreated = System.DateTime.Now,
                DateUpdated = System.DateTime.Now
            };
            Suppliers.Add(supplier);
            SelectedSupplier = supplier;
            MarkDirty();
            return supplier;
        }

        private async Task DeleteSupplierAsync(Supplier supplier)
        {
            await _service.DeleteAsync(supplier.Id);
            Suppliers.Remove(supplier);
            MarkDirty();
        }

        private async Task SaveSelectedAsync()
        {
            if (SelectedSupplier != null)
            {
                await _service.SaveAsync(SelectedSupplier);
            }
        }

        public void SelectPreviousSupplier()
        {
            if (SelectedSupplier == null)
            {
                if (Suppliers.Count > 0)
                    SelectedSupplier = Suppliers[0];
                return;
            }

            var index = Suppliers.IndexOf(SelectedSupplier);
            if (index > 0)
            {
                SelectedSupplier = Suppliers[index - 1];
            }
            else if (index == 0)
            {
                if (DialogHelper.ShowConfirmation("Új szállítót szeretnél létrehozni?", "Megerősítés"))
                {
                    AddCommand.Execute(null);
                }
            }
        }
    }
}
