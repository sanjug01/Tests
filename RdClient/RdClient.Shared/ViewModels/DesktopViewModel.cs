namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using Windows.UI.Xaml.Media.Imaging;

    public class DesktopViewModel : Helpers.MutableObject, IDesktopViewModel
    {
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
            INavigationService navigationService)
        {
            return new DesktopViewModel(desktopContainer, dataModel, navigationService);
        }

        private DesktopViewModel(IModelContainer<RemoteConnectionModel> desktopContainer,
            ApplicationDataModel dataModel,
            INavigationService navigationService)
        {
            Contract.Assert(null != desktopContainer);
            Contract.Assert(null != desktopContainer);
            Contract.Assert(null != navigationService);
            Contract.Assert(!Guid.Empty.Equals(desktopContainer.Id));

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
            IRemoteSession session = _sessionFactory.CreateSession(sessionSetup);

            _navigationService.NavigateToView("RemoteSessionView", session);
#if false
            if (this.Credentials != null)
            {
                InternalConnect(this.Credentials, false);
            }
            else
            {
                AddUserViewArgs args = new AddUserViewArgs(new CredentialsModel(), true);

                ModalPresentationCompletion addUserCompleted = new ModalPresentationCompletion();

                addUserCompleted.Completed += (s, e) =>
                {
                    CredentialPromptResult result = e.Result as CredentialPromptResult;

                    if (result != null && !result.UserCancelled)
                    {
                        InternalConnect(result.Credentials, result.Save);
                    }
                };

                _navigationService.PushModalView("AddUserView", args, addUserCompleted);
            }            
#endif
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
            _navigationService.PushAccessoryView("DeleteDesktopsView", new DeleteDesktopsArgs(TemporaryModelContainer<DesktopModel>.WrapModel(_desktopId, _desktop)));            
        }
    }
}
