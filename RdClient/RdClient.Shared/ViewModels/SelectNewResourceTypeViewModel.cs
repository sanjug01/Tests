namespace RdClient.Shared.ViewModels
{
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Windows.Input;

    public sealed class SelectNewResourceTypeViewModel : ViewModelBase
    {
        private readonly RelayCommand _addDesktop;

        public enum Result
        {
            AddDesktop,
        }

        public ICommand AddDesktop
        {
            get { return _addDesktop; }
        }

        public SelectNewResourceTypeViewModel()
        {
            _addDesktop = new RelayCommand(this.ExecuteAddDesktop);
        }

        private void ExecuteAddDesktop(object parameter)
        {
            //
            // Dismiss self as a modal view; this will also dismisses accessory views.
            //
            this.DismissModal(Result.AddDesktop);
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(activationParameter is CancellationToken);
            base.OnPresenting(activationParameter);

            CancellationToken token = (CancellationToken)activationParameter;
            token.Register(this.OnCancel);
        }

        protected override void OnDismissed()
        {
            base.OnDismissed();
        }

        private void OnCancel()
        {
            this.DismissModal(null);
        }
    }
}
