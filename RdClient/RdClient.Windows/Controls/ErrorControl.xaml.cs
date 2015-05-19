namespace RdClient.Controls
{
    using RdClient.Shared.ValidationRules;
    using RdClient.Shared.ViewModels;
    using System.Diagnostics;
    using Windows.System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;

    public sealed partial class ErrorControl : UserControl
    {
        public ErrorControl()
        {
            this.InitializeComponent();            
            this.DataContextChanged += ErrorControl_DataContextChanged;
            this.ErrorButton.Command = new RelayCommand(o => FlyoutBase.ShowAttachedFlyout(this.ErrorBorder));
            this.HideErrors();
        }

        static ErrorControl()
        {
            PresentedContentDP = DependencyProperty.Register("PresentedContent", typeof(UIElement), typeof(ErrorControl), new PropertyMetadata(null, PresentedContentChanged));
        }        

        public UIElement PresentedContent
        {
            private get { return GetValue(PresentedContentDP) as UIElement; }
            set { SetValue(PresentedContentDP, value); }
        }

        public static readonly DependencyProperty PresentedContentDP;

        private static void PresentedContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var errorControl = d as ErrorControl;            
            var content = e.NewValue as UIElement;
            if (errorControl != null)
            {
                //show the new content
                errorControl.ContentPresenter.Content = content;
                
                if (content != null)
                {
                    // Show any errors after user has finished editing the content
                    content.LostFocus += (sender, args) =>
                    {
                        Debug.WriteLine("Lost focus");
                        errorControl.ShowErrors();
                    };
                    // Show any errors after user tries to submit by pressing enter
                    content.KeyDown += (sender, args) =>
                    {
                        if (args.Key == VirtualKey.Enter)
                        {
                            Debug.WriteLine("Enter pressed");
                            errorControl.ShowErrors();
                        }
                    };
                    // Hide any displayed errors once the user starts editing the content to fix them
                    // (Any errors will be shown again once they finish editing or try to submit)
                    content.GotFocus += (sender, args) =>
                    {
                        Debug.WriteLine("Got focus");
                        errorControl.HideErrors();
                    };
                }
            }
        }

        private void ErrorControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var property = args.NewValue as ValidatedProperty<string>;
            if (property != null)
            {
                // When the user presses enter to submit we want to show the error only until they start editing the value to fix it.
                // (Then we hide the error until they finish editing or try to submit again)
                property.PropertyChanged += (s, e) =>
                {
                    if (string.Equals("Value", e.PropertyName))
                    {
                        Debug.WriteLine("Value changed");
                        this.HideErrors();
                    }
                };
            }
        }

        //Make the error border visible if there are any errors to display
        private void ShowErrors()
        {
            var property = this.DataContext as ValidatedProperty<string>;
            if (property?.State?.IsValid == false)
            {
                this.ErrorBorder.Visibility = Visibility.Visible;
                this.ErrorButton.Visibility = Visibility.Visible;
            }
        }

        private void HideErrors()
        {            
            this.ErrorBorder.Visibility = Visibility.Collapsed;
            this.ErrorButton.Visibility = Visibility.Collapsed;        
            FlyoutBase.GetAttachedFlyout(this.ErrorBorder).Hide();
        }
    }
}
