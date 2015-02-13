namespace RdClient.Shared.ViewModels
{
    /// <summary>
    /// Interface of a task that the EditCredentialsView must perform.
    /// An object implementing the interface is passed to EditCredentialsViewModel by the navigation service
    /// as the presentation parameter.
    /// </summary>
    public interface IEditCredentialsTask
    {
        /// <summary>
        /// Populate the view model once when the view is presented.
        /// </summary>
        /// <param name="viewModel">View model that backs the view that is being presented.</param>
        void Populate(IEditCredentialsViewModel viewModel);
        /// <summary>
        /// Perform validation of data entered in the view.
        /// </summary>
        /// <param name="viewModel">View model representing the credentials editor view.</param>
        /// <returns>True if the modal credentials editor may be dismissed; otherwise - false, in which case
        /// the view model will disable the "dismiss" button.</returns>
        bool Validate(IEditCredentialsViewModel viewModel);
        void Dismissed(IEditCredentialsViewModel viewModel);
        void Cancelled(IEditCredentialsViewModel viewModel);
    }
}
