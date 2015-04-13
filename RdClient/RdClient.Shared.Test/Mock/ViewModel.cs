using RdClient.Shared.Navigation;
using RdMock;

namespace RdClient.Shared.Test.Mock
{
    public class ViewModel : MockBase, IViewModel
    {
        public void Presenting(INavigationService navigationService, object activationParameter, IStackedPresentationContext presentationResult)
        {
            Invoke(new object[] { navigationService, activationParameter, presentationResult });
        }

        public void Dismissing()
        {
            Invoke(new object[] { });
        }    

        public void NavigatingBack(IBackCommandArgs backArgs)
        {
            Invoke(new object[] { backArgs });
        }
    }
}
