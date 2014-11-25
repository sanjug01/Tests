namespace RdClient.Shared.Navigation
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Interface of a completion handler passed to the navigation service for modal presentation of a view.
    /// The navigation service calls IPresentationCompletion.Completed when it dismisses the modal view.
    /// The completion handler may examine the view and its view model in the handler.
    /// </summary>
    [ContractClass(typeof(Contracts.IPresentationCompletionContract))]
    public interface IPresentationCompletion
    {
        /// <summary>
        /// Called by the navigation service after it has dismissed a modal view.
        /// </summary>
        /// <param name="view">The view that has been dismissed.</param>
        /// <param name="result">Arbitrary result object that may have been set by the view model of the view.</param>
        /// <remarks>The result object is the one that the view model has passed to IModalPresentationContext.Dismiss.</remarks>
        /// <remarks>If the view does not have a view model, or the view model has been dismissed forcefully,
        /// the result parameter is null.</remarks>
        void Completed(IPresentableView view, object result);
    }
}
