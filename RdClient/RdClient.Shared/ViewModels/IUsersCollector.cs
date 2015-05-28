namespace RdClient.Shared.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    // generic interface for mantaining a ReadOnlyObservableCollection of UserComboBoxElement instaces
    public interface IUsersCollector
    {
        ReadOnlyObservableCollection<UserComboBoxElement> Users { get; }
        UserComboBoxElement SelectedUser { get; set; }
        ICommand EditUser { get; }
        ICommand AddUser { get; }
    }
}
