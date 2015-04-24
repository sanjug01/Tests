namespace RdClient.Shared.Navigation
{
    /// <summary>
    /// Optional interface of views that expect to be presented on a stack.
    /// </summary>
    public interface IStackedView
    {
        /// <summary>
        /// Called by the navigation service when the view becomes active because it is at the top of the stack.
        /// </summary>
        void Activate();

        /// <summary>
        /// Called buy the navigation service when another view is pushed onto the stack on top of the view,
        ///  or the view is removed from the stack.
        /// </summary>
        void Deactivate();
    }
}
