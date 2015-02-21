using System;
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
        /// Perform validation of a change of a single property.
        /// </summary>
        /// <param name="viewModel">View model representing the credentials editor view.</param>
        /// <param name="propertyName">Name of the changed property of the IEditCredentialsViewModel object.</param>
        /// <returns>True if the new state of the credentials editor is valid.</returns>
        /// <remarks>If method returns true, Validate is also called to perform possibly redundant
        /// but more holistic validation.</remarks>
        bool ValidateChangedProperty(IEditCredentialsViewModel viewModel, string propertyName);
        /// <summary>
        /// Perform validation of data entered in the view.
        /// </summary>
        /// <param name="viewModel">View model representing the credentials editor view.</param>
        /// <returns>True if the modal credentials editor may be dismissed; otherwise - false, in which case
        /// the view model will disable the "dismiss" button.</returns>
        bool Validate(IEditCredentialsViewModel viewModel);
        /// <summary>
        /// Called when user has activated the dismiss UI (clicked the button) to perform any additional checks
        /// or present any additional UI that asks user additional questions.
        /// </summary>
        /// <param name="viewModel">View model representing the credentials editor view.</param>
        /// <param name="dismiss">Action delegate that unconditionally dismisses the credentials editor</param>
        /// <returns>True if the credentials editor may be dismissed. If method returned true, Dismissed is called immediately and the dismiss
        /// delegate is invalidated.</returns>
        bool Dismissing(IEditCredentialsViewModel viewModel, Action dismiss);
        void Dismissed(IEditCredentialsViewModel viewModel);
        void Cancelled(IEditCredentialsViewModel viewModel);
    }
}
