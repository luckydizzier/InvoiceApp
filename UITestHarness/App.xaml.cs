using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using InvoiceApp;
using InvoiceApp.Views;

namespace UITestHarness
{
    public partial class App : Application
    {
        private System.IServiceProvider _services = null!;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _services = await StartupOrchestrator.Configure();
            await StartupOrchestrator.InitializeDatabaseAsync(_services);

            Window window = e.Args.Contains("dialog")
                ? new InvoiceApp.Views.SampleDataDialog()
                : new InvoiceApp.Views.MainWindow();

            window.Show();
            if (window is MainWindow main)
                MainWindow = main;
        }
    }
}
