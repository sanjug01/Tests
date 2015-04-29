namespace RdClient.Controls
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class ErrorControl : UserControl
    {
        public ErrorControl()
        {
            this.InitializeComponent();
        }        

        static ErrorControl()
        {
            PresentedContentProperty = DependencyProperty.Register("PresentedContent", typeof(UIElement), typeof(ErrorControl), new PropertyMetadata(null, PresentedContentChanged));
        } 

        public UIElement PresentedContent
        {
            set { SetValue(PresentedContentProperty, value); }
        }

        public static readonly DependencyProperty PresentedContentProperty;

        private static void PresentedContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ErrorControl).ContentPresenter.Content = e.NewValue as UIElement;
        }
    }
}
