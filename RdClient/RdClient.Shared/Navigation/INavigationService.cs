using RdClient.Shared.Helpers;
using System;
using System.Windows.Input;

namespace RdClient.Shared.Navigation
{
    /// <summary>
    /// Interface of a navigation service that navigates the UI between named views.
    /// The service supports two navigation modes - the main UI that is always shown, and a stack
    /// of modal views shown for a short time.
    /// </summary>
    public interface INavigationService
    {
        event EventHandler PushingFirstModalView;
        event EventHandler DismissingLastModalView;
        IViewPresenter Presenter { set; }
        IPresentableViewFactory ViewFactory { set; }
        NavigationExtensionList Extensions { get; }
        ICommand BackCommand { get; }        
            
        /// <summary>
        /// Navigate to a view. If the view is not currently shown on the UI, the presenter is asked to present it.
        /// Otherwise (for example when the view is already navigated to) the Presenting() callback is called with the new parameter
        /// but the view hierarchy remains unchanged.
        /// In any case, if there is already a view in the presenter, its Dismissing() callback is triggered.
        /// The method also triggers the new view's Presenting() callback.
        /// </summary>
        /// <param name="viewName">name of the view. has to be registered in the view factory</param>
        /// <param name="activationParameter">A user determined parameter passed to the view.</param>
        void NavigateToView(string viewName, object activationParameter);

        /// <summary>
        /// Displays a modal view ontop of the current view. The view is pushed on top of the stack of modal views currently presented.
        /// If the view is already on the stack or if the view is the currently navigated to view a NavigationServiceException is thrown.
        /// </summary>
        /// <param name="viewName">Name of the view as registered in the view factory.</param>
        /// <param name="activationParameter">A user determined parameter passed to the view.</param>
        /// <param name="presentationCompletion">Completion object called when the accessory view gets dismissed.</param>
        void PushModalView(string viewName, object activationParameter, IPresentationCompletion presentationCompletion = null);
        /// <summary>
        /// Dismisses a modal view. If the view is not the top view on the stack, the views on the stack which are above it are dismissed as well from top to bottom.
        /// </summary>
        /// <param name="modalView">view to dismiss</param>
        void DismissModalView(IPresentableView modalView);
        /// <summary>
        /// Push a modal view onto the accessory view presenter of the currently presented view.
        /// </summary>
        /// <remarks>If the currently presented view does not have an accessory presenter, method pushes the view
        /// on top of the modal stack.</remarks>
        /// <param name="viewName">Name of the view as registered in the view factory.</param>
        /// <param name="activationParameter">A user determined parameter passed to the view.</param>
        void PushAccessoryView(string viewName, object activationParameter, IPresentationCompletion presentationCompletion = null);

        SynchronousCompletion CreateAccessoryStack(string viewName, object activationParameter, IPresentationCompletion presentationCompletion = null);

        /// <summary>
        /// Dismiss an accessory view and any views on top of it on the accessory stack.
        /// If the view was presented modally, it is removed from the modal atack with all modal views
        /// on top of it.
        /// </summary>
        /// <param name="accessoryView"></param>
        void DismissAccessoryView(IPresentableView accessoryView);
    }
}
