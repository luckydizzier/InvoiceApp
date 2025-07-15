using InvoiceApp.Models;

namespace InvoiceApp.ViewModels
{
    public class InvoiceItemViewModel : ViewModelBase
    {
        private readonly InvoiceItem _item;

        public InvoiceItemViewModel(InvoiceItem item)
        {
            _item = item;
        }

        public InvoiceItem Item => _item;

        public int Id
        {
            get => _item.Id;
            set { if (_item.Id != value) { _item.Id = value; OnPropertyChanged(); } }
        }

        public string? Description
        {
            get => _item.Description;
            set { if (_item.Description != value) { _item.Description = value; OnPropertyChanged(); } }
        }

        public decimal Quantity
        {
            get => _item.Quantity;
            set { if (_item.Quantity != value) { _item.Quantity = value; OnPropertyChanged(); } }
        }

        public decimal UnitPrice
        {
            get => _item.UnitPrice;
            set { if (_item.UnitPrice != value) { _item.UnitPrice = value; OnPropertyChanged(); } }
        }

        public decimal Deposit
        {
            get => _item.Deposit;
            set { if (_item.Deposit != value) { _item.Deposit = value; OnPropertyChanged(); } }
        }

        public decimal Return
        {
            get => _item.Return;
            set { if (_item.Return != value) { _item.Return = value; OnPropertyChanged(); } }
        }

        public int InvoiceId
        {
            get => _item.InvoiceId;
            set { if (_item.InvoiceId != value) { _item.InvoiceId = value; OnPropertyChanged(); } }
        }

        public int ProductId
        {
            get => _item.ProductId;
            set { if (_item.ProductId != value) { _item.ProductId = value; OnPropertyChanged(); } }
        }

        public int TaxRateId
        {
            get => _item.TaxRateId;
            set { if (_item.TaxRateId != value) { _item.TaxRateId = value; OnPropertyChanged(); } }
        }

        public TaxRate? TaxRate
        {
            get => _item.TaxRate;
            set
            {
                if (_item.TaxRate != value)
                {
                    _item.TaxRate = value;
                    _item.TaxRateId = value?.Id ?? 0;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TaxRateId));
                }
            }
        }
    }
}
