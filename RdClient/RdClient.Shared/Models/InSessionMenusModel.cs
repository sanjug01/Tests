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
        private readonly IRemoteSession _session;
        private readonly IFullScreenModel _fullScreenModel;
        private readonly RelayCommand _enterFullScreen;
        private readonly RelayCommand _exitFullScreen;

        public InSessionMenusModel(IRemoteSession session, IFullScreenModel fullScreenModel)
        {
            Contract.Assert(null != session);
            Contract.Assert(null != fullScreenModel);

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
        }

        void IInSessionMenus.Disconnect()
        {
            _session.Disconnect();
        }

        ICommand IInSessionMenus.EnterFullScreen { get { return _enterFullScreen; } }

        ICommand IInSessionMenus.ExitFullScreen { get { return _exitFullScreen; } }

        protected override void DisposeManagedState()
        {
            _fullScreenModel.EnteredFullScreen -= this.OnFullScreenChanged;
            _fullScreenModel.ExitedFullScreen -= this.OnFullScreenChanged;
            base.DisposeManagedState();
        }

        private void OnFullScreenChanged(object sender, EventArgs e)
        {
            _enterFullScreen.EmitCanExecuteChanged();
            _exitFullScreen.EmitCanExecuteChanged();
        }
    }
}
