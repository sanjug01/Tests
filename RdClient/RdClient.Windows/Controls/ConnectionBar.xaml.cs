using RdClient.Shared.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using System.Diagnostics.Contracts;
using System.Collections.ObjectModel;
using System.Diagnostics;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Controls
{
    public sealed partial class ConnectionBar : UserControl
    {

        private const double _deceleration = 0.05;
        private Pointer _pointer;

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _pointer = e.Pointer;
        }

        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            ((ButtonBase)e.OriginalSource).ReleasePointerCaptures();
            this.CapturePointer(_pointer);
        }

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            ViewModel.MoveConnectionBar(e.Delta.Translation.X, this.ItemsControl.ActualWidth, ((FrameworkElement)this.Parent).ActualWidth);
        }

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingRoutedEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = _deceleration;
        }

        private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Contract.Assert(sender is ConnectionBar);
            Contract.Assert(null == e.NewValue || e.NewValue is ReadOnlyObservableCollection<object>);

            ((ConnectionBar)sender).OnItemsSourceChanged((ReadOnlyObservableCollection<object>)e.OldValue, (ReadOnlyObservableCollection<object>)e.NewValue);
        }
        private void OnItemsSourceChanged(ReadOnlyObservableCollection<object> oldSource, ReadOnlyObservableCollection<object> newSource)
        {
            ViewModel.ConnectionBarItems = newSource;
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
        public ConnectionBarViewModel ViewModel
        {
            get
            {
                return this.Root.DataContext as ConnectionBarViewModel;
            }
        }

        public ConnectionBar()
        {
            this.InitializeComponent();
            this.AddHandler(PointerPressedEvent, new PointerEventHandler(this.OnPointerPressed), true);
        }

    }
}
