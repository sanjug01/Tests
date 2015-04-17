﻿namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Navigation;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Base class for view view models of accessory views shown in the connection center.
    /// </summary>
    public abstract class AccessoryViewModelBase : ViewModelBase
    {
        private SynchronousCompletion _cancellation;

        protected SynchronousCompletion Cancellation { get { return _cancellation; } }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(activationParameter is SynchronousCompletion);
            base.OnPresenting(activationParameter);

            _cancellation = (SynchronousCompletion)activationParameter;
            _cancellation.Completed += this.OnCancellationRequested;
        }

        protected override void OnDismissed()
        {
            Contract.Assert(null != _cancellation);

            base.OnDismissed();
            _cancellation.Completed -= this.OnCancellationRequested;
            _cancellation = null;
        }

        protected override void OnNavigatingBack(IBackCommandArgs backArgs)
        {
            base.OnNavigatingBack(backArgs);
            //
            // Just dismiss self. This will pull the top-most accessory view from the stack.
            //
            DismissModal(null);
        }

        private void OnCancellationRequested(object sender, EventArgs e)
        {
            Contract.Assert(object.ReferenceEquals(_cancellation, sender));
            this.DismissModal(null);
        }
    }
}