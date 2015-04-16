// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Controls
{
    using RdClient.Shared.Navigation;
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class AccessoryViewPresenter : UserControl, IStackedViewPresenter
    {
        /// <summary>
        /// Dependency property ICommand that the control calls to tell a bound view model that it must
        /// show the presenter. In response to this command the view model must update a property bound
        /// to visibility of the presenter and any overlay views that dismiss it.
        /// </summary>
        public readonly DependencyProperty ShowPresenterProperty = DependencyProperty.Register("ShowPresenter",
            typeof(ICommand), typeof(AccessoryViewPresenter),
            new PropertyMetadata(null));

        /// <summary>
        /// Dependency property ICommand that the control calls to tell a bound view model that it must
        /// hide the presenter. In response to this command the view model must update a property bound
        /// to visibility of the presenter and any overlay views that dismiss it.
        /// </summary>
        public readonly DependencyProperty HidePresenterProperty = DependencyProperty.Register("HidePresenter",
            typeof(ICommand), typeof(AccessoryViewPresenter),
            new PropertyMetadata(null));

        public AccessoryViewPresenter()
        {
            this.InitializeComponent();

            this.AccessoriesContainer.PushingFirstView += this.OnPushingFirstView;
            this.AccessoriesContainer.DismissedLastView += this.OnDismissedLastView;
        }

        public ICommand ShowPresenter
        {
            get { return (ICommand)GetValue(ShowPresenterProperty); }
            set { SetValue(ShowPresenterProperty, value); }
        }

        public ICommand HidePresenter
        {
            get { return (ICommand)GetValue(HidePresenterProperty); }
            set { SetValue(HidePresenterProperty, value); }
        }

        void IStackedViewPresenter.PushView(IPresentableView view, bool animated)
        {
            Contract.Assert(view is UIElement);
            this.AccessoriesContainer.Push((UIElement)view, animated);
        }

        void IStackedViewPresenter.DismissView(IPresentableView view, bool animated)
        {
            this.AccessoriesContainer.Pop((UIElement)view, animated);
        }

        private void OnPushingFirstView(object sender, EventArgs e)
        {
            ICommand command = this.ShowPresenter;

            if (null != command && command.CanExecute(null))
                command.Execute(null);
        }

        private void OnDismissedLastView(object sender, EventArgs e)
        {
            ICommand command = this.HidePresenter;

            if (null != command && command.CanExecute(null))
                command.Execute(null);
        }
    }
}
