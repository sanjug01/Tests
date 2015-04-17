// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Views
{
    using RdClient.Shared.Navigation;
    using System.Diagnostics.Contracts;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using System;


    /// <summary>
    /// Stock implementation of IViewPresenter hosted on the main page (MainPage.xaml)
    /// and passed to the navigation service in the main page's view controller.
    /// </summary>
    public sealed partial class ViewPresenter : UserControl, IViewPresenter, IStackedViewPresenter
    {
        private int _modalStackDepth;

        public ViewPresenter()
        {
            this.InitializeComponent();
            _modalStackDepth = 0;
        }

        void IViewPresenter.PresentView(IPresentableView view)
        {
            Contract.Requires(view != null);
            Contract.Assert(view is UIElement);

            this.TransitionAnimationContainer.ShowContent(view as UIElement);
        }

        void IStackedViewPresenter.PushView(IPresentableView view, bool animated)
        {
            Contract.Requires(view != null);
            Contract.Assert(view is UIElement);

            if (0 == _modalStackDepth++)
                OnPushedFirstModalView();

            this.ModalStackContainer.Push(view as UIElement, animated);
        }

        void IStackedViewPresenter.DismissView(IPresentableView view, bool animated)
        {
            Contract.Requires(view != null);
            Contract.Assert(view is UIElement);
            Contract.Assert(_modalStackDepth > 0);

            this.ModalStackContainer.Pop(view as UIElement, animated);

            if (0 == --_modalStackDepth)
                OnDismissedLastModalView();
        }

        private void OnPushedFirstModalView()
        {
            this.TransitionAnimationContainer.IsEnabled = false;
            this.ModalStackContainer.Visibility = Visibility.Visible;
        }

        public void OnDismissedLastModalView()
        {
            this.ModalStackContainer.Visibility = Visibility.Collapsed;
            this.TransitionAnimationContainer.IsEnabled = true;
        }
    }
}
