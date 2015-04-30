namespace RdClient
{
    using RdClient.Factories;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels;
    using System.Diagnostics.Contracts;
    using Windows.Foundation;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    public sealed partial class MainPage : Page
    {
        private enum SharedVisualState
        {
            DefaultLayout,
            NarrowLayout
        }

        public MainPage()
        {
            this.InitializeComponent();
            this.SizeChanged += this.OnSizeChanged;
            this.SharedVisualStates.CurrentStateChanging += this.OnSharedVisualStateChanging;

            AppInitializer initializer = (this.Resources["AppInitializer"] as AppInitializer);
            Contract.Assert(null != initializer);

            initializer.AppBarViewModel = this.DataContext as IApplicationBarViewModel;
            initializer.ViewPresenter = this.ViewPresenter;
            initializer.Initialize();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Loaded += OnPageLoaded;
            this.Unloaded += OnPageUnloaded;
            AppInitializer initializer = (this.Resources["AppInitializer"] as AppInitializer);
            initializer.CreateBackButtonHandler(SystemNavigationManager.GetForCurrentView());            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this.Loaded -= OnPageLoaded;
            this.Unloaded -= OnPageUnloaded;
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += OnWindowSizeChanged;
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= OnWindowSizeChanged;
        }

        private void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            if (!e.Size.IsEmpty)
            {
                //
                // If the view model implements ILayoutAwareViewModel, tell it to update the layout for the new orientation.
                // Ignore flipped orientations, care only about portrait and landscape layouts.
                //
                this.DataContext.CastAndCall<ILayoutAwareViewModel>(lavm =>
                {
                    //
                    // TODO: Maybe implement a more sophisticated layout detection
                    //
                    lavm.OrientationChanged(e.Size.Height < e.Size.Width ? ViewOrientation.Landscape : ViewOrientation.Portrait);
                });
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            VisualStateManager.GoToState(this, GetVisualState(e.NewSize).ToString(), true);
        }

        private SharedVisualState GetVisualState(Size size)
        {
            SharedVisualState state = SharedVisualState.DefaultLayout;

            if (size.Width <= 640.0)
                state = SharedVisualState.NarrowLayout;

            return state;
        }

        private void OnSharedVisualStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            VisualStateManager.GoToState(this.ViewPresenter, e.NewState.Name, true);
        }
    }
}
