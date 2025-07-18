using System;
using System.Windows;

namespace InvoiceApp
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; } = null!;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Services = await StartupOrchestrator.Configure();
            if (StartupOrchestrator.IsNewDatabase)
            {
                var dlg = new Views.SampleDataDialog();
                if (dlg.ShowDialog() == true)
                {
                    await StartupOrchestrator.PopulateSampleDataAsync(Services, dlg.Options);
                }
            }
            this.DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show($"Váratlan hiba: {args.Exception.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Serilog.Log.CloseAndFlush();
        }
    }
}
