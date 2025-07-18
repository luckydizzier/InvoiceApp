using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp;

namespace InvoiceApp.ViewModels
{
    public class UnitViewModel : EntityCollectionViewModel<Unit>
    {
        private readonly IUnitService _service;
        public ObservableCollection<Unit> Units
        {
            get => Items;
            set => Items = value;
        }

        public Unit? SelectedUnit
        {
            get => SelectedItem;
            set => SelectedItem = value;
        }

        public UnitViewModel(IUnitService service)
            : base()
        {
            _service = service;
        }

        public async Task LoadAsync()
        {
            var items = await _service.GetAllAsync();
            Units = new ObservableCollection<Unit>(items);
        }

        protected override Unit CreateNewItem() => new Unit();

        protected override async Task<bool> DeleteItemAsync(Unit unit)
        {
            if (!DialogHelper.ConfirmDeletion("mértékegységet"))
                return false;
            await _service.DeleteAsync(unit.Id);
            DialogHelper.ShowInfo("Törlés sikeres.");
            return true;
        }

        protected override async Task SaveItemAsync(Unit unit)
        {
            await _service.SaveAsync(unit);
            DialogHelper.ShowInfo("Mentés kész.");
        }

        protected override bool CanSaveItem(Unit? unit) => unit != null && !string.IsNullOrWhiteSpace(unit.Name);

        public void SelectPreviousUnit()
        {
            if (SelectedUnit == null)
            {
                if (Units.Count > 0)
                    SelectedUnit = Units[0];
                return;
            }

            var index = Units.IndexOf(SelectedUnit);
            if (index > 0)
            {
                SelectedUnit = Units[index - 1];
            }
            else if (index == 0)
            {
                if (DialogHelper.ShowConfirmation("Új mértékegységet szeretnél létrehozni?", "Megerősítés"))
                {
                    AddCommand.Execute(null);
                }
            }
        }
    }
}
