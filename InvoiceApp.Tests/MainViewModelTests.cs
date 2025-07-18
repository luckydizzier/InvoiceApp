using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvoiceApp.ViewModels;
using InvoiceApp.Models;
using InvoiceApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace InvoiceApp.Tests
{
    internal class StubNavigationService : INavigationService
    {
        public AppState CurrentState { get; set; }
        public bool Popped { get; private set; }
        public event EventHandler<AppState>? StateChanged;
        public void Pop() { Popped = true; }
        public void PopSubstate() { }
        public void Push(AppState state) { CurrentState = state; }
        public void PushSubstate(AppState state) { CurrentState = state; }
        public void ClearSubstates() { }
        public void SwitchRoot(AppState state) { CurrentState = state; }
        public IEnumerable<AppState> GetStatePath() => new[] { CurrentState };
    }

    [TestClass]
    public class MainViewModelTests
    {
        [TestMethod]
        public void BackCommand_ShowsConfirmation_WhenChangesPending()
        {
            var nav = new StubNavigationService { CurrentState = AppState.Suppliers };
            var supplierVm = new SupplierViewModel(new StubService<Supplier> { });
            supplierVm.MarkDirty();
            var invoiceVm = TestHelpers.CreateInvoiceViewModel();

            var services = new ServiceCollection()
                .AddSingleton(supplierVm)
                .BuildServiceProvider();

            var app = new App();
            typeof(App).GetProperty("Services")!.SetValue(app, services);

            bool confirmed = false;
            DialogHelper.ConfirmationHandler = (m, t) => { confirmed = true; return true; };
            try
            {
                var vm = new MainViewModel(nav, invoiceVm);
                vm.BackCommand.Execute(null);
            }
            finally
            {
                DialogHelper.ConfirmationHandler = null;
            }

            Assert.IsTrue(confirmed);
            Assert.IsTrue(nav.Popped);
        }
    }
}
