namespace RdClient.Shared.ViewModels
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
        private SynchronousCompletion _completion;

        /// <summary>
        /// Base class for presentation completion objects passed to the navigation service to report completion
        /// of accessory views shown in the connection center.
        /// </summary>
        public abstract class CompletionBase : IPresentationCompletion
        {
            public event EventHandler Completed;
            public event EventHandler Cancelled;

            /// <summary>
            /// Overridable called when IPresentationCompletion.Completed is called with non-null result.
            /// </summary>
            /// <param name="result">Non-null value passed to IPresentationCompletion.Completed</param>
            /// <remarks>Default implementation does nothing so there is no need to call it.</remarks>
            protected virtual void OnCompleted(object result)
            {
            }

            void IPresentationCompletion.Completed(IPresentableView view, object result)
            {
                EmitCompleted();

                if(null == result)
                {
                    EmitCancelled();
                }
                else
                {
                    OnCompleted(result);
                }
            }

            private void EmitCompleted()
            {
                if (null != this.Completed)
                    this.Completed(this, EventArgs.Empty);
            }

            private void EmitCancelled()
            {
                if (null != this.Cancelled)
                    this.Cancelled(this, EventArgs.Empty);
            }
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(activationParameter is SynchronousCompletion);
            base.OnPresenting(activationParameter);

            _completion = (SynchronousCompletion)activationParameter;
            _completion.Completed += this.OnCancellationRequested;
        }

        protected override void OnDismissed()
        {
            Contract.Assert(null != _completion);

            base.OnDismissed();
            _completion.Completed -= this.OnCancellationRequested;
            _completion = null;
        }

        private void OnCancellationRequested(object sender, EventArgs e)
        {
            Contract.Assert(object.ReferenceEquals(_completion, sender));
            this.DismissModal(null);
        }
    }
}
