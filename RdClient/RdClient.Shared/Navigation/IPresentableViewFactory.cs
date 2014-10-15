using System;
namespace RdClient.Navigation
{
    /// <summary>
    /// Interface of a factory of presentable views.
    /// </summary>
    public interface IPresentableViewFactory
    {
        IPresentableView CreateView(string name, object activationParameter);
    }
}
