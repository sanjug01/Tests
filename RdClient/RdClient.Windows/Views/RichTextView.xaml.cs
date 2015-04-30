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
            this.InitializeComponent();
        }

        private async void RichEditBox_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            StorageFile infoFile = await StorageFile.GetFileFromApplicationUriAsync(
                new Uri("ms-appx:///Strings/EULA.rtf"));
            _infoText = await FileIO.ReadTextAsync(infoFile);

            // apply text
            this.InfoBox.IsReadOnly = false;
            this.InfoBox.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, _infoText);
            this.InfoBox.IsReadOnly = true;
        }

        private string _infoText;
    }
}
