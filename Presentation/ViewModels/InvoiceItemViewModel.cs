using InvoiceApp.Domain;
using InvoiceApp.Helpers;

namespace InvoiceApp.ViewModels
{
    public class InvoiceItemViewModel : ViewModelBase
    {
        private readonly InvoiceItem _item;

        public InvoiceItemViewModel()
        {
            _item = new InvoiceItem
            {
                Active = true,
                DateCreated = System.DateTime.Now,
                DateUpdated = System.DateTime.Now
            };
            _taxRatePercentage = _item.TaxRate?.Percentage ?? 0m;
        }

        public InvoiceItemViewModel(InvoiceItem item)
        {
            _item = item;
            _taxRatePercentage = item.TaxRate?.Percentage ?? 0m;
        }

        public InvoiceItem Item => _item;

        public int Id
        {
            get => _item.Id;
            set { if (_item.Id != value) { _item.Id = value; OnPropertyChanged(); } }
        }


        private bool _isGross;

        public bool IsGross
        {
            get => _isGross;
            set
            {
                if (_isGross != value)
                {
                    _isGross = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NetAmount));
                    OnPropertyChanged(nameof(VatAmount));
                    OnPropertyChanged(nameof(GrossAmount));
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        public decimal Quantity
        {
            get => _item.Quantity;
            set
            {
                if (_item.Quantity != value)
                {
                    _item.Quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NetAmount));
                    OnPropertyChanged(nameof(VatAmount));
                    OnPropertyChanged(nameof(GrossAmount));
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        public decimal UnitPrice
        {
            get => _item.UnitPrice;
            set
            {
                if (_item.UnitPrice != value)
                {
                    _item.UnitPrice = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NetAmount));
                    OnPropertyChanged(nameof(VatAmount));
                    OnPropertyChanged(nameof(GrossAmount));
                    OnPropertyChanged(nameof(Total));
                }
            }
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
                    _taxRatePercentage = value?.Percentage ?? _taxRatePercentage;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TaxRateId));
                    OnPropertyChanged(nameof(TaxRatePercentage));
                    OnPropertyChanged(nameof(NetAmount));
                    OnPropertyChanged(nameof(VatAmount));
                    OnPropertyChanged(nameof(GrossAmount));
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        private decimal _taxRatePercentage;

        public decimal TaxRatePercentage
        {
            get => _taxRatePercentage;
            set
            {
                if (_taxRatePercentage != value)
                {
                    _taxRatePercentage = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NetAmount));
                    OnPropertyChanged(nameof(VatAmount));
                    OnPropertyChanged(nameof(GrossAmount));
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        private AmountCalculator.Amounts Calculate() =>
            AmountCalculator.Calculate(Quantity, UnitPrice, TaxRatePercentage, IsGross);

        public decimal NetAmount => Calculate().Net;

        public decimal VatAmount => Calculate().Vat;

        public decimal GrossAmount => Calculate().Gross;

        public decimal Total => GrossAmount;
    }
}
