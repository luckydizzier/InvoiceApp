using System;
using InvoiceApp.Models;

namespace InvoiceApp.Services
{
    /// <summary>
    /// Provides navigation between logical screens of the application.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Gets the current application state.
        /// </summary>
        AppState CurrentState { get; }

        /// <summary>
        /// Navigate forward to the specified state.
        /// </summary>
        void Push(AppState state);

        /// <summary>
        /// Navigate forward to a sub-state within the current editor.
        /// </summary>
        void PushSubstate(AppState state);

        /// <summary>
        /// Navigate back to the previous state.
        /// </summary>
        void Pop();

        /// <summary>
        /// Navigate back within the editor sub-state stack.
        /// </summary>
        void PopSubstate();

        /// <summary>
        /// Clears the navigation stack and sets the new root state.
        /// </summary>
        void SwitchRoot(AppState state);

        /// <summary>
        /// Raised whenever the current state changes.
        /// </summary>
        event EventHandler<AppState> StateChanged;
    }
}
