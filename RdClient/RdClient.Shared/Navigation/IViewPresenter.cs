namespace RdClient.Shared.Navigation
{
    /// <summary>
    /// Responsible for inserting and removing views into the view hierarchy.
    /// </summary>
    public interface IViewPresenter
    {
        /// <summary>
        /// Adds a view to the view hierarchy. If a view is already being presented, the old one is removed.
        /// </summary>
        /// <param name="view">the view to be added</param>
        void PresentView(IPresentableView view);
        /// <summary>
        /// addes a view onto the modal view stack
        /// </summary>
        /// <param name="view">the view to be added</param>
        void PushModalView(IPresentableView view);
        /// <summary>
        /// removes a view from the modal view stack
        /// </summary>
        /// <param name="view">the view to be removed</param>
        void DismissModalView(IPresentableView view);

        void PresentingFirstModalView();
        void DismissedLastModalView();

    }
}
