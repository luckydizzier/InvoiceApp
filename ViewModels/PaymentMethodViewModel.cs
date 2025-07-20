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
    public class PaymentMethodViewModel : MasterDataViewModel<PaymentMethod>, IHasChanges
    {
        private readonly IPaymentMethodService _service;
        private readonly IStatusService _statusService;
        public ObservableCollection<PaymentMethod> Methods
        {
            get => Items;
            set => Items = value;
        }

        public PaymentMethod? SelectedMethod
        {
            get => SelectedItem;
            set => SelectedItem = value;
        }

        public PaymentMethodViewModel(IPaymentMethodService service,
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
                Methods = new ObservableCollection<PaymentMethod>(items);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load payment methods");
                DialogHelper.ShowError("Hiba történt a fizetési módok betöltésekor.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected override PaymentMethod CreateNewItem() => new PaymentMethod();

        protected override async Task<bool> DeleteItemAsync(PaymentMethod method)
        {
            if (!DialogHelper.ConfirmDeletion("fizetési módot"))
                return false;
            await _service.DeleteAsync(method.Id);
            _statusService.Show("Törlés sikeres.");
            return true;
        }

        protected override async Task SaveItemAsync(PaymentMethod method)
        {
            await _service.SaveAsync(method);
            _statusService.Show("Mentés kész.");
        }

        protected override bool CanSaveItem(PaymentMethod? method) => method != null && !string.IsNullOrWhiteSpace(method.Name);

        public void SelectPreviousMethod()
        {
            if (SelectedMethod == null)
            {
                if (Methods.Count > 0)
                    SelectedMethod = Methods[0];
                return;
            }

            var index = Methods.IndexOf(SelectedMethod);
            if (index > 0)
            {
                SelectedMethod = Methods[index - 1];
            }
            else if (index == 0)
            {
                if (DialogHelper.ShowConfirmation("Új fizetési módot szeretnél létrehozni?", "Megerősítés"))
                {
                    AddCommand.Execute(null);
                }
            }
        }
    }
}
