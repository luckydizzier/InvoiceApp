using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp;
using InvoiceApp.Resources;
using Serilog;

namespace InvoiceApp.ViewModels
{
    public class UnitViewModel : MasterDataViewModel<Unit>, IHasChanges
    {
        private readonly IUnitService _service;
        private readonly IStatusService _statusService;
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

        public UnitViewModel(IUnitService service,
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
                Units = new ObservableCollection<Unit>(items);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load units");
                DialogHelper.ShowError(Resources.Strings.UnitLoadError);
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected override Unit CreateNewItem() => new Unit();

        protected override async Task<bool> DeleteItemAsync(Unit unit)
        {
            if (!DialogHelper.ConfirmDeletion("mértékegységet"))
                return false;
            await _service.DeleteAsync(unit.Id);
            _statusService.Show("Törlés sikeres.");
            return true;
        }

        protected override async Task SaveItemAsync(Unit unit)
        {
            await _service.SaveAsync(unit);
            _statusService.Show("Mentés kész.");
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
