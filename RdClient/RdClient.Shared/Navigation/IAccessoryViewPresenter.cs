﻿namespace RdClient.Shared.Navigation
{
    /// <summary>
    /// Presenter of accessory views - a simplified version of IViewPresenter that always presents
    /// views on a modal stack.
    /// </summary>
    public interface IAccessoryViewPresenter
    {
        void PushAccessoryView(IPresentableView view, object activationParameter, IPresentationCompletion presentationCompletion = null);
        void DismissAccessoryView(IPresentableView view);
    }
}
