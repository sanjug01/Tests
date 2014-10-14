﻿namespace FadeTest.Navigation
{
    /// <summary>
    /// Interface of a UI object that may be presented by the view presenter component (IViewPresenter interface)
    /// </summary>
    public interface IPresentableView
    {
        void Activating(INavigationService navigationService, object activationParameter);
        void Presenting(IModalViewController modalViewController);
        void Dismissing();
    }
}
