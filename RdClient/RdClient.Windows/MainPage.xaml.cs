namespace RdClient
{
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            //
            // Set initial orientation.
            // TODO: handle orientation based on the actual dimentions of the view.
            //
            DisplayInformation di = DisplayInformation.GetForCurrentView();
            OnOrientationChanged(di, null);
            di.OrientationChanged += OnOrientationChanged;

            this.NavigationService.PushingFirstModalView += (s, e) => this.ViewPresenter.PresentingFirstModalView();
            this.NavigationService.DismissingLastModalView += (s, e) => this.ViewPresenter.DismissedLastModalView();

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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.ViewFactory.AddViewClass("ConnectionCenterView", typeof(Views.ConnectionCenterView));
            this.ViewFactory.AddViewClass("SessionView", typeof(Views.SessionView));
            this.ViewFactory.AddViewClass("TestsView", typeof(Views.TestsView));
            this.ViewFactory.AddViewClass("AddOrEditDesktopView", typeof(Views.AddOrEditDesktopView));
            this.ViewFactory.AddViewClass("DialogMessage", typeof(Views.DialogMessage));

            this.NavigationService.Presenter = this;
            this.NavigationService.AppBarViewModel = this.DataContext as IApplicationBarViewModel;

            this.NavigationService.NavigateToView("ConnectionCenterView", null);
            this.NavigationService.PushModalView("DialogMessage", new DialogMessageArgs("this is a message", () => { }, () => { }));
        }
    }
}
