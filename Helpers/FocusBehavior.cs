using System.Windows;
using System.Windows.Input;

namespace InvoiceApp.Helpers
{
    public static class FocusBehavior
    {
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached(
                "IsFocused",
                typeof(bool),
                typeof(FocusBehavior),
                new PropertyMetadata(false, OnIsFocusedChanged));

        public static bool GetIsFocused(DependencyObject obj) =>
            (bool)obj.GetValue(IsFocusedProperty);

        public static void SetIsFocused(DependencyObject obj, bool value) =>
            obj.SetValue(IsFocusedProperty, value);

        private static void OnIsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IInputElement element && e.NewValue is bool shouldFocus && shouldFocus)
            {
                element.Focus();
            }
        }

        public static readonly DependencyProperty AdvanceOnEnterProperty =
            DependencyProperty.RegisterAttached(
                "AdvanceOnEnter",
                typeof(bool),
                typeof(FocusBehavior),
                new PropertyMetadata(false, OnAdvanceOnEnterChanged));

        public static bool GetAdvanceOnEnter(DependencyObject obj) =>
            (bool)obj.GetValue(AdvanceOnEnterProperty);

        public static void SetAdvanceOnEnter(DependencyObject obj, bool value) =>
            obj.SetValue(AdvanceOnEnterProperty, value);

        public static readonly DependencyProperty EnterCommandOnLastProperty =
            DependencyProperty.RegisterAttached(
                "EnterCommandOnLast",
                typeof(ICommand),
                typeof(FocusBehavior),
                new PropertyMetadata(null));

        public static ICommand GetEnterCommandOnLast(DependencyObject obj) =>
            (ICommand)obj.GetValue(EnterCommandOnLastProperty);

        public static void SetEnterCommandOnLast(DependencyObject obj, ICommand value) =>
            obj.SetValue(EnterCommandOnLastProperty, value);

        private static void OnAdvanceOnEnterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                if ((bool)e.NewValue)
                {
                    element.PreviewKeyDown += ElementOnPreviewKeyDown;
                }
                else
                {
                    element.PreviewKeyDown -= ElementOnPreviewKeyDown;
                }
            }
        }

        private static void ElementOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || sender is not UIElement element)
            {
                return;
            }

            bool moved = element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            if (!moved)
            {
                var command = GetEnterCommandOnLast(element);
                if (command?.CanExecute(null) == true)
                {
                    command.Execute(null);
                }
            }

            e.Handled = true;
        }
    }
}
