namespace RdClient.Shared.Navigation
{
    /// <summary>
    /// Interface of a context of a view presented on a stack, passed to the view's view model.
    /// The view model may use the context to dismiss itself from the stack.
    /// </summary>
    /// <remarks>The 2 types of stacks are modal stack and accesory stack.</remarks>
    public interface IStackedPresentationContext
    {
        /// <summary>
        /// Dismiss the view for which the object was created from the modal stack and pass a parameter
        /// to the completion handler that may have been passed to the navigation service to report
        /// completion of modal presentation.
        /// </summary>
        /// <param name="result">Arbitrary object passed to the completion handler passed to the navigation service.</param>
        void Dismiss(object result);
    }
}
