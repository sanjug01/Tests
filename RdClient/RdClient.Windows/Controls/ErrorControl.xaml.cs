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
                
                //hook up error display to new content
                if (content != null)
                {
                    // Show any errors after user has finished editing the content
                    content.LostFocus += (sender, args) =>
                    {
                        errorControl.ShowErrors();
                    };
                    
                    content.KeyDown += (sender, args) =>
                    {
                        // Show any errors after user tries to submit by pressing enter
                        if (args.Key == VirtualKey.Enter)
                        {
                            errorControl.ShowErrors();
                        }
                        // Hide any errors when the user is editing
                        else
                        {
                            errorControl.HideErrors();
                        }
                    };

                    // Hide any displayed errors once the user starts editing the content to fix them
                    // (Any errors will be shown again once they finish editing or try to submit)
                    content.GotFocus += (sender, args) =>
                    {
                        errorControl.HideErrors();
                    };
                }
            }
        }

        //Make the error border visible if there are any errors to display
        private void ShowErrors()
        {
            var property = this.DataContext as ValidatedProperty<string>;
            if (property?.State?.IsValid == false)
            {
                this.ErrorBorder.Visibility = Visibility.Visible;
            }
        }

        private void HideErrors()
        {            
            this.ErrorBorder.Visibility = Visibility.Collapsed;     
            FlyoutBase.GetAttachedFlyout(this.ErrorBorder).Hide();
        }
    }
}
