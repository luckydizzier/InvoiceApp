using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp;

namespace InvoiceApp.ViewModels
{
    public class ProductGroupViewModel : EntityCollectionViewModel<ProductGroup>
    {
        private readonly IProductGroupService _service;
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

        public ProductGroupViewModel(IProductGroupService service)
            : base()
        {
            _service = service;
        }

        public async Task LoadAsync()
        {
            var items = await _service.GetAllAsync();
            Groups = new ObservableCollection<ProductGroup>(items);
        }

        protected override ProductGroup CreateNewItem() => new ProductGroup();

        protected override async Task<bool> DeleteItemAsync(ProductGroup group)
        {
            if (!DialogHelper.ConfirmDeletion("termékcsoportot"))
                return false;
            await _service.DeleteAsync(group.Id);
            DialogHelper.ShowInfo("Törlés sikeres.");
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
