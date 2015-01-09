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

            _pointerModel = new PointerModel(new WinrtThreadPoolTimer());
            _pointerModel.MousePointerChanged += (s, o) => { var ignore = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { this.MousePointer = o; }); };
            this.SizeChanged += (s, o) => { _pointerModel.WindowSize = o.NewSize; };
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

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingRoutedEventArgs args)
        {
            if(args.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
            {
                _pointerModel.ConsumeEvent(PointerEventConverter.ManipulationInertiaStartingArgsConverter(args));
            }
        }

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs args)
        {
            if (args.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
            {
                _pointerModel.ConsumeEvent(PointerEventConverter.ManipulationDeltaArgsConverter(args));
            }
        }

        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs args)
        {
            if (args.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
            {
                _pointerModel.ConsumeEvent(PointerEventConverter.ManipulationCompletedArgsConverter(args));
            }
        }

        protected override void OnPointerCanceled(PointerRoutedEventArgs args)
        {
            _pointerModel.ConsumeEvent(PointerEventConverter.PointerArgsConverter(this, args));
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs args)
        {
            _pointerModel.ConsumeEvent(PointerEventConverter.PointerArgsConverter(this, args));
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs args)
        {
            _pointerModel.ConsumeEvent(PointerEventConverter.PointerArgsConverter(this, args));
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs args)
        {
            _pointerModel.ConsumeEvent(PointerEventConverter.PointerArgsConverter(this, args));
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs args)
        {
            _exitCursor = Window.Current.CoreWindow.PointerCursor;
            Window.Current.CoreWindow.PointerCursor = null;
        }

        protected override void OnPointerExited(PointerRoutedEventArgs args)
        {
            Window.Current.CoreWindow.PointerCursor = _exitCursor;
        }
    }
}
