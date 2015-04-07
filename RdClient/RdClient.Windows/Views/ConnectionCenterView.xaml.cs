namespace RdClient.Views
{
    using RdClient.Shared.Navigation;
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
            Layout layout = Layout.DefaultLayout;
            //
            // If the view has become narrower than 640 pixels, switch to the narrow layout.
            //
            if (e.NewSize.Width < 640)
                layout = Layout.NarrowLayout;

            VisualStateManager.GoToState(this, layout.ToString(), true);
        }
    }
}
