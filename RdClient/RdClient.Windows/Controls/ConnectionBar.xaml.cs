using RdClient.Shared.Models;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace RdClient.Controls
{
    public sealed partial class ConnectionBar : UserControl
    {
        private const double _deceleration = 0.05;
        private FrameworkElement _container;
        private ReadOnlyObservableCollection<object> _itemsSource;

        private static void SetButtonsEnabled(ReadOnlyObservableCollection<object> itemSource, bool state)
        {
            if (itemSource != null)
            {
                foreach (object o in itemSource)
                {
                    if (o is SymbolBarButtonModel)
                    {
                        ((SymbolBarButtonModel)o).IsEnabled = state;
                    }
                }
            }
        }

        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            this.Grid.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            SetButtonsEnabled(_itemsSource, false);
        }

        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            SetButtonsEnabled(_itemsSource, true);
            this.Grid.Background = null;
        }

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            this.MoveConnectionBar(_container.ActualWidth, e.Delta.Translation.X);
        }

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingRoutedEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = _deceleration;
        }

        private void MoveConnectionBar(double containerWidth, double dx)
        {
            double connectionBarWidth = this.ItemsControl.ActualWidth;
            double maxLeft = -((containerWidth / 2) - (connectionBarWidth / 2));
            double maxRight = ((containerWidth / 2) - (connectionBarWidth / 2));
            double position = this.ConnectionBarTransform.X;
            
            if (position + dx < maxLeft)
            {
                position = maxLeft;
            }
            else if (position + dx > maxRight)
            {
                position = maxRight;
            }
            else
            {
                position += dx;
            }
            this.ConnectionBarTransform.X = position;
        }

        private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Contract.Assert(sender is ConnectionBar);
            Contract.Assert(null == e.NewValue || e.NewValue is ReadOnlyObservableCollection<object>);

            ((ConnectionBar)sender).OnItemsSourceChanged((ReadOnlyObservableCollection<object>)e.OldValue, (ReadOnlyObservableCollection<object>)e.NewValue);
        }
        private void OnItemsSourceChanged(ReadOnlyObservableCollection<object> oldSource, ReadOnlyObservableCollection<object> newSource)
        {
            _itemsSource = newSource;
            this.ItemsControl.ItemsSource = newSource;
        }
        private void ContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.MoveConnectionBar(e.NewSize.Width, 0.0);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this._container = ((FrameworkElement)this.Parent);
            _container.SizeChanged += this.ContainerSizeChanged;
        }

        private void OnUnLoaded(object sender, RoutedEventArgs e)
        {
           _container.SizeChanged -= this.ContainerSizeChanged;
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource",
        typeof(object),
        typeof(ConnectionBar),
        new PropertyMetadata(null, OnItemsSourceChanged));

        public ReadOnlyObservableCollection<object> ItemsSource
        {
            get {
                return (ReadOnlyObservableCollection<object>)GetValue(ItemsSourceProperty); 
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }
   
        public ConnectionBar()
        {
            this.InitializeComponent();
        }

    }
}
