using System.Windows.Input;

namespace InvoiceApp.Presentation.ViewModels
{
    public class DashboardMenuItem
    {
        public string Key { get; }
        public string Description { get; }
        public ICommand Command { get; }

        public DashboardMenuItem(string key, string description, ICommand command)
        {
            Key = key;
            Description = description;
            Command = command;
        }
    }
}
