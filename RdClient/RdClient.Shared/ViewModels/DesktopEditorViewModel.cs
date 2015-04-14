namespace RdClient.Shared.ViewModels
{
    using System.Diagnostics.Contracts;
    using System.Threading;

    public sealed class DesktopEditorViewModel : ViewModelBase
    {
        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(activationParameter is CancellationToken);
            base.OnPresenting(activationParameter);

            CancellationToken token = (CancellationToken)activationParameter;
            token.Register(this.OnCancel);
        }

        private void OnCancel()
        {
            this.DismissModal(null);
        }
    }
}
