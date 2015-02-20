namespace RdClient
{
    using RdClient.Factories;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels;
    using System.Diagnostics.Contracts;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            AppInitializer initializer = (this.Resources["AppInitializer"] as AppInitializer);
            Contract.Assert(null != initializer);

            initializer.AppBarViewModel = this.DataContext as IApplicationBarViewModel;
            initializer.ViewPresenter = this.ViewPresenter;
            initializer.BackButton = this.AppHeader.BackButton;
            initializer.Initialiaze();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Loaded += OnPageLoaded;
            this.Unloaded += OnPageUnloaded;
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
    }
}
