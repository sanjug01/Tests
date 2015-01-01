using System;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Controls
{
    using RdClient.Shared.Converters;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System.Diagnostics;
    using Windows.UI.Core;
    // int mouseState, float x, float y;
    // mouseState is:
    // 0: LeftPress
    // 1: LeftRelease
    // 2: MouseHWheel
    // 3: Mousewheel
    // 4: Move
    // 5: RightPress
    // 6: RightRelease
    using MousePointer = Tuple<int, float, float>;
    using Position = Tuple<float, float>;

    public sealed partial class MouseLayerControl : UserControl
    {
        private CoreCursor _exitCursor;
        private PointerModel _pointerModel;

        public MouseLayerControl()
        {
            this.InitializeComponent();                       

            this.AddHandler(PointerCanceledEvent, new PointerEventHandler(PointerCanceledHandler), true);
            this.AddHandler(PointerReleasedEvent, new PointerEventHandler(PointerReleasedHandler), true);
            this.AddHandler(PointerPressedEvent, new PointerEventHandler(PointerPressedHandler), true);
            this.AddHandler(PointerMovedEvent, new PointerEventHandler(PointerMovedHandler), true);

            this.ManipulationStarted += ManipulationStartedHandler;
            this.ManipulationInertiaStarting += ManipulationInertiaStartingHandler;
            this.ManipulationDelta += ManipulationDeltaHandler;
            this.ManipulationCompleted += ManipulationCompletedHandler;
            
            this.PointerEntered += OnPointerEntered;
            this.PointerExited += OnPointerExited;

            _pointerModel = new PointerModel(new WinrtThreadPoolTimer());
            _pointerModel.MousePointerChanged += (s, o) => { this.MousePointer = o; };
            this.SizeChanged += (s, o) => { _pointerModel.WindowSize = o.NewSize; };
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs args)
        {
            _exitCursor = Window.Current.CoreWindow.PointerCursor;
            Window.Current.CoreWindow.PointerCursor = null;
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs args)
        {
            Window.Current.CoreWindow.PointerCursor = _exitCursor;
        }

        public static readonly DependencyProperty MousePointerProperty = DependencyProperty.Register(
            "MousePointer", typeof(MousePointer),
            typeof(MouseLayerControl), new PropertyMetadata(true, MousePointerPropertyChanged));

        public MousePointer MousePointer
        {
            get { return (MousePointer)GetValue(MousePointerProperty); }
            set { SetValue(MousePointerProperty, value); }
        }

        private static void MousePointerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public static readonly DependencyProperty MousePointerShapeProperty = DependencyProperty.Register(
            "MousePointerShape", typeof(ImageSource),
            typeof(MouseLayerControl), new PropertyMetadata(true, MousePointerShapePropertyChanged));

        public ImageSource MousePointerShape
        {
            get { return (ImageSource)GetValue(MousePointerProperty); }
            set { SetValue(MousePointerProperty, value); }
        }
        private static void MousePointerShapePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MouseLayerControl mlc = d as MouseLayerControl;
            
            mlc.MousePointerShapeElement.Source = e.NewValue as ImageSource;
        }

        public static readonly DependencyProperty MousePointerShapePositionProperty = DependencyProperty.Register(
            "MousePointerShapePosition", typeof(Position),
            typeof(MouseLayerControl), new PropertyMetadata(true, MousePointerShapePositionPropertyChanged));

        public Position MousePointerShapePosition
        {
            get { return (Position)GetValue(MousePointerShapePositionProperty); }
            set { SetValue(MousePointerProperty, value); }
        }
        private static void MousePointerShapePositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MouseLayerControl mlc = d as MouseLayerControl;
            Position position = e.NewValue as Position;

            mlc.MousePointerShapeElementTranslateTransform.X = position.Item1;
            mlc.MousePointerShapeElementTranslateTransform.Y = position.Item2;
        }

        void MousePointerShapeChangedHandler()
        {
            
        }

        void ManipulationStartedHandler(object sender, ManipulationStartedRoutedEventArgs args)
        {
            //_pointerModel.ConsumeEvent(PointerEventConverter.ManipulationStartedArgsConverter(args));
        }

        void ManipulationInertiaStartingHandler(object sender, ManipulationInertiaStartingRoutedEventArgs args)
        {
            _pointerModel.ConsumeEvent(PointerEventConverter.ManipulationInertiaStartingArgsConverter(args));
        }

        void ManipulationDeltaHandler(object sender, ManipulationDeltaRoutedEventArgs args)
        {
            _pointerModel.ConsumeEvent(PointerEventConverter.ManipulationDeltaArgsConverter(args));
        }

        void ManipulationCompletedHandler(object sender, ManipulationCompletedRoutedEventArgs args)
        {
            _pointerModel.ConsumeEvent(PointerEventConverter.ManipulationCompletedArgsConverter(args));
        }

        void PointerCanceledHandler(object sender, PointerRoutedEventArgs args)
        {
            _pointerModel.ConsumeEvent(PointerEventConverter.PointerArgsConverter(this, args));
        }

        void PointerReleasedHandler(object sender, PointerRoutedEventArgs args)
        {
            _pointerModel.ConsumeEvent(PointerEventConverter.PointerArgsConverter(this, args));
        }

        void PointerPressedHandler(object sender, PointerRoutedEventArgs args)
        {
            _pointerModel.ConsumeEvent(PointerEventConverter.PointerArgsConverter(this, args));
        }

        void PointerMovedHandler(object sender, PointerRoutedEventArgs args)
        {
            _pointerModel.ConsumeEvent(PointerEventConverter.PointerArgsConverter(this, args));
        }
    }
}
