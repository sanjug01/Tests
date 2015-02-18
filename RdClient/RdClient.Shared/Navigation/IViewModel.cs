namespace RdClient.Shared.Navigation
{
    using System;

    public interface IViewModel
    {
        /// <summary>
        /// Called by the navigation service before presenting a view either in the view pewsenter,
        /// or on the modal stack.
        /// </summary>
        /// <param name="navigationService">Navigation service that is presenting the view model.</param>
        /// <param name="activationParameter">Arbitrary parameter passed to the navigation service.</param>
        /// <param name="presentationContext">Context of presenting a view/view model on the model stack.
        /// The non-null parameter is passed only to view models pushed onto the model stack.</param>
        void Presenting(INavigationService navigationService, object activationParameter, IModalPresentationContext presentationContext);

        void Dismissing();

        void NavigatingBack(IBackCommandArgs backArgs);
    }
}
