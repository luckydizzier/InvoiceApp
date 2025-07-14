# InvoiceApp

This project demonstrates a simple WPF application following an MRS (Model–Repository–Service) and MVVM (View–ViewModel–Model) architecture.

## Structure
- **Models** – POCO classes inheriting from `Base`.
- **Repositories** – data access abstractions (here a mock repository).
- **Services** – business logic using repositories.
- **ViewModels** – UI logic implementing `INotifyPropertyChanged`.
- **Views** – XAML files containing only UI elements.

`StartupOrchestrator` configures dependency injection and Serilog logging.
