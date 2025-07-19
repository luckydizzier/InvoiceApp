using System;
using System.Windows;
using InvoiceApp.ViewModels;

namespace InvoiceApp
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; } = null!;

        public App()
        {
            InitializeComponent();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show($"Váratlan hiba: {args.Exception.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };

            Services = await StartupOrchestrator.Configure();

            if (!StartupOrchestrator.DatabaseFileExists(Services))
            {
                var dlg = new Views.SampleDataDialog();
                if (dlg.ShowDialog() == true)
                {
                    await StartupOrchestrator.InitializeDatabaseAsync(Services);
                    await StartupOrchestrator.PopulateSampleDataAsync(Services, dlg.Options);
                }
                else
                {
                    Shutdown();
                    return;
                }
            }
            else
            {
                await StartupOrchestrator.InitializeDatabaseAsync(Services);
            }

            var locator = new ViewModels.ViewModelLocator();
            await locator.PaymentMethodViewModel.LoadAsync();
            await locator.SupplierViewModel.LoadAsync();
            await locator.UnitViewModel.LoadAsync();
            await locator.ProductGroupViewModel.LoadAsync();
            await locator.TaxRateViewModel.LoadAsync();
            await locator.ProductViewModel.LoadAsync();
            await locator.InvoiceViewModel.LoadAsync();
            await locator.DashboardViewModel.LoadAsync();

            var main = new Views.MainWindow();
            MainWindow = main;
            main.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Serilog.Log.CloseAndFlush();
        }
    }
}
