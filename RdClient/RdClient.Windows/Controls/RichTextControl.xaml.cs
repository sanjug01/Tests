using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient.Controls
{
    public sealed partial class RichTextControl : UserControl
    {
        public static readonly DependencyProperty RtfDocumentProperty = DependencyProperty.Register("RtfDocument",
            typeof(object), typeof(RichTextControl),
            new PropertyMetadata(string.Empty, OnDocumentChanged));

        public RichTextControl()
        {
            this.InitializeComponent();
        }

        public string RtfDocument
        {
            get { return (string)GetValue(RtfDocumentProperty); }
            set { SetValue(RtfDocumentProperty, value); }
        }

        private static void OnDocumentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((RichTextControl)sender).InternalOnDocumentChanged(e);
        }

        private void InternalOnDocumentChanged(DependencyPropertyChangedEventArgs e)
        {
            string text = (string) e.NewValue;
            ////// apply text
            this.InfoBox.IsReadOnly = false;
            this.InfoBox.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, text);
            this.InfoBox.IsReadOnly = true;

            // bug with the foreground color
            this.InfoBox.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

    }
}
