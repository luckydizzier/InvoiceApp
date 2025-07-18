using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvoiceApp.Models;
using InvoiceApp.ViewModels;

namespace InvoiceApp.Tests
{
    [TestClass]
    public class InvoiceValidationTests
    {
        [TestMethod]
        public void InvoiceIsInvalid_WhenRequiredFieldsMissing()
        {
            var invoice = new Invoice();
            Assert.IsFalse(invoice.IsValid());
        }

        [TestMethod]
        public void ViewModel_PreventsSave_ForInvalidInvoice()
        {
            var vm = TestHelpers.CreateInvoiceViewModel();
            vm.SelectedInvoice = new Invoice();
            vm.Items = new ObservableCollection<InvoiceItemViewModel>
            {
                new InvoiceItemViewModel(new InvoiceItem
                {
                    Quantity = 1,
                    UnitPrice = 10,
                    TaxRate = new TaxRate { Percentage = 27m }
                }) { TaxRatePercentage = 27m }
            };
            Assert.IsFalse(vm.SaveCommand.CanExecute(null));
        }
    }
}
