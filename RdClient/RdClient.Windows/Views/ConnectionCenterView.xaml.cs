namespace RdClient.Views
{
    using RdClient.Shared.Navigation;
    using Windows.Graphics.Display;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class ConnectionCenterView : Page, IPresentableView
    {
        //
        // The enum prevents typos in names of visual states. When a new visual state
        // needs to be applied, the name of the state comes from a member of the enum.
        //
        private enum Layout
        {
            DefaultLayout,
            NarrowLayout,
            PhoneLayout
        }

        public ConnectionCenterView()
        {
            this.InitializeComponent();
            this.SizeChanged += this.OnSizeChanged;
        }

        IViewModel IPresentableView.ViewModel { get { return this.DataContext as IViewModel; } }
        void IPresentableView.Activating(object activationParameter) { }
        void IPresentableView.Presenting(INavigationService navigationService, object activationParameter) { }
        void IPresentableView.Dismissing() { }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //
            // Analyze the layout and apply an appropriate visual state.
            //
            DisplayInformation di = DisplayInformation.GetForCurrentView();
            Layout layout = Layout.DefaultLayout;

            if(e.NewSize.Width < 640 * di.RawPixelsPerViewPixel)
            {
                //
                // The view is too small, switch to the phone layout.
                //
                layout = Layout.PhoneLayout;
            }
            else if (e.NewSize.Width < e.NewSize.Height)
            {
                if (e.NewSize.Height / e.NewSize.Width >= 15.0/9.0)
                {
                    //
                    // The view is much taller than it is wide; switch to the phone layout, unless
                    // the view is wider than 1024 virtual pixels.
                    //
                    if (e.NewSize.Width < 1024 * di.RawPixelsPerViewPixel)
                        layout = Layout.PhoneLayout;
                    else
                        layout = Layout.NarrowLayout;
                }
                else if (e.NewSize.Width < 1080 * di.RawPixelsPerViewPixel)
                {
                    //
                    // If the view is narrower than 1080 virtual pixels, switch to the narrow layout.
                    //
                    layout = Layout.NarrowLayout;
                }
            }

            VisualStateManager.GoToState(this, layout.ToString(), true);
        }
    }
}
