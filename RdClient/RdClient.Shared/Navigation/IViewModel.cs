using System;
namespace RdClient.Navigation
{
    public interface IViewModel
    {
        void Presenting(INavigationService navigationService, object activationParameter);

        void Dismissing();
    }
}
