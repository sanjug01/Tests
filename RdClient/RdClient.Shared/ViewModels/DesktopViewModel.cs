namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels.EditCredentialsTasks;
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using Windows.Foundation;

    public class DesktopViewModel : Helpers.MutableObject, IDesktopViewModel
    {
        private readonly Telemetry.ITelemetryClient _telemetryClient;
        private readonly RelayCommand _editCommand;
        private readonly RelayCommand _connectCommand;
        private readonly RelayCommand _deleteCommand;
        private readonly Guid _desktopId;
        private readonly DesktopModel _desktop;
        private readonly ApplicationDataModel _dataModel;
        private readonly INavigationService _navigationService;
        private ISessionFactory _sessionFactory;
        private bool _isSelected;
        private bool _selectionEnabled;

        public static IDesktopViewModel Create(IModelContainer<RemoteConnectionModel> desktopContainer,
            ApplicationDataModel dataModel,
            INavigationService navigationService,
            Telemetry.ITelemetryClient telemetryClient)
        {
            return new DesktopViewModel(desktopContainer, dataModel, navigationService, telemetryClient);
        }

        private DesktopViewModel(IModelContainer<RemoteConnectionModel> desktopContainer,
            ApplicationDataModel dataModel,
            INavigationService navigationService,
            Telemetry.ITelemetryClient telemetryClient)
        {
            Contract.Assert(null != desktopContainer);
            Contract.Assert(null != desktopContainer);
            Contract.Assert(null != navigationService);
            Contract.Assert(!Guid.Empty.Equals(desktopContainer.Id));

            _telemetryClient = telemetryClient;
            _editCommand = new RelayCommand(EditCommandExecute);
            _connectCommand = new RelayCommand(ConnectCommandExecute);
            _deleteCommand = new RelayCommand(DeleteCommandExecute);
            _navigationService = navigationService;

            _desktop = (DesktopModel)desktopContainer.Model;
            _desktopId = desktopContainer.Id;
            //
            // DesktopVieModel does not require Presenting/Dismissing, 
            //          but stil needs DataModel and NavigationService
            //          NavigationService may be initialized later while presenting the parent view
            //
            _dataModel = dataModel;
        }

        public Guid DesktopId
        {
            get { return _desktopId; }
        }

        public DesktopModel Desktop
        {
            get { return _desktop; }
        }

        public CredentialsModel Credentials
        {
            get 
            {
                CredentialsModel cred = null;

                if (_desktop.HasCredentials)
                {
                    try
                    {
                        cred = _dataModel.Credentials.GetModel(_desktop.CredentialsId);
                    }
                    catch(Exception ex)
                    {
                        //
                        // This may happen if the saved data was edited by hand; otherwise, the application data model
                        // clears references to removed objects.
                        //
                        _desktop.CredentialsId = Guid.Empty;
                        Debug.WriteLine("Exception {0} when looking up the credentials for desktop {1}", ex, _desktopId);
                    }
                }

                return cred;
            }
        }

        public bool SelectionEnabled
        {
            get { return _selectionEnabled; }
            set 
            {
                if (!value)
                {
                    this.IsSelected = false;
                }
                SetProperty(ref _selectionEnabled, value); 

            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (this.SelectionEnabled)
                {
                    SetProperty(ref _isSelected, value);
                }
            }
        }

        public ICommand EditCommand
        {
            get { return _editCommand; }
        }

        public ICommand ConnectCommand
        {
            get { return _connectCommand; }
        }

        public ICommand DeleteCommand
        {
            get { return _deleteCommand; }
        }


        void IRemoteConnectionViewModel.Presenting(ISessionFactory sessionFactory)
        {
            Contract.Assert(null != sessionFactory);
            Contract.Ensures(null != _sessionFactory);

            _sessionFactory = sessionFactory;
        }

        void IRemoteConnectionViewModel.Dismissed()
        {
            Contract.Assert(null != _sessionFactory);
            Contract.Ensures(null == _sessionFactory);

            _sessionFactory = null;
        }

        private void EditCommandExecute(object o)
        {
            _navigationService.PushAccessoryView("AddOrEditDesktopView", new EditDesktopViewModelArgs(this.Desktop));
        }

        private void ConnectCommandExecute(object o)
        {
            RemoteSessionSetup sessionSetup = new RemoteSessionSetup(_dataModel, this.Desktop);

            if (Guid.Empty.Equals(this.Desktop.CredentialsId))
            {
                InSessionCredentialsTask task = new InSessionCredentialsTask(sessionSetup.SessionCredentials,
                    _dataModel, CredentialPromptMode.EnterCredentials, sessionSetup);

                task.Submitted += (sender, e) =>
                {
                    RemoteSessionSetup setup = new RemoteSessionSetup((RemoteSessionSetup)e.State);
                    IRemoteSession session = _sessionFactory.CreateSession(setup);

                    if (e.SaveCredentials)
                        setup.SaveCredentials();

                    _navigationService.NavigateToView("RemoteSessionView", session);
                };

                _navigationService.PushModalView("InSessionEditCredentialsView", task);
            }
            else
            {
                _navigationService.NavigateToView("RemoteSessionView", _sessionFactory.CreateSession(sessionSetup));
            }
        }

        private void InternalConnect(CredentialsModel credentials, bool storeCredentials)
        {
            if(storeCredentials)
            {
                this.Desktop.CredentialsId = this._dataModel.Credentials.AddNewModel(credentials);
            }

            ConnectionInformation connectionInformation = new ConnectionInformation()
            {
                Desktop = this.Desktop,
                Credentials = credentials
            };

            _navigationService.NavigateToView("SessionView", connectionInformation);
        }

        private void DeleteCommandExecute(object o)
        {            
            _dataModel.LocalWorkspace.Connections.RemoveModel(this.DesktopId);

            _telemetryClient.CastAndCall<Telemetry.ITelemetryClient>(tc =>
                tc.ReportEvent(new Telemetry.Events.RemovedDesktop(_dataModel.LocalWorkspace.Connections.Models.Count)));
        }

        // updates the tiles' sizes based on screen resolution and phone/tablet mode
        private void UpdateTileSize()
        {
            throw new NotImplementedException();
        }
    }
}
