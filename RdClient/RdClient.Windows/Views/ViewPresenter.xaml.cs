// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Views
{
    using RdClient.Shared.Navigation;
    using System;
    using System.Diagnostics.Contracts;
    using Windows.UI.Xaml;
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

        public void PresentingFirstModalView()
        {
            //
            // TODO: get rid of this dependency
            //
            this.TransitionAnimationContainer.IsEnabled = false;
            this.ModalStackContainer.Visibility = Visibility.Visible;
        }

        public void DismissedLastModalView()
        {
            //
            // TODO: get rid of this dependency
            //
            this.ModalStackContainer.Visibility = Visibility.Collapsed;
            this.TransitionAnimationContainer.IsEnabled = true;
        }

        void IViewPresenter.PresentView(IPresentableView view)
        {
            Contract.Requires(view != null);
            Contract.Assert(view is UIElement);

            this.TransitionAnimationContainer.ShowContent(view as UIElement);
        }

        void IViewPresenter.PushModalView(IPresentableView view)
        {
            Contract.Requires(view != null);
            Contract.Assert(view is UIElement);

            this.ModalStackContainer.Push(view as UIElement);
        }

        void IViewPresenter.DismissModalView(IPresentableView view)
        {
            Contract.Requires(view != null);

            this.ModalStackContainer.Pop();
        }
    }
}
