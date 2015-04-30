namespace RdClient.Views
{
    using RdClient.Controls;
    using System;
    using Windows.UI.Xaml.Controls;
    using Windows.Storage;

    public sealed partial class RichTextView : ModalUserControl
    {
        public RichTextView()
        {
            // TODO: _resourceFile should be bindable
            _resourceUri = "ms-appx:///Strings/EULA.rtf";
            this.InitializeComponent();
        }

        private void RichEditBox_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.LoadResourceFile();
        }

        private async void LoadResourceFile()
        {
            if(!string.IsNullOrEmpty(_resourceUri))
            {
                try
                {
                    StorageFile infoFile = await StorageFile.GetFileFromApplicationUriAsync(
                        new Uri(_resourceUri));
                    _infoText = await FileIO.ReadTextAsync(infoFile);
                }
                catch (Exception exc)
                {
                    // TODO : remove try/catch in released version.
                    _infoText = "Error loading resource file .... " + exc.Message;
                }

                this.ApplyRichText(_infoText);
            }
            else
            {
                // no resource file available
                this.ApplyRichText(string.Empty) ;
            }
        }

        private void ApplyRichText(string text)
        {
            // apply text
            this.InfoBox.IsReadOnly = false;
            this.InfoBox.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, text);
            this.InfoBox.IsReadOnly = true;
            // bug with the foreground color
            this.InfoBox.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        private string _resourceUri;
        private string _infoText;
    }
}
