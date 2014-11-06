using System;
using RdClient.Navigation;
using RdMock;

namespace RdClient.Shared.Test.Mock
{
    public class PresentableView : MockBase, IPresentableView
    {
        public void Activating(object activationParameter)
        {
            Invoke(new object[] { activationParameter });
        }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            Invoke(new object[] { navigationService, activationParameter });
        }

        public void Dismissing()
        {
            Invoke(new object[] { });
        }
    }
}
