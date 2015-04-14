namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Navigation;
    using System;
    using System.Diagnostics.Contracts;

    public abstract class AccessoryViewModelBase : ViewModelBase
    {
        private SynchronousCompletion _completion;

        public event EventHandler Completed;

        public abstract class CompletionBase : IPresentationCompletion
        {
            public event EventHandler Completed;
            public event EventHandler Cancelled;

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
