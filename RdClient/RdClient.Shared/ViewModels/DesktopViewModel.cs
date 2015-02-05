namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Windows.Input;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml.Media.Imaging;

    public class DesktopViewModel : Helpers.MutableObject, IDesktopViewModel
    {
        private readonly RelayCommand _editCommand;
        private readonly RelayCommand _connectCommand;
        private readonly RelayCommand _deleteCommand;
        private readonly DesktopModel _desktop;
        private readonly ApplicationDataModel _dataModel;
        private readonly Guid _desktopId;
        private bool _isSelected;
        private bool _selectionEnabled;
        private BitmapImage _thumbnailImage;
        private IExecutionDeferrer _executionDeferrer;
        private bool _thumbnailUpdateNeeded;
        private bool _hasThumbnailImage;

        public DesktopViewModel(DesktopModel desktop, Guid desktopId, ApplicationDataModel dataModel, IExecutionDeferrer executionDeferrer)
        {
            Contract.Assert(null != desktop);
            Contract.Assert(!desktopId.Equals(Guid.Empty));

            _editCommand = new RelayCommand(EditCommandExecute);
            _connectCommand = new RelayCommand(ConnectCommandExecute);
            _deleteCommand = new RelayCommand(DeleteCommandExecute);
            _thumbnailUpdateNeeded = true;
            _executionDeferrer = executionDeferrer;

            _desktop = desktop;
            _desktopId = desktopId;

            /// DesktopVieModel does not require Presenting/Dismissing, 
            //          but stil needs DataModel and NavigationService
            //          NavigationService may be initialized later while presenting the parent view
            _dataModel = dataModel;
            this.Thumbnail.PropertyChanged += Thumbnail_PropertyChanged;
            this.UpdateThumbnailImage(this.Thumbnail.EncodedImageBytes);
        }

        public void Presented()
        {
            if (_thumbnailUpdateNeeded)
            {
                UpdateThumbnailImage(this.Thumbnail.EncodedImageBytes);
            }
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

        public BitmapImage ThumbnailImage
        {
            get
            {
                return _thumbnailImage;
            }
            private set
            {
                SetProperty(ref _thumbnailImage, value);
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
                Thumbnail = this.Thumbnail
            };

            NavigationService.NavigateToView("SessionView", connectionInformation);            
        }

        private void DeleteCommandExecute(object o)
        {
            NavigationService.PushModalView("DeleteDesktopsView", new DeleteDesktopsArgs(TemporaryModelContainer<DesktopModel>.WrapModel(_desktopId, _desktop)));            
        }

        private void Thumbnail_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.Thumbnail != null && "EncodedImageBytes".Equals(e.PropertyName))
            {
                UpdateThumbnailImage(this.Thumbnail.EncodedImageBytes);
            }
        }

        private void UpdateThumbnailImage(byte[] imageBytes)
        {
            if (imageBytes != null)
            {
                _thumbnailUpdateNeeded = !_executionDeferrer.TryDeferToUI(
                    async () =>
                    {
                        BitmapImage newImage = new BitmapImage();
                        using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
                        {
                            await stream.WriteAsync(imageBytes.AsBuffer());
                            stream.Seek(0);
                            await newImage.SetSourceAsync(stream);
                        }
                        this.ThumbnailImage = newImage;

                        this.HasThumbnailImage = _dataModel.Settings.UseThumbnails 
                                                    && this.ThumbnailImage != null 
                                                    && this.ThumbnailImage.PixelHeight > 0 
                                                    && this.ThumbnailImage.PixelHeight > 0;
                    });
            }
        }
    }
}
