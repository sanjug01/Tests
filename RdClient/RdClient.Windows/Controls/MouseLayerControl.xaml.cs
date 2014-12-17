using System;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Controls
{
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
        private PointerPoint _lastPointerPoint;

        public MouseLayerControl()
        {
            this.InitializeComponent();                       

            this.AddHandler(PointerCanceledEvent, new PointerEventHandler(PointerCanceledHandler), true);
            this.AddHandler(PointerReleasedEvent, new PointerEventHandler(PointerReleasedHandler), true);
            this.AddHandler(PointerPressedEvent, new PointerEventHandler(PointerPressedHandler), true);
            this.AddHandler(PointerMovedEvent, new PointerEventHandler(PointerMovedHandler), true);

            this.PointerEntered += OnPointerEntered;
            this.PointerExited += OnPointerExited;
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
