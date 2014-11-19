using System;
namespace RdClient.Shared.Navigation
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

        /// <summary>
        /// Registers a view type with the factory
        /// </summary>
        /// <param name="name">name by which the view is referred to when creating it</param>
        /// <param name="viewClass">subclass of a Page</param>
        /// <param name="isSingleton">if there can be only one instance of this view in the app, set this to true</param>
        void AddViewClass(string name, Type viewClass, bool isSingleton = false);
    }
}
