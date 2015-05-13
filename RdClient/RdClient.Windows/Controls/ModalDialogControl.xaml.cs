namespace RdClient.Controls
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class ModalDialogControl : UserControl
    {
        public ModalDialogControl()
        {
            this.InitializeComponent();
        }

        static ModalDialogControl()
        {
            DialogContentProperty = DependencyProperty.Register("DialogContent", typeof(UIElement), typeof(ModalDialogControl), new PropertyMetadata(null, DialogContentChanged));
        }

        public UIElement DialogContent
        {
            set { SetValue(DialogContentProperty, value); }
        }

        public static readonly DependencyProperty DialogContentProperty;

        private static void DialogContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ModalDialogControl).DialogPresenter.Content = e.NewValue as UIElement;
        }
    }
}
