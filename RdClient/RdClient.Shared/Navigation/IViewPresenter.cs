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
    }
}
