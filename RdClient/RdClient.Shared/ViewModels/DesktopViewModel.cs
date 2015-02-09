namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public class DesktopViewModel : Helpers.MutableObject, IDesktopViewModel
    {
        private static readonly uint ThumbnailHeight = 276;

        private readonly RelayCommand _editCommand;
        private readonly RelayCommand _connectCommand;
        private readonly RelayCommand _deleteCommand;
        private readonly Guid _desktopId;
        private readonly DesktopModel _desktop;
        private readonly ApplicationDataModel _dataModel;
        private readonly IThumbnailEncoder _thumbnailEncoder;
        private bool _isSelected;
        private bool _selectionEnabled;
        private IExecutionDeferrer _executionDeferrer;
        private bool _hasThumbnailImage;

        public static IDesktopViewModel Create(IModelContainer<RemoteConnectionModel> desktopContainer, ApplicationDataModel dataModel, IExecutionDeferrer executionDeferrer)
        {
            return new DesktopViewModel(desktopContainer, dataModel, executionDeferrer);
        }

        private DesktopViewModel(IModelContainer<RemoteConnectionModel> desktopContainer, ApplicationDataModel dataModel, IExecutionDeferrer executionDeferrer)
        {
            Contract.Assert(null != desktopContainer);
            Contract.Assert(null != desktopContainer);
            Contract.Assert(!Guid.Empty.Equals(desktopContainer.Id));

            _editCommand = new RelayCommand(EditCommandExecute);
            _connectCommand = new RelayCommand(ConnectCommandExecute);
            _deleteCommand = new RelayCommand(DeleteCommandExecute);
            _thumbnailEncoder = ThumbnailEncoder.Create(ThumbnailHeight);
            _executionDeferrer = executionDeferrer;

            _desktop = (DesktopModel)desktopContainer.Model;
            _desktopId = desktopContainer.Id;
            //
            // DesktopVieModel does not require Presenting/Dismissing, 
            //          but stil needs DataModel and NavigationService
            //          NavigationService may be initialized later while presenting the parent view
            //
            _dataModel = dataModel;
            //
            //
            //
            _thumbnailEncoder.ThumbnailUpdated += this.OnThumbnailUpdated;
        }

        public void Presented()
        {
        }

        public INavigationService NavigationService { private get; set; }

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
                        // TODO: perhaps, remove credentials ID from the desktop
                        //
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
            get { return _hasThumbnailImage; }
            private set { SetProperty(ref _hasThumbnailImage, value); }
        }

        public ThumbnailModel Thumbnail
        {
            get 
            {
                return _desktop.Thumbnail;
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

        private void EditCommandExecute(object o)
        {
            NavigationService.PushModalView("AddOrEditDesktopView", new EditDesktopViewModelArgs(this.Desktop));
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

                NavigationService.PushModalView("AddUserView", args, addUserCompleted);
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
                Credentials = credentials,
                Thumbnail = _thumbnailEncoder
            };

            NavigationService.NavigateToView("SessionView", connectionInformation);            
        }

        private void DeleteCommandExecute(object o)
        {
            NavigationService.PushModalView("DeleteDesktopsView", new DeleteDesktopsArgs(TemporaryModelContainer<DesktopModel>.WrapModel(_desktopId, _desktop)));            
        }

        private void OnThumbnailUpdated(object sender, ThumbnailUpdatedEventArgs e)
        {
            _executionDeferrer.TryDeferToUI(() =>
            {
                this.Thumbnail.EncodedImageBytes = e.EncodedImageBytes;

                this.HasThumbnailImage = _dataModel.Settings.UseThumbnails
                                            && null != this.Thumbnail.Image
                                            && this.Thumbnail.Image.PixelHeight > 0 
                                            && this.Thumbnail.Image.PixelHeight > 0;
            });
        }
    }
}
