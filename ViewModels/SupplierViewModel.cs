using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp;

namespace InvoiceApp.ViewModels
{
    public class SupplierViewModel : MasterDataViewModel<Supplier>, IHasChanges
    {
        public new RelayCommand SaveCommand { get; }
        private readonly ISupplierService _service;
        private readonly IStatusService _statusService;
        public ObservableCollection<Supplier> Suppliers
        {
            get => Items;
            set => Items = value;
        }

        public Supplier? SelectedSupplier
        {
            get => SelectedItem;
            set => SelectedItem = value;
        }

        public SupplierViewModel(ISupplierService service,
            IStatusService statusService)
            : base(true)
        {
            _service = service;
            _statusService = statusService;
            ClearChanges();
            SaveCommand = new RelayCommand(async _ =>
            {
                if (SelectedSupplier != null)
                {
                    await SaveItemAsync(SelectedSupplier);
                    AfterSave(SelectedSupplier);
                    ClearChanges();
                }
            }, _ => SelectedSupplier != null && HasChanges && CanSaveItem(SelectedSupplier));
        }

        public new bool HasChanges => base.HasChanges;

        public new void MarkDirty() => base.MarkDirty();

        public new void ClearChanges() => base.ClearChanges();

        public async Task LoadAsync()
        {
            var items = await _service.GetAllAsync();
            Suppliers = new ObservableCollection<Supplier>(items);
        }

        public Supplier AddSupplier()
        {
            AddCommand.Execute(null);
            return SelectedSupplier!;
        }

        protected override Supplier CreateNewItem()
        {
            return new Supplier
            {
                Active = true,
                DateCreated = System.DateTime.Now,
                DateUpdated = System.DateTime.Now
            };
        }

        protected override async Task<bool> DeleteItemAsync(Supplier supplier)
        {
            if (!DialogHelper.ConfirmDeletion("szállítót"))
                return false;
            await _service.DeleteAsync(supplier.Id);
            _statusService.Show("Törlés sikeres.");
            return true;
        }

        protected override async Task SaveItemAsync(Supplier supplier)
        {
            await _service.SaveAsync(supplier);
            DialogHelper.ShowInfo("Mentés kész.");
        }

        protected override bool CanSaveItem(Supplier? supplier) => supplier != null && !string.IsNullOrWhiteSpace(supplier.Name);

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
