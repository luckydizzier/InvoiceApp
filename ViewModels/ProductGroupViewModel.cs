using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp;
using Serilog;

namespace InvoiceApp.ViewModels
{
    public class ProductGroupViewModel : MasterDataViewModel<ProductGroup>, IHasChanges
    {
        private readonly IProductGroupService _service;
        private readonly IStatusService _statusService;
        public ObservableCollection<ProductGroup> Groups
        {
            get => Items;
            set => Items = value;
        }

        public ProductGroup? SelectedGroup
        {
            get => SelectedItem;
            set => SelectedItem = value;
        }

        public ProductGroupViewModel(IProductGroupService service,
            IStatusService statusService)
            : base(true)
        {
            _service = service;
            _statusService = statusService;
            ClearChanges();
        }

        public new bool HasChanges => base.HasChanges;

        public new void MarkDirty() => base.MarkDirty();

        public new void ClearChanges() => base.ClearChanges();

        public async Task LoadAsync()
        {
            try
            {
                IsLoading = true;
                var items = await _service.GetAllAsync();
                Groups = new ObservableCollection<ProductGroup>(items);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load product groups");
                DialogHelper.ShowError("Hiba történt a termékcsoportok betöltésekor.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected override ProductGroup CreateNewItem() => new ProductGroup();

        protected override async Task<bool> DeleteItemAsync(ProductGroup group)
        {
            if (!DialogHelper.ConfirmDeletion("termékcsoportot"))
                return false;
            await _service.DeleteAsync(group.Id);
            _statusService.Show("Törlés sikeres.");
            return true;
        }

        protected override async Task SaveItemAsync(ProductGroup group)
        {
            await _service.SaveAsync(group);
            DialogHelper.ShowInfo("Mentés kész.");
        }

        protected override bool CanSaveItem(ProductGroup? group) => group != null && !string.IsNullOrWhiteSpace(group.Name);

        public void SelectPreviousGroup()
        {
            if (SelectedGroup == null)
            {
                if (Groups.Count > 0)
                    SelectedGroup = Groups[0];
                return;
            }

            var index = Groups.IndexOf(SelectedGroup);
            if (index > 0)
            {
                SelectedGroup = Groups[index - 1];
            }
            else if (index == 0)
            {
                if (DialogHelper.ShowConfirmation("Új termékcsoportot szeretnél létrehozni?", "Megerősítés"))
                {
                    AddCommand.Execute(null);
                }
            }
        }
    }
}
