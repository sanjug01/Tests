namespace RdClient.Shared.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Windows.Input;
    using Windows.ApplicationModel;
    using Windows.UI.Xaml;
    using Windows.Storage;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.Helpers;
    using System.Threading.Tasks;
    using RdClient.Shared.Navigation;

    public enum InternalDocType
    {
        EulaDoc,
        ThirdPartyNotices,
        PrivacyDoc // TODO: not yet available, currently a link
    }
    public class RichTextViewModelArgs
    {
        public InternalDocType DocumentType { private set; get; }
        public RichTextViewModelArgs(InternalDocType type)
        {
            this.DocumentType = type;
        }
    }

    public sealed class RichTextViewModel : AccessoryViewModelBase, IDeferredExecutionSite
    {
        private readonly RelayCommand _closeCommand;
        private string _infoText;
        private InternalDocType _docType;
        private bool _isLoading;
        private IDeferredExecution _dispatcher;

        public RichTextViewModel()
        {
            _closeCommand = new RelayCommand(o => { this.DismissModal(null); });
            ResourceUri = string.Empty;
            IsLoading = true;
        }

        public ICommand Close { get { return _closeCommand; } }

        /// <summary>
        /// FileName should be empty, or match a resource file.
        /// Other cases may result in exceptions while loading the view. 
        /// </summary>
        private string ResourceUri { get; set; }

        public string Document
        {
            get { return _infoText; }
            private set
            {
                this.SetProperty(ref _infoText, value);
            }
        }

        public InternalDocType DocumentType
        {
            get { return _docType; }
            private set
            {
                this.SetProperty(ref _docType, value);
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set
            {
                this.SetProperty(ref _isLoading, value);
            }
        }

        protected override void DefaultAction()
        {
            DismissModal(null);
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(null != activationParameter as RichTextViewModelArgs);

            RichTextViewModelArgs args = activationParameter as RichTextViewModelArgs;
            this.DocumentType = args.DocumentType;
            switch (args.DocumentType)
            {
                case InternalDocType.EulaDoc:
                    this.ResourceUri = GlobalConstants.EulaUri;
                    break;
                case InternalDocType.ThirdPartyNotices:
                    this.ResourceUri = GlobalConstants.ThirdPartyUri;
                    break;
                case InternalDocType.PrivacyDoc:                    
                    this.ResourceUri = GlobalConstants.PrivacyDocUri;
                    break;
            }

            this.IsLoading = true;
            this.StartLoadingResourceFile();
        }

        private void StartLoadingResourceFile()
        {
            if (!string.IsNullOrEmpty(this.ResourceUri))
            {
                try
                {
                    StorageFile.GetFileFromApplicationUriAsync(new Uri(this.ResourceUri))
                        .AsTask<StorageFile>().ContinueWith(OnStreamOpened);
                }
                catch (Exception exc)
                {
                    System.Diagnostics.Debug.WriteLine("Could not open resource file:" + this.ResourceUri + " because" + exc.Message);
                }
            }
            else
            {
                // this should never happen
                // but until we finalize all internal documents will show the progress bar.
                this.Document = "No document available";
            }           
        }

        private void OnStreamOpened(Task<StorageFile> task)
        {
            try
            {
                FileIO.ReadTextAsync(task.Result).AsTask<string>().ContinueWith(OnTextLoaded);
            }
            catch (Exception exc)
            {
                this.Document = "Failed to read document";
                this.IsLoading = false;
                System.Diagnostics.Debug.WriteLine("Could not read resource file:" + this.ResourceUri + " because" + exc.Message);
            }
        }

        private void OnTextLoaded(Task<string> task)
        {
            string infoText ;
            try
            {
                infoText = task.Result;
            }
            catch
            {
                infoText = string.Empty;
                System.Diagnostics.Debug.WriteLine("Could not read string content from resource file:" + this.ResourceUri);
            }

            // Defer displaying the text
            _dispatcher.CastAndCall<IDeferredExecution>(
                de => de.Defer(() =>
                {
                    this.Document = infoText;
                    this.IsLoading = false;
                } ));
        }

        void IDeferredExecutionSite.SetDeferredExecution(IDeferredExecution defEx)
        {
            _dispatcher = defEx;
        }
    }
}
