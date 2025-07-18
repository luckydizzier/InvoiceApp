using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp;

namespace InvoiceApp.ViewModels
{
    public class PaymentMethodViewModel : MasterDataViewModel<PaymentMethod>, IHasChanges
    {
        private readonly IPaymentMethodService _service;
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

        public PaymentMethodViewModel(IPaymentMethodService service)
            : base(true)
        {
            _service = service;
            ClearChanges();
        }

        public bool HasChanges => base.HasChanges;

        public void MarkDirty() => base.MarkDirty();

        public void ClearChanges() => base.ClearChanges();

        public async Task LoadAsync()
        {
            var items = await _service.GetAllAsync();
            Methods = new ObservableCollection<PaymentMethod>(items);
        }

        protected override PaymentMethod CreateNewItem() => new PaymentMethod();

        protected override async Task<bool> DeleteItemAsync(PaymentMethod method)
        {
            if (!DialogHelper.ConfirmDeletion("fizetési módot"))
                return false;
            await _service.DeleteAsync(method.Id);
            DialogHelper.ShowInfo("Törlés sikeres.");
            return true;
        }

        protected override async Task SaveItemAsync(PaymentMethod method)
        {
            await _service.SaveAsync(method);
            DialogHelper.ShowInfo("Mentés kész.");
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
