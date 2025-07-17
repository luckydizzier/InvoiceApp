using System;
using System.Windows;

namespace InvoiceApp
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Services = StartupOrchestrator.Configure();
            if (StartupOrchestrator.IsNewDatabase)
            {
                var dlg = new Views.SampleDataDialog();
                if (dlg.ShowDialog() == true)
                {
                    StartupOrchestrator.PopulateSampleDataAsync(Services, dlg.Options).GetAwaiter().GetResult();
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
