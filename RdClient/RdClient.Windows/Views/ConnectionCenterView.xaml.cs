namespace RdClient.Views
{
    using RdClient.Shared.Navigation;
    using System.Diagnostics.Contracts;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class ConnectionCenterView : Page, IPresentableView, IStackedViewPresenter
    {
        public ConnectionCenterView()
        {
            this.InitializeComponent();
            this.VisualStates.CurrentStateChanging += this.OnVisualStateChanging;
            this.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("SizeChanged :" + e.NewSize);
        }

        IViewModel IPresentableView.ViewModel { get { return this.DataContext as IViewModel; } }
        void IPresentableView.Activating(object activationParameter) { }
        void IPresentableView.Presenting(INavigationService navigationService, object activationParameter) { }
        void IPresentableView.Dismissing() { }

        void IStackedViewPresenter.PushView(IPresentableView view, bool animated)
        {
            Contract.Assert(null != view);
            this.AccessoryViewPresenter.PushView(view, animated);
        }

        void IStackedViewPresenter.DismissView(IPresentableView view, bool animated)
        {
            this.AccessoryViewPresenter.DismissView(view, animated);
        }

        private void OnVisualStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            VisualStateManager.GoToState(this.AccessoryViewPresenter, e.NewState.Name, true);
        }
    }
}
