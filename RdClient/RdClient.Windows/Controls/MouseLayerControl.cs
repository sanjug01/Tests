using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace RdClient.Controls
{
    using Windows.UI.Input;
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

    public class MouseLayerControl : Control
    {
        private PointerPoint _lastPointerPoint;

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

        public MouseLayerControl()
        {
            this.DefaultStyleKey = typeof(MouseLayerControl);

            this.PointerCanceled += PointerCanceledHandler;
            this.PointerReleased += PointerReleasedHandler;
            this.PointerPressed += PointerPressedHandler;
            this.PointerMoved += PointerMovedHandler;
        }

        void PointerCanceledHandler(object sender, PointerRoutedEventArgs args)
        {

        }

        void PointerReleasedHandler(object sender, PointerRoutedEventArgs args)
        {
            PointerPoint point = args.GetCurrentPoint(this);
            float x = (float)point.Position.X;
            float y = (float)point.Position.Y;

            if (_lastPointerPoint.Properties.IsLeftButtonPressed == true && point.Properties.IsLeftButtonPressed == false)
            {
                this.MousePointer = new MousePointer(1, x, y);
            }
            else if (_lastPointerPoint.Properties.IsRightButtonPressed == true && point.Properties.IsRightButtonPressed == false)
            {
                this.MousePointer = new MousePointer(6, x, y);
            }

            _lastPointerPoint = point;
        }

        void PointerPressedHandler(object sender, PointerRoutedEventArgs args)
        {
            PointerPoint point = args.GetCurrentPoint(this);
            float x = (float) point.Position.X;
            float y = (float) point.Position.Y;

            if(point.Properties.IsLeftButtonPressed)
            {
                this.MousePointer = new MousePointer(0, x, y);
            }
            else if(point.Properties.IsRightButtonPressed)
            {
                this.MousePointer = new MousePointer(5, x, y);
            }

            _lastPointerPoint = point;
        }

        void PointerMovedHandler(object sender, PointerRoutedEventArgs args)
        {
            PointerPoint point = args.GetCurrentPoint(this);
            float x = (float)point.Position.X;
            float y = (float)point.Position.Y;

            this.MousePointer = new MousePointer(4, x, y);
            
            _lastPointerPoint = point;
        }
    }
}
