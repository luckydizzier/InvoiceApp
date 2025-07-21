# InvoiceApp

A small WPF application demonstrating an MRS (Model–Repository–Service) and MVVM architecture.

## Structure
- **Domain** – Entities and core business rules.
- **Application** – Use case orchestration and contracts.
- **Infrastructure** – Data access and integrations.
- **Presentation** – WPF UI (ViewModels and Views).
- **Shared** – Helpers and crosscutting utilities.
The source tree mirrors these layers with top-level folders such as `Application`, `Infrastructure`, `Presentation` and `Shared`.

See [Docs/Architecture.md](Docs/Architecture.md) for a description of each layer and an example workflow.

`StartupOrchestrator` configures dependency injection and Serilog logging. It also ensures a local SQLite database is present. Missing configuration or database files are created automatically. On first launch, users can choose to populate the database with sample data.
## Features
- **Dependency Injection** – Managed by `StartupOrchestrator`.
- **Serilog Logging** – Configured in `StartupOrchestrator`.
- **SQLite Database** – Local database with automatic creation and sample data option.
- **MVVM Pattern** – Clear separation of concerns between UI and business logic.
- **MRS Architecture** – Organized structure for models, repositories, and services.
- **Unit Tests** – Basic tests for repositories and services.
- **Sample Data** – Option to populate the database with sample data on first launch.
- **Sample Data Options** – Configure invoice item quantity range when generating sample data.
- **XAML-Only Views** – Clean separation of UI from logic, using XAML for design.
- **POCO Models** – Simple data models inheriting from a base class for consistency.
- **EF Core Repositories** – For production use, providing robust data access.
- **ViewModels** – Implementing INotifyPropertyChanged for UI updates.
- **Commands** – Implementing ICommand for user interactions.
- **Status Bar Feedback** – Success messages, such as after deletions, appear in the status bar instead of modal dialogs.

## Documentation
Detailed navigation information can be found in
[Docs/AppStateNavigation.md](Docs/AppStateNavigation.md) and
[Docs/UIKeyboardNavigation.md](Docs/UIKeyboardNavigation.md).
Core business rules are described in
[Docs/BusinessRules.md](Docs/BusinessRules.md).
[Docs/LazyLoading.md](Docs/LazyLoading.md) explains the `LazyLoadOnDisposedContextWarning`
observed when navigation properties are accessed after the context is disposed.

## Getting Started
1. Clone the repository.
1. Open the solution in Visual Studio.
1. Restore NuGet packages.
1. Build the solution.
1. Run the application.
1. On first launch, choose to populate the database with sample data or start with an empty database.
1. Explore the features and functionality of the application.

## Technologies Used
- **C#** – Primary programming language.
- **WPF** – For building the user interface.
- **MVVM** – Architectural pattern for UI development.
- **MRS** – Architectural pattern for organizing code.
- **Entity Framework Core** – For data access.
- **SQLite** – Lightweight database for local storage.
- **Serilog** – For logging.
- **Dependency Injection** – For managing dependencies and services.

## Contributing
Contributions are welcome! Please follow these steps:
1. Fork the repository.
1. Create a new branch for your feature or bug fix.
1. Make your changes and commit them with clear messages.
1. Push your changes to your forked repository.
1. Create a pull request describing your changes and why they should be merged.
1. Wait for review and feedback from the maintainers.
1. Make any necessary changes based on feedback.
1. Once approved, your changes will be merged into the main branch.
1. Celebrate your contribution to the project!
1. Thank you for your contributions!

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
## Contact
For any questions or feedback, please open an issue in the repository or contact the maintainers via email.
## Acknowledgments
Thanks to all contributors and the open-source community for their support and inspiration. Special thanks to the creators of the technologies used in this project, including WPF, Entity Framework Core, and Serilog.
## Issues
If you encounter any issues or bugs, please open an issue in the repository. Provide as much detail as possible, including steps to reproduce the issue, expected behavior, and actual behavior. This will help us address the problem more effectively.
## Future Enhancements
We plan to enhance the application with the following features:
- **Data Export** – Allow users to export invoices and data in various formats (e.g., CSV, PDF).
- **Reporting** – Add reporting features to analyze invoice data.
- **UI Improvements** – Enhance the user interface for better usability and aesthetics.
- **Unit Tests** – Expand unit tests for better coverage and reliability.
- **Performance Optimization** – Improve performance for larger datasets.
- **Localization** – Support multiple languages for international users.
- **Dark Mode** – Add a dark mode option for better user experience.
- **Documentation** – Improve documentation for better understanding and usage of the application.
- **Code Refactoring** – Regularly refactor code for maintainability and readability.
- **Feature Requests** – Open to feature requests from users to enhance functionality based on real-world needs.
- **Bug Fixes** – Continuously address bugs and issues reported by users to improve application stability.
- **User Feedback** – Gather user feedback to prioritize enhancements and new features.

## Known Issues	
- **Database Initialization** – Occasionally, the database may not initialize correctly on first launch. If this happens, try deleting the database file and restarting the application.
- **Sample Data Population** – The sample data population feature may not work as expected in some environments. If you encounter issues, please report them in the issues section.
- **UI Responsiveness** – The UI may become unresponsive during heavy data operations. Consider implementing background tasks for better performance.
- **Logging Configuration** – Ensure that the Serilog configuration is set up correctly in `StartupOrchestrator`. If logging fails, check the configuration settings.
- **Dependency Injection Issues** – If you encounter issues with dependency injection, ensure that all services and repositories are registered correctly in the `StartupOrchestrator`.
- **Unit Tests** – Some unit tests may fail due to environment-specific configurations. Ensure that your test environment matches the expected setup.
- **EF Core Migrations** – If you make changes to the database schema, ensure that EF Core migrations are applied correctly. Use the Package Manager Console or CLI to update the database.
- **XAML Binding Issues** – If you encounter binding errors in XAML, ensure that the ViewModels implement `INotifyPropertyChanged` correctly and that the bindings are set up properly in the XAML files.

## Troubleshooting
If you encounter issues while using the application, here are some troubleshooting steps:
- **Check Logs** – Review the Serilog logs for any errors or warnings that may indicate the source of the problem.
- **Database Connection** – Ensure that the SQLite database file is accessible and not locked by another process.
- **Rebuild Solution** – Sometimes, rebuilding the solution can resolve issues related to outdated binaries or configurations.
- **Clear Cache** – If the application behaves unexpectedly, try clearing any cached data or settings.
- **Update Dependencies** – Ensure that all NuGet packages are up to date. Use the NuGet Package Manager to check for updates.
- **Review Code Changes** – If you recently made changes to the code, review them for any potential issues or errors.
