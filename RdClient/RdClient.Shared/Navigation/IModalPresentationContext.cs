namespace RdClient.Shared.Navigation
{
    /// <summary>
    /// Interface of a context of a modal presentation of a view passed to the view's view model.
    /// The view model may use the context to dismiss itself from the modal stack.
    /// </summary>
    public interface IModalPresentationContext
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
