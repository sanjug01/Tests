// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Views
{
    using RdClient.Shared.Navigation;
    using System;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Stock implementation of IViewPresenter hosted on the main page (MainPage.xaml)
    /// and passed to the navigation service in the main page's view controller.
    /// </summary>
    public sealed partial class ViewPresenter : UserControl, IViewPresenter
    {
        public ViewPresenter()
        {
            this.InitializeComponent();
        }

        void IViewPresenter.PresentView(IPresentableView view)
        {
            throw new NotImplementedException();
        }

        void IViewPresenter.PushModalView(IPresentableView view)
        {
            throw new NotImplementedException();
        }

        void IViewPresenter.DismissModalView(IPresentableView view)
        {
            throw new NotImplementedException();
        }
    }
}
