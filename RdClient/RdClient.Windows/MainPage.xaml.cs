namespace RdClient
{
    using RdClient.Factories;
    using RdClient.Shared.Input.Keyboard;
    using System.Diagnostics.Contracts;
    using Windows.Foundation;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    public sealed partial class MainPage : Page, IInputPanelFactory
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

            initializer.ViewPresenter = this.ViewPresenter;
            initializer.InputPanelFactory = this;
            initializer.Initialize();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AppInitializer initializer = (this.Resources["AppInitializer"] as AppInitializer);
            initializer.CreateBackButtonHandler(SystemNavigationManager.GetForCurrentView());            
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            VisualStateManager.GoToState(this, GetVisualState(e.NewSize).ToString(), true);
        }

        private SharedVisualState GetVisualState(Size size)
        {
            SharedVisualState state = SharedVisualState.DefaultLayout;
            
            if (size.Width <= RdClient.Shared.Helpers.GlobalConstants.NarrowLayoutMaxWidth)
                state = SharedVisualState.NarrowLayout;

            return state;
        }

        private void OnSharedVisualStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            VisualStateManager.GoToState(this.ViewPresenter, e.NewState.Name, true);
        }

        IInputPanel IInputPanelFactory.GetInputPanel()
        {
            return this.TouchKeyboardActivator;
        }
    }
}
