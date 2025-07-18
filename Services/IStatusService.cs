namespace InvoiceApp.Services
{
    /// <summary>
    /// Provides application-wide status messages for the UI.
    /// </summary>
    public interface IStatusService
    {
        /// <summary>
        /// Gets the currently displayed status message.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Displays a new status message.
        /// </summary>
        void Show(string message);
    }
}
