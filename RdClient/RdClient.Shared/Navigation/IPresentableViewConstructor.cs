using RdClient.Navigation;
using System;
namespace RdClient.Shared.Navigation
{
    public interface IPresentableViewConstructor
    {
        void Initialize(Type viewClass, bool isSingleton);
        IPresentableView CreateView();
    }
}
