using RdClient.Shared.Navigation;
using RdMock;

namespace RdClient.Shared.Test.Mock
{
    public class ViewModel : MockBase, IViewModel
    {
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
