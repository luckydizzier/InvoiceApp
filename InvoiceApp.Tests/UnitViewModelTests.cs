using System.Collections.ObjectModel;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvoiceApp.Tests
{
    [TestClass]
    public class UnitViewModelTests
    {
        [TestMethod]
        public void SelectPreviousUnit_AtTop_ShowsConfirmation()
        {
            var vm = new UnitViewModel(new StubService<Unit>(), new StatusService());
            vm.Units = new ObservableCollection<Unit>
            {
                new Unit { Id = 1, Name = "db" }
            };
            vm.SelectedUnit = vm.Units[0];

            bool confirmed = false;
            DialogHelper.ConfirmationHandler = (m, t) => { confirmed = true; return false; };
            try
            {
                vm.SelectPreviousUnit();
            }
            finally
            {
                DialogHelper.ConfirmationHandler = null;
            }

            Assert.IsTrue(confirmed);
        }

        [TestMethod]
        public void SelectPreviousUnit_AtTop_AddsUnit_WhenConfirmed()
        {
            var vm = new UnitViewModel(new StubService<Unit>(), new StatusService());
            vm.Units = new ObservableCollection<Unit>
            {
                new Unit { Id = 1, Name = "db" }
            };
            vm.SelectedUnit = vm.Units[0];

            DialogHelper.ConfirmationHandler = (m, t) => true;
            try
            {
                vm.SelectPreviousUnit();
            }
            finally
            {
                DialogHelper.ConfirmationHandler = null;
            }

            Assert.AreEqual(2, vm.Units.Count);
        }
    }
}
