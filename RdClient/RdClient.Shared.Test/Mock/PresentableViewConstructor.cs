using RdClient.Navigation;
using RdClient.Shared.Navigation;
using RdMock;
using System;

namespace RdClient.Shared.Test.Mock
{
    public class PresentableViewConstructor : MockBase, IPresentableViewConstructor
    {
        public void Initialize(Type viewClass, bool isSingleton)
        {
            Invoke(new object[] { viewClass, isSingleton });
        }

        public IPresentableView CreateView()
        {
            return (IPresentableView)Invoke(new object[] { });
        }
    }
}
