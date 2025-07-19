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

            var main = new Views.MainWindow();
            main.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Serilog.Log.CloseAndFlush();
        }
    }
}
