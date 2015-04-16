﻿// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

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
        public readonly DependencyProperty AccessoryPresenterVisibilityProperty = DependencyProperty.Register(
            "AccessoryPresenterVisibility",
            typeof(IViewVisibility), typeof(AccessoryViewPresenter),
            new PropertyMetadata(null));

        public AccessoryViewPresenter()
        {
            this.InitializeComponent();

            this.AccessoriesContainer.PushingFirstView += this.OnPushingFirstView;
            this.AccessoriesContainer.DismissedLastView += this.OnDismissedLastView;
        }

        public IViewVisibility AccessoryPresenterVisibility
        {
            get { return (IViewVisibility)GetValue(AccessoryPresenterVisibilityProperty); }
            set { SetValue(AccessoryPresenterVisibilityProperty, value); }
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
            IViewVisibility visibility = this.AccessoryPresenterVisibility;
            Contract.Assert(null != visibility);

            visibility.Show();
        }

        private void OnDismissedLastView(object sender, EventArgs e)
        {
            IViewVisibility visibility = this.AccessoryPresenterVisibility;
            Contract.Assert(null != visibility);

            visibility.Hide();
        }
    }
}
