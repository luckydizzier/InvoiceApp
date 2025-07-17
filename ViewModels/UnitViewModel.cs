using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp;

namespace InvoiceApp.ViewModels
{
    public class UnitViewModel : ViewModelBase
    {
        private readonly IUnitService _service;
        private ObservableCollection<Unit> _units = new();
        private Unit? _selectedUnit;

        public ObservableCollection<Unit> Units
        {
            get => _units;
            set { _units = value; OnPropertyChanged(); }
        }

        public Unit? SelectedUnit
        {
            get => _selectedUnit;
            set
            {
                _selectedUnit = value;
                OnPropertyChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }

        public UnitViewModel(IUnitService service)
        {
            _service = service;
            AddCommand = new RelayCommand(_ => AddUnit());
            DeleteCommand = new RelayCommand(async obj =>
            {
                if (obj is Unit unit && DialogHelper.ConfirmDeletion("mértékegységet"))
                {
                    await DeleteUnitAsync(unit);
                    DialogHelper.ShowInfo("Törlés sikeres.");
                }
            }, _ => SelectedUnit != null);
            SaveCommand = new RelayCommand(async _ =>
            {
                await SaveSelectedAsync();
                DialogHelper.ShowInfo("Mentés kész.");
            }, _ => SelectedUnit != null && !string.IsNullOrWhiteSpace(SelectedUnit?.Name));
        }

        public async Task LoadAsync()
        {
            var items = await _service.GetAllAsync();
            Units = new ObservableCollection<Unit>(items);
        }

        private void AddUnit()
        {
            var unit = new Unit();
            Units.Add(unit);
            SelectedUnit = unit;
        }

        private async Task DeleteUnitAsync(Unit unit)
        {
            await _service.DeleteAsync(unit.Id);
            Units.Remove(unit);
        }

        private async Task SaveSelectedAsync()
        {
            if (SelectedUnit != null)
            {
                await _service.SaveAsync(SelectedUnit);
            }
        }

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
