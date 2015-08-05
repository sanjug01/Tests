namespace RdClient.Shared.Models
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// Model injected in the view model of the in-session menus view.
    /// </summary>
    public sealed class InSessionMenusModel : DisposableObject, IInSessionMenus
    {
        private readonly IDeferredExecution _dispatcher;
        private readonly IRemoteSession _session;
        private readonly IFullScreenModel _fullScreenModel;
        private readonly RelayCommand _enterFullScreen;
        private readonly RelayCommand _exitFullScreen;
        private bool _disposed;

        /// <summary>
        /// Create a new InSessionMenusModel object.
        /// </summary>
        /// <param name="dispatcher">Deferred execution dispatcher dispatching events emitted by the full screen model
        /// to the UI thread.</param>
        /// <param name="session">Remote session object.</param>
        /// <param name="fullScreenModel">Full screen model object.</param>
        public InSessionMenusModel(IDeferredExecution dispatcher, IRemoteSession session, IFullScreenModel fullScreenModel)
        {
            Contract.Assert(null != dispatcher);
            Contract.Assert(null != session);
            Contract.Assert(null != fullScreenModel);
            Contract.Ensures(null != _dispatcher);
            Contract.Ensures(null != _session);
            Contract.Ensures(null != _fullScreenModel);

            _dispatcher = dispatcher;
            _session = session;
            _fullScreenModel = fullScreenModel;
            _enterFullScreen = new RelayCommand(
                parameter => _fullScreenModel.EnterFullScreen(),
                parameter => !_fullScreenModel.IsFullScreenMode);
            _exitFullScreen = new RelayCommand(
                parameter => _fullScreenModel.ExitFullScreen(),
                parameter => _fullScreenModel.IsFullScreenMode);
            _fullScreenModel.EnteredFullScreen += this.OnFullScreenChanged;
            _fullScreenModel.ExitedFullScreen += this.OnFullScreenChanged;
            _disposed = false;
        }

        void IInSessionMenus.Disconnect()
        {
            _session.Disconnect();
        }

        ICommand IInSessionMenus.EnterFullScreen { get { return _enterFullScreen; } }

        ICommand IInSessionMenus.ExitFullScreen { get { return _exitFullScreen; } }

        protected override void DisposeManagedState()
        {
            _disposed = true;
            _fullScreenModel.EnteredFullScreen -= this.OnFullScreenChanged;
            _fullScreenModel.ExitedFullScreen -= this.OnFullScreenChanged;
            base.DisposeManagedState();
        }

        private void OnFullScreenChanged(object sender, EventArgs e)
        {
            _dispatcher.Defer(() =>
            {
                if (!_disposed)
                {
                    _enterFullScreen.EmitCanExecuteChanged();
                    _exitFullScreen.EmitCanExecuteChanged();
                }
            });
        }
    }
}
