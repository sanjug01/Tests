using RdClient.Navigation;
using System;
using System.ComponentModel;
namespace RdClient.Shared.ViewModels
{
    interface IViewModel
    {
        event PropertyChangedEventHandler PropertyChanged;
        void Presenting(INavigationService navigationService, object activationParameter);

        void Dismissing();
    }
}
