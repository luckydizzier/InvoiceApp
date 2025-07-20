using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Stack<AppState> _substates = new();

        public event EventHandler<AppState>? StateChanged;

        public AppState CurrentState =>
            _substates.Count > 0 ? _substates.Peek() :
            (_history.Count > 0 ? _history.Peek() : AppState.MainWindow);

        public NavigationService()
        {
            _history.Push(AppState.Invoices);
        }

        public void Push(AppState state)
        {
            _history.Push(state);
            _substates.Clear();
            StateChanged?.Invoke(this, state);
        }

        public void PushSubstate(AppState state)
        {
            _substates.Push(state);
            StateChanged?.Invoke(this, state);
        }

        public void Pop()
        {
            if (_substates.Count > 0)
            {
                _substates.Pop();
            }
            else if (_history.Count > 1)
            {
                _history.Pop();
            }
            StateChanged?.Invoke(this, CurrentState);
        }

        public void PopSubstate()
        {
            if (_substates.Count > 0)
            {
                _substates.Pop();
            }
            StateChanged?.Invoke(this, CurrentState);
        }

        public void ClearSubstates()
        {
            if (_substates.Count > 0)
            {
                _substates.Clear();
                StateChanged?.Invoke(this, CurrentState);
            }
        }

        public void SwitchRoot(AppState state)
        {
            _history.Clear();
            _substates.Clear();
            _history.Push(state);
            StateChanged?.Invoke(this, state);
        }

        public IEnumerable<AppState> GetStatePath()
        {
            var rootPath = _history.Reverse().Skip(1); // skip MainWindow
            var subPath = _substates.Reverse();
            return rootPath.Concat(subPath);
        }
    }
}
