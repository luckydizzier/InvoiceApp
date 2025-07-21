using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using InvoiceApp.Presentation.ViewModels;
using InvoiceApp.Shared.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvoiceApp.Tests
{
    [TestClass]
    public class DataGridFocusBehaviorTests
    {
        [TestMethod]
        public void MoveFocus_AddsNewRow_WhenAtLastItem()
        {
            var items = new ObservableCollection<int> { 1 };
            var context = new StubContext(items);
            var grid = new DataGrid
            {
                CanUserAddRows = true,
                ItemsSource = items,
                DataContext = context
            };
            grid.Columns.Add(new DataGridTextColumn());
            grid.CurrentItem = items[^1];
            grid.CurrentColumn = grid.Columns[0];

            DataGridFocusBehavior.MoveFocus(grid);

            Assert.AreEqual(2, items.Count);
        }

        private class StubContext
        {
            public ObservableCollection<int> Items { get; }
            public ICommand AddItemCommand { get; }

            public StubContext(ObservableCollection<int> items)
            {
                Items = items;
                AddItemCommand = new RelayCommand(_ => Items.Add(0));
            }
        }
    }
}
