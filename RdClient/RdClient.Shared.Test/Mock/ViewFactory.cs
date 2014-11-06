using RdClient.Navigation;
using RdMock;
using System.Collections.Generic;

namespace RdClient.Shared.Test.Mock
{
    class ViewFactory : MockBase, IPresentableViewFactory
    {
        public IPresentableView CreateView(string name, object activationParameter)
        {
            return (IPresentableView)Invoke(new object[] { name, activationParameter });
        }

        public void AddViewClass(string name, System.Type viewClass, bool isSingleton = false)
        {
            Invoke(new object[] { name, viewClass, isSingleton });
        }
    }
}
