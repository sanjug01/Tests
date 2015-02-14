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
        private bool _isSelected;
        private bool _selectionEnabled;
        private IDeferredExecution _dispatcher;
        private BitmapImage _thumbnail;

        public static IDesktopViewModel Create(IModelContainer<RemoteConnectionModel> desktopContainer,
            ApplicationDataModel dataModel,
            IDeferredExecution dispatcher,
            INavigationService navigationService)
        {
            return new DesktopViewModel(desktopContainer, dataModel, dispatcher, navigationService);
        }

        private DesktopViewModel(IModelContainer<RemoteConnectionModel> desktopContainer,
            ApplicationDataModel dataModel,
            IDeferredExecution dispatcher,
            INavigationService navigationService)
        {
            Contract.Assert(null != desktopContainer);
            Contract.Assert(null != desktopContainer);
            Contract.Assert(null != navigationService);
            Contract.Assert(!Guid.Empty.Equals(desktopContainer.Id));

            _editCommand = new RelayCommand(EditCommandExecute);
            _connectCommand = new RelayCommand(ConnectCommandExecute);
            _deleteCommand = new RelayCommand(DeleteCommandExecute);
            _dispatcher = dispatcher;
            _navigationService = navigationService;

            _desktop = (DesktopModel)desktopContainer.Model;
            _desktopId = desktopContainer.Id;
            //
            // DesktopVieModel does not require Presenting/Dismissing, 
            //          but stil needs DataModel and NavigationService
            //          NavigationService may be initialized later while presenting the parent view
            //
            _dataModel = dataModel;
            //
            // Register for notifications from the thumbnail encoder. The encoder is passed to the session model
            // that hooks it up with a snapshotter that periodically takes screenshots of the remote session and
            // pushes them to the encoder. The encoder resamples screenshots and compresses them for serialization.
            // The view model sends the compressed thimbnails to the thumbnail model that sets its Image property
            // to the thumbnail image.
            //
            _desktop.Thumbnail.ImageUpdated += this.OnAsyncThumbnailUpdated;
            _thumbnail = _desktop.Thumbnail.Image;
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
                        cred = _dataModel.LocalWorkspace.Credentials.GetModel(_desktop.CredentialsId);
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

        public bool HasThumbnailImage
        {
            get { return null != _thumbnail; }
        }

        public BitmapImage Thumbnail
        {
            get { return _thumbnail; }
            set
            {
                if(this.SetProperty(ref _thumbnail, value))
                {
                    EmitPropertyChanged("HasThumbnailImage");
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

        void IDesktopViewModel.Dismissed()
        {
            _desktop.Thumbnail.ImageUpdated -= this.OnAsyncThumbnailUpdated;
        }

        private void EditCommandExecute(object o)
        {
            _navigationService.PushModalView("AddOrEditDesktopView", new EditDesktopViewModelArgs(this.Desktop));
        }

        private void ConnectCommandExecute(object o)
        {
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
        }

        private void InternalConnect(CredentialsModel credentials, bool storeCredentials)
        {
            if(storeCredentials)
            {
                this.Desktop.CredentialsId = this._dataModel.LocalWorkspace.Credentials.AddNewModel(credentials);
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
            _navigationService.PushModalView("DeleteDesktopsView", new DeleteDesktopsArgs(TemporaryModelContainer<DesktopModel>.WrapModel(_desktopId, _desktop)));            
        }

        private void OnAsyncThumbnailUpdated(object sender, EventArgs e)
        {
            _dispatcher.Defer(() =>
            {
                //
                // Pull an image from the thumbnail model and put it in the Thumbnail property
                // to update the view.
                //
                this.Thumbnail = _desktop.Thumbnail.Image;
            });
        }
    }
}
