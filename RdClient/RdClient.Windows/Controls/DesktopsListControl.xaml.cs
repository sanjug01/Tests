namespace RdClient.Controls
{
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class DesktopsListControl : UserControl, IItemsView
    {
        public static readonly DependencyProperty ViewItemsSourceProperty = DependencyProperty.Register("ViewItemsSource",
            typeof(IViewItemsSource), typeof(DesktopsListControl),
            new PropertyMetadata(null, (sender, e) => ((DesktopsListControl)sender).OnViewItemsSourceChanged(e)));

        public DesktopsListControl()
        {
            this.InitializeComponent();
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
                UIElement container = this.DesktopItemsControl.ContainerFromItem(item) as UIElement;

                if(null != container)
                {
                    Point pt = container.TransformToVisual(this.Scroller).TransformPoint(new Point(0, 0));
                    this.Scroller.ChangeView(pt.X, pt.Y, null);
                }
            }
        }

        private void OnViewItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            e.OldValue.CastAndCall<IViewItemsSource>(src => src.SetItemsView(null));
            e.NewValue.CastAndCall<IViewItemsSource>(src => src.SetItemsView(this));
        }
    }
}
