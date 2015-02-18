using System.Windows.Input;

namespace RdClient.Shared.Navigation
{
    public interface INavigationCommands
    {
        ICommand BackCommand { get; }
    }
}
