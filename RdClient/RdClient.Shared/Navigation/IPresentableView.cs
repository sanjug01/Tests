namespace RdClient.Navigation
{
    /// <summary>
    /// Interface of a UI object that may be presented by the view presenter component (IViewPresenter interface)
    /// </summary>
    public interface IPresentableView
    {
        IViewModel ViewModel { get; }

        /// <summary>
        /// Invoked when the view is constructed.
        /// </summary>
        /// <param name="activationParameter">A user determined parameter passed to the view.</param>
        void Activating(object activationParameter);
        /// <summary>
        /// Invoked when the view is about to be displayed. It can also be called when the view is already displayed in which case the view should use
        /// the activation parameter to update its contents.
        /// </summary>
        /// <param name="navigationService">the navigation service used to navigate away from the view or display modal views</param>
        /// <param name="activationParameter">a user determined parameter used to update the contents of the ivew</param>
        void Presenting(INavigationService navigationService, object activationParameter);
        /// <summary>
        /// Invoked when the view is just about to be removed from the view hierarchy.
        /// </summary>
        void Dismissing();
    }
}
