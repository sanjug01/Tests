namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Models;
    using System.ComponentModel;
    using System.Windows.Input;

    public interface IRemoteConnectionViewModel : INotifyPropertyChanged
    {

        bool IsSelected { get; set; }

        bool SelectionEnabled { get; set; }

        ICommand EditCommand { get; }

        ICommand ConnectCommand { get; }

        ICommand DeleteCommand { get; }

        void Presenting(ISessionFactory sessionFactory);

        void Dismissed();
    }
}
