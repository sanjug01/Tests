namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// View model of the modal dialog shown over the remote session view when the "ellipsis"
    /// button's been clicked in the connection bar.
    /// </summary>
    public sealed class InSessionMenusViewModel : ViewModelBase
    {
        private readonly RelayCommand _cancel;
        private readonly RelayCommand _disconnect;
        private readonly CommandBinding _enterFullScreen;
        private readonly CommandBinding _exitFullScreen;
        private readonly CommandBinding _setTouchMode;
        private readonly CommandBinding _setPointerMode;
        private bool _canDisconnect;
        private IInSessionMenus _model;

        public InSessionMenusViewModel()
        {
            _cancel = new RelayCommand(this.OnCancel);
            _disconnect = new RelayCommand(this.OnDisconnect, o => this.CanDisconnect);
            _enterFullScreen = new CommandBinding();
            _exitFullScreen = new CommandBinding();
            _setTouchMode = new CommandBinding();
            _setPointerMode = new CommandBinding();
        }

        public ICommand Cancel
        {
            get { return _cancel; }
        }

        public ICommand Disconnect
        {
            get { return _disconnect; }
        }

        public CommandBinding EnterFullScreen
        {
            get { return _enterFullScreen; }
        }

        public CommandBinding ExitFullScreen
        {
            get { return _exitFullScreen; }
        }

        public CommandBinding SetTouchMode
        {
            get { return _setTouchMode; }
        }

        public CommandBinding SetPointerMode
        {
            get { return _setPointerMode; }
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(null == _model);
            Contract.Assert(activationParameter is IInSessionMenus);

            _model = (IInSessionMenus)activationParameter;
            this.CanDisconnect = true;
            _enterFullScreen.Command = _model.EnterFullScreen;
            _exitFullScreen.Command = _model.ExitFullScreen;
            _setTouchMode.Command = _model.TouchMode;
            _setPointerMode.Command = _model.PointerMode;

            base.OnPresenting(activationParameter);
        }

        protected override void OnDismissed()
        {
            _enterFullScreen.Command = null;
            _exitFullScreen.Command = null;
            _setTouchMode.Command = null;
            _setPointerMode.Command = null;
            _model.Dispose();
            _model = null;
            base.OnDismissed();
        }

        private void OnCancel(object parameter)
        {
            this.DismissModal(null);
        }

        private void OnDisconnect(object parameter)
        {
            Contract.Assert(null != _model);
            _model.Disconnect();
            this.CanDisconnect = false;
        }

        private bool CanDisconnect
        {
            get { return _canDisconnect; }
            set
            {
                if(value != _canDisconnect)
                {
                    _canDisconnect = value;
                    _disconnect.EmitCanExecuteChanged();
                }
            }
        }
    }
}
