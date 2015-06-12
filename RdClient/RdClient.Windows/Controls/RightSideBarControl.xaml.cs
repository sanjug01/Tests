namespace RdClient.Controls
{
    using RdClient.Shared.Models;
    using System.Diagnostics.Contracts;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Animation;

    public sealed partial class RightSideBarControl : UserControl
    {
        private Storyboard _activeStoryboard;

        public static readonly DependencyProperty BarVisibilityProperty = DependencyProperty.Register("BarVisibility",
            typeof(Windows.UI.Xaml.Visibility), typeof(RightSideBarControl),
            new PropertyMetadata(Windows.UI.Xaml.Visibility.Collapsed, OnRightSideBarVisibilityChanged));

        public Windows.UI.Xaml.Visibility BarVisibility
        {
            get
            {
                return (Visibility)GetValue(BarVisibilityProperty);
            }
            set
            {
                SetValue(BarVisibilityProperty, value);
            }
        }

        public RightSideBarControl()
        {
            this.InitializeComponent();
            this.ShowRightSideBarStoryboard.Completed += this.ShowRightSideBarCompleted;
            this.HideRightSideBarStoryboard.Completed += this.HideRightSideBarCompleted;
        }
        //
        // Implementation
        //
        private void ShowRightSideBarCompleted(object sender, object e)
        {
            if (object.ReferenceEquals(_activeStoryboard, sender))
            {
                _activeStoryboard = null;
            }
        }

        private void HideRightSideBarCompleted(object sender, object e)
        {
            if (object.ReferenceEquals(_activeStoryboard, sender))
            {
                _activeStoryboard = null;
            }

            this.RightSideBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private static void OnRightSideBarVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((RightSideBarControl)sender).InternalOnRightSideBarVisibilityChanged(e);
        }

        private void InternalOnRightSideBarVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != _activeStoryboard)
            {
                _activeStoryboard.SkipToFill();
                _activeStoryboard.Stop();
                _activeStoryboard = null;
            }

            switch ((Windows.UI.Xaml.Visibility)e.NewValue)
            {
                case Windows.UI.Xaml.Visibility.Visible:
                    _activeStoryboard = this.ShowRightSideBarStoryboard;
                    this.RightSideBarTransform.X = this.RightSideBar.ActualWidth;
                    this.RightSideBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    this.ShowRightSideBarAnimation.From = this.RightSideBar.ActualWidth;
                    this.ShowRightSideBarAnimation.To = 0.0;
                    break;

                case Windows.UI.Xaml.Visibility.Collapsed:
                    _activeStoryboard = this.HideRightSideBarStoryboard;
                    this.HideRightSideBarAnimation.From = 0.0;
                    this.HideRightSideBarAnimation.To = this.RightSideBar.ActualWidth;
                    break;
            }

            // Binding Visibility to {Binding Source={StaticResource DeviceCapabilities}, Path=TouchPresent, Converter={StaticResource BooleanToVisibilityConverter}}
            // does not work for some reason :(
            //if (((IDeviceCapabilities)Application.Current.Resources["DeviceCapabilities"]).TouchPresent)
            //{
            //    this.MouseModeButton.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    this.MouseModeButton.Visibility = Visibility.Collapsed;
            //}

            Contract.Assert(null != _activeStoryboard);
            _activeStoryboard.Begin();
        }
    }
}
