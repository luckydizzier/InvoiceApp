using System;
using System.Collections.Generic;
using InvoiceApp.Models;

namespace InvoiceApp.Services
{
    /// <summary>
    /// Default implementation of <see cref="INavigationService"/>.
    /// Tracks navigation history via a stack.
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly Stack<AppState> _history = new();

        public event EventHandler<AppState>? StateChanged;

        public AppState CurrentState => _history.Count > 0 ? _history.Peek() : AppState.MainWindow;

        public NavigationService()
        {
            _history.Push(AppState.MainWindow);
        }

        public void Push(AppState state)
        {
            _history.Push(state);
            StateChanged?.Invoke(this, state);
        }

        public void Pop()
        {
            if (_history.Count > 1)
            {
                _history.Pop();
            }
            StateChanged?.Invoke(this, CurrentState);
        }

        public void SwitchRoot(AppState state)
        {
            _history.Clear();
            _history.Push(state);
            StateChanged?.Invoke(this, state);
        }
    }
}
