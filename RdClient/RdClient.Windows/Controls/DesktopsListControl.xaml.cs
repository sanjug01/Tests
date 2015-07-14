namespace RdClient.Controls
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;

    public sealed partial class DesktopsListControl : UserControl, IItemsView
    {
        public static readonly DependencyProperty ViewItemsSourceProperty = DependencyProperty.Register("ViewItemsSource",
            typeof(IViewItemsSource), typeof(DesktopsListControl),
            new PropertyMetadata(null, (sender, e) => ((DesktopsListControl)sender).OnViewItemsSourceChanged(e)));

        public DesktopsListControl()
        {
            this.InitializeComponent();
            this.VisualStates.CurrentStateChanging += this.OnVisualStateChanging;
        }
        
        public IViewItemsSource ViewItemsSource
        {
            get { return (IViewItemsSource)GetValue(ViewItemsSourceProperty); }
            set { SetValue(ViewItemsSourceProperty, value); }
        }

        void IItemsView.SelectItem(IViewItemsSource itemsSource, object item)
        {
            if(object.ReferenceEquals(itemsSource, this.ViewItemsSource))
            {
                this.UpdateLayout();
                UIElement container = this.DesktopItemsControl.ContainerFromItem(item) as UIElement;

                if (null != container)
                {
                    //
                    // Converge the top left corner of the new item with the offset of the scroller
                    // and scroll the scroller to the converged offset, which will bring the new item
                    // into the view.
                    //
                    Point ptOrigin = new Point(this.Scroller.HorizontalOffset, this.Scroller.VerticalOffset);
                    Point ptDestination = container.TransformToVisual(this.Scroller).TransformPoint(ptOrigin);
                    this.Scroller.ChangeView(ptDestination.X, ptDestination.Y, null);
                    //
                    // TODO: highlight the new item temporarily.
                    //

                    //
                    // Set input focus to the new item
                    //
                    Control cc = container.FindFirstTabControl();
                    if (null != cc)
                        cc.Focus(FocusState.Programmatic);
                }
            }
        }

        private void OnViewItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            e.OldValue.CastAndCall<IViewItemsSource>(src => src.SetItemsView(null));
            e.NewValue.CastAndCall<IViewItemsSource>(src => src.SetItemsView(this));
        }

        private void OnVisualStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            foreach (var item in this.DesktopItemsControl.Items)
            {
                var contentPresenter = this.DesktopItemsControl.ContainerFromItem(item);

                int childrenCount = VisualTreeHelper.GetChildrenCount(contentPresenter);
                for (int i = 0; i < childrenCount; i++)
                {
                    var childElement = VisualTreeHelper.GetChild(contentPresenter, i);
                    childElement.CastAndCall<Control>(
                        c => VisualStateManager.GoToState(c, e.NewState.Name, true));
                }

            }
        }

    }
}
