using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System.ComponentModel;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System;
using System.Threading.Tasks;
using RdClient.Shared.Navigation.Extensions;

namespace RdClient.Shared.ViewModels
{
    public class DesktopViewModel : Helpers.MutableObject, IDesktopViewModel
    {
        private readonly RelayCommand _editCommand;
        private readonly RelayCommand _connectCommand;
        private readonly RelayCommand _deleteCommand;
        private Desktop _desktop;
        private bool _isSelected;
        private bool _selectionEnabled;
        private RdDataModel _dataModel;
        private BitmapImage _thumbnailImage;
        private IExecutionDeferrer _executionDeferrer;
        private bool _thumbnailUpdateNeeded;
        private bool _hasThumbnailImage;

        public DesktopViewModel(Desktop desktop, INavigationService navService, RdDataModel dataModel, IExecutionDeferrer executionDeferrer)
        {
            _editCommand = new RelayCommand(EditCommandExecute);
            _connectCommand = new RelayCommand(ConnectCommandExecute);
            _deleteCommand = new RelayCommand(DeleteCommandExecute);
            _thumbnailUpdateNeeded = true;
            _executionDeferrer = executionDeferrer;
            this.Desktop = desktop;

            /// DesktopVieModel does not require Presenting/Dismissing, 
            //          but stil needs DataModel and NavigationService
            //          NavigationService may be initialized later while presenting the parent view
            _dataModel = dataModel;
            this.NavigationService = navService;
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

        public Desktop Desktop
        {
            get { return _desktop; }
            private set 
            { 
                SetProperty(ref _desktop, value);
            }
        }

        public Credentials Credential
        {
            get 
            {
                Credentials cred;
                if (this.Desktop.HasCredential)
                {
                    cred = _dataModel.LocalWorkspace.Credentials.GetItemWithId(this.Desktop.CredentialId);
                }
                else
                {
                    cred = null;
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

        public Thumbnail Thumbnail
        {
            get 
            { 
                if (!this.Desktop.HasThumbnail || !_dataModel.Thumbnails.ContainsItemWithId(this.Desktop.ThumbnailId))
                {
                    Thumbnail thumbnail = new Thumbnail();
                    this.Desktop.ThumbnailId = thumbnail.Id;
                    _dataModel.Thumbnails.Add(thumbnail);
                }
                return _dataModel.Thumbnails.GetItemWithId(this.Desktop.ThumbnailId);
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
            if (this.Credential != null)
            {
                InternalConnect(this.Credential, false);
            }
            else
            {
                AddUserViewArgs args = new AddUserViewArgs(new Credentials(), true);
                ModalPresentationCompletion addUserCompleted = new ModalPresentationCompletion();
                addUserCompleted.Completed += (s, e) =>
                    {
                        CredentialPromptResult result = e.Result as CredentialPromptResult;
                        if (result != null && !result.UserCancelled)
                        {
                            InternalConnect(result.Credential, result.Save);
                        }
                    };
                NavigationService.PushModalView("AddUserView", args, addUserCompleted);
            }            
        }

        private void InternalConnect(Credentials credentials, bool storeCredentials)
        {
            if(storeCredentials)
            {
                this.Desktop.CredentialId = credentials.Id;
                this._dataModel.LocalWorkspace.Credentials.Add(credentials);
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
            NavigationService.PushModalView("DeleteDesktopsView", new DeleteDesktopsArgs(this.Desktop));            
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
