using System;
namespace RdClient.Navigation
{
    /// <summary>
    /// Interface of a factory of presentable views.
    /// </summary>
    public interface IPresentableViewFactory
    {
        /// <summary>
        /// Create a view registered under the string. Pass the activation parameter to the view so it can initialize itself.
        /// </summary>
        /// <param name="name">name of the view which we want to create</param>
        /// <param name="activationParameter">A user defined parameter passed to the Activating() callback method of the IPresentableView</param>
        /// <returns>A View object implementing the IPresentableView protocol - either newly created or, if marked as singleton at registration, an already existing instance</returns>
        IPresentableView CreateView(string name, object activationParameter);
    }
}
