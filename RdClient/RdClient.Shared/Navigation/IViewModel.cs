using System;
namespace RdClient.Shared.Navigation
{
    public interface IViewModel
    {
        void Presenting(INavigationService navigationService, object activationParameter);

        void Dismissing();
    }
}
