using System;
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

        [TestMethod]
        public void SaveCommand_ReactsToValidationChanges()
        {
            var vm = TestHelpers.CreateInvoiceViewModel();
            var invoice = new Invoice();
            vm.SelectedInvoice = invoice;
            vm.Items = new ObservableCollection<InvoiceItemViewModel>
            {
                new InvoiceItemViewModel(new InvoiceItem
                {
                    Quantity = 1,
                    UnitPrice = 10,
                    ProductId = 1,
                    TaxRate = new TaxRate { Id = 1, Percentage = 27m },
                    TaxRateId = 1
                }) { TaxRatePercentage = 27m }
            };

            bool canExecuteChanged = false;
            vm.SaveCommand.CanExecuteChanged += (_, _) => canExecuteChanged = true;

            Assert.IsFalse(vm.SaveCommand.CanExecute(null));

            invoice.Number = "INV-1";
            invoice.Issuer = "Issuer";
            invoice.Date = DateTime.Today;
            invoice.Supplier = new Supplier { Id = 1 };
            invoice.SupplierId = 1;
            invoice.PaymentMethod = new PaymentMethod { Id = 1 };
            invoice.PaymentMethodId = 1;

            Assert.IsTrue(canExecuteChanged);
            Assert.IsTrue(vm.SaveCommand.CanExecute(null));

            canExecuteChanged = false;
            invoice.Number = string.Empty;

            Assert.IsTrue(canExecuteChanged);
            Assert.IsFalse(vm.SaveCommand.CanExecute(null));
        }
    }
}
