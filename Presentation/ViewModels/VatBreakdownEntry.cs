
namespace InvoiceApp.Presentation.ViewModels
{
    public class VatBreakdownEntry : ViewModelBase
    {
        private decimal _rate;
        private decimal _net;
        private decimal _vat;

        public decimal Rate
        {
            get => _rate;
            set { _rate = value; OnPropertyChanged(); }
        }

        public decimal Net
        {
            get => _net;
            set { _net = value; OnPropertyChanged(); }
        }

        public decimal Vat
        {
            get => _vat;
            set { _vat = value; OnPropertyChanged(); }
        }

        public decimal Gross => Net + Vat;
    }
}
