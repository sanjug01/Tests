namespace RdClient.Controls
{
    using RdClient.Shared.ValidationRules;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class ErrorControl : UserControl
    {
        public ErrorControl()
        {
            this.InitializeComponent();
            this.ErrorBorder.Visibility = Visibility.Collapsed;
        }        

        static ErrorControl()
        {
            PresentedContentProperty = DependencyProperty.Register("PresentedContent", typeof(UIElement), typeof(ErrorControl), new PropertyMetadata(null, PresentedContentChanged));
            PropertyProperty = DependencyProperty.Register("Property", typeof(ValidatedProperty<string>), typeof(ErrorControl), new PropertyMetadata(null, PropertyPropertyChanged));
        } 

        public UIElement PresentedContent
        {
            private get { return GetValue(PresentedContentProperty) as UIElement; }
            set { SetValue(PresentedContentProperty, value); }
        }

        public ValidatedProperty<string> Property
        {
            private get { return GetValue(PropertyProperty) as ValidatedProperty<string>; }
            set { SetValue(PropertyProperty, value); }
        }

        public static readonly DependencyProperty PresentedContentProperty;
        public static readonly DependencyProperty PropertyProperty;

        private static void PresentedContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var errorControl = d as ErrorControl;
            var newContent = e.NewValue as UIElement;
            (d as ErrorControl).ContentPresenter.Content = newContent;
            newContent.LostFocus += (sender, args) =>
            {
                if (!errorControl.Property.State.IsValid)
                {
                    errorControl.ErrorBorder.Visibility = Visibility.Visible;
                }
            };
        }

        private static void PropertyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var errorControl = d as ErrorControl;
            var property = e.NewValue as ValidatedProperty<string>;
            property.PropertyChanged += (sender, args) =>
            {
                if (string.Equals("Value", args.PropertyName))
                {
                    errorControl.ErrorBorder.Visibility = Visibility.Collapsed;
                }
            };
        }
    }
}
