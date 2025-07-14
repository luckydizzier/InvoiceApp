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
            this.DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show($"Váratlan hiba: {args.Exception.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };
        }
    }
}
