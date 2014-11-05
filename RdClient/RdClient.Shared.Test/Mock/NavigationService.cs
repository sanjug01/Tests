using RdClient.Navigation;
using RdMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.Mock
{
    public class NavigationService : MockBase, INavigationService
    {
        public void NavigateToView(string viewName, object activationParameter)
        {
            Invoke(new object[] { viewName, activationParameter });
        }

        public void PushModalView(string viewName, object activationParameter)
        {
            Invoke(new object[] { viewName, activationParameter });                
        }

        public void DismissModalView(IPresentableView modalView)
        {
            Invoke(new object[] { modalView });
        }
    }
}
