namespace RdClient.Shared.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Windows.Input;
    using Windows.ApplicationModel;
    using Windows.UI.Xaml;

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

    public sealed class RichTextViewModel : AccessoryViewModelBase
    {
        private readonly RelayCommand _closeCommand;
        private string _resourceFileUri;

        public RichTextViewModel()
        {
            _closeCommand = new RelayCommand(o => { this.DismissModal(null); });
            ResourceUri = string.Empty;
        }

        public ICommand Close { get { return _closeCommand; } }

        /// <summary>
        /// FileName should be empty, or match a resource file.
        /// Other cases may result in exceptions while loading the view. 
        /// </summary>
        public String ResourceUri
        {
            get { return _resourceFileUri; }
            private set
            {
                this.SetProperty(ref _resourceFileUri, value);
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
            switch (args.DocumentType)
            {
                case InternalDocType.EulaDoc:
                    this.ResourceUri = "ms-appx:///Strings/EULA.rtf";
                    break;
                case InternalDocType.ThirdPartyNotices:
                    this.ResourceUri = "ms-appx:///Strings/ThirdPartyNotices.rtf";
                    break;
                case InternalDocType.PrivacyDoc:
                    // TODO: doc not yet available, using link until then
                    this.ResourceUri = string.Empty;
                    break;
            }
        }
    }
}
