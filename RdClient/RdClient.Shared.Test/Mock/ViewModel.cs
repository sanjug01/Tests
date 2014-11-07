using RdClient.Navigation;
using RdMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
