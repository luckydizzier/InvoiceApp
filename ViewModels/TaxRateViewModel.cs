using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp;

namespace InvoiceApp.ViewModels
{
    public class TaxRateViewModel : MasterDataViewModel<TaxRate>, IHasChanges
    {
        private readonly ITaxRateService _service;
        private readonly IStatusService _statusService;
        public ObservableCollection<TaxRate> Rates
        {
            get => Items;
            set => Items = value;
        }

        public TaxRate? SelectedRate
        {
            get => SelectedItem;
            set => SelectedItem = value;
        }

        public TaxRateViewModel(ITaxRateService service,
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
            var items = await _service.GetAllAsync();
            Rates = new ObservableCollection<TaxRate>(items);
        }

        protected override TaxRate CreateNewItem() => new TaxRate();

        protected override async Task<bool> DeleteItemAsync(TaxRate rate)
        {
            if (!DialogHelper.ConfirmDeletion("áfakulcsot"))
                return false;
            await _service.DeleteAsync(rate.Id);
            _statusService.Show("Törlés sikeres.");
            return true;
        }

        protected override async Task SaveItemAsync(TaxRate rate)
        {
            await _service.SaveAsync(rate);
            DialogHelper.ShowInfo("Mentés kész.");
        }

        protected override bool CanSaveItem(TaxRate? rate) => rate != null && !string.IsNullOrWhiteSpace(rate.Name);

        public void SelectPreviousRate()
        {
            if (SelectedRate == null)
            {
                if (Rates.Count > 0)
                    SelectedRate = Rates[0];
                return;
            }

            var index = Rates.IndexOf(SelectedRate);
            if (index > 0)
            {
                SelectedRate = Rates[index - 1];
            }
            else if (index == 0)
            {
                if (DialogHelper.ShowConfirmation("Új áfakulcsot szeretnél létrehozni?", "Megerősítés"))
                {
                    AddCommand.Execute(null);
                }
            }
        }
    }
}
