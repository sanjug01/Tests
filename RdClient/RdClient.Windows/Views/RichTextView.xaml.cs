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
            _resourceFileName = "ms-appx:///Strings/EULA.rtf";
            this.InitializeComponent();
        }

        private void RichEditBox_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.LoadResourceFile();
        }

        private async void LoadResourceFile()
        {
            StorageFile infoFile = await StorageFile.GetFileFromApplicationUriAsync(
                new Uri(_resourceFileName));
            _infoText = await FileIO.ReadTextAsync(infoFile);

            // apply text
            this.InfoBox.IsReadOnly = false;
            this.InfoBox.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, _infoText);
            this.InfoBox.IsReadOnly = true;
            // bug with the foreground color
            this.InfoBox.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        private string _resourceFileName;
        private string _infoText;
    }
}
