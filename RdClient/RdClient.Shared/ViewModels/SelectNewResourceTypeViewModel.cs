namespace RdClient.Shared.ViewModels
{
    using System;
    using System.Windows.Input;

    public sealed class SelectNewResourceTypeViewModel : AccessoryViewModelBase
    {
        private readonly RelayCommand _addDesktop;
        private readonly RelayCommand _addOnPremiseWorkspace;
        private readonly RelayCommand _addCloudWorkspace;

        public enum InternalResult
        {
            AddDesktop,
            AddOnPremiseWorkspace,
            AddCloudWorkspace
        }

        public sealed class Completion : CompletionBase
        {
            public event EventHandler AddDesktop;
            public event EventHandler AddOnPremiseWorkspace;
            public event EventHandler AddCloudWorkspace;

            protected override void OnCompleted(object result)
            {
                switch ((InternalResult)result)
                {
                    case InternalResult.AddDesktop:
                        EmitAddDesktop();
                        break;
                    case InternalResult.AddOnPremiseWorkspace:
                        EmitAddOnPremiseWorkspace();
                        break;

                    case InternalResult.AddCloudWorkspace:
                        EmitAddCloudWorkspace();
                        break;
                }
            }

            private void EmitAddDesktop()
            {
                if (null != this.AddDesktop)
                    this.AddDesktop(this, EventArgs.Empty);
            }

            private void EmitAddOnPremiseWorkspace()
            {
                if (null != this.AddOnPremiseWorkspace)
                    this.AddOnPremiseWorkspace(this, EventArgs.Empty);
            }

            private void EmitAddCloudWorkspace()
            {
                if (null != this.AddCloudWorkspace)
                    this.AddCloudWorkspace(this, EventArgs.Empty);
            }
        }

        public ICommand AddDesktop
        {
            get { return _addDesktop; }
        }

        public ICommand AddOnPremiseWorkspace
        {
            get { return _addOnPremiseWorkspace; }
        }

        public ICommand AddCloudWorkspace
        {
            get { return _addCloudWorkspace; }
        }

        public SelectNewResourceTypeViewModel()
        {
            _addDesktop = new RelayCommand(param => this.DismissModal(InternalResult.AddDesktop));
            _addOnPremiseWorkspace = new RelayCommand(param => this.DismissModal(InternalResult.AddOnPremiseWorkspace));
            _addCloudWorkspace = new RelayCommand(param => this.DismissModal(InternalResult.AddCloudWorkspace));
        }
    }
}
