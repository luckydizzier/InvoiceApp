namespace InvoiceApp.ViewModels
{
    /// <summary>
    /// Interface for view models that track unsaved modifications.
    /// </summary>
    public interface IHasChanges
    {
        /// <summary>
        /// Indicates whether there are pending changes.
        /// </summary>
        bool HasChanges { get; }

        /// <summary>
        /// Clears the change tracking state after saving.
        /// </summary>
        void ClearChanges();
    }
}
