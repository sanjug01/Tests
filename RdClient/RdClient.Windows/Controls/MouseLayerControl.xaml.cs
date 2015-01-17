﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace RdClient.Controls
{
    using RdClient.Shared.Converters;
    using RdClient.Shared.Input.Mouse;
    using Windows.Foundation;
    using Windows.UI.Core;

    public sealed partial class MouseLayerControl : UserControl
    {
        private CoreCursor _exitCursor;

        public MouseLayerControl()
        {
            this.InitializeComponent();                       
        }

        public static readonly DependencyProperty PointerEventConsumerProperty = DependencyProperty.Register(
            "PointerEventConsumer", typeof(PointerEventConsumer),
            typeof(MouseLayerControl), new PropertyMetadata(true, PointerEventConsumerPropertyChanged));
        public PointerEventConsumer PointerEventConsumer
        {
            private get { return (PointerEventConsumer)GetValue(PointerEventConsumerProperty); }
            set { SetValue(PointerEventConsumerProperty, value); }
        }
        private static void PointerEventConsumerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IPointerEventConsumer pec = e.NewValue as IPointerEventConsumer;
        }

        public static readonly DependencyProperty MouseShapeProperty = DependencyProperty.Register(
            "MouseShape", typeof(ImageSource),
            typeof(MouseLayerControl), new PropertyMetadata(true, MouseShapePropertyChanged));
        public ImageSource MouseShape
        {
            private get { return (ImageSource)GetValue(MouseShapeProperty); }
            set { SetValue(MouseShapeProperty, value); }
        }

        private static void MouseShapePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MouseLayerControl mlc = d as MouseLayerControl;
            
            mlc.MouseShapeElement.Source = e.NewValue as ImageSource;
        }

        public static readonly DependencyProperty MousePositionProperty = DependencyProperty.Register(
            "MousePosition", typeof(Point),
            typeof(MouseLayerControl), new PropertyMetadata(true, MousePositionPropertyPropertyChanged));
        public Point MousePosition
        {
            private get { return (Point)GetValue(MousePositionProperty); }
            set { SetValue(MousePositionProperty, value); }
        }

        private static void MousePositionPropertyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MouseLayerControl mlc = d as MouseLayerControl;
            Point position = (Point) e.NewValue;

            mlc.MouseShapeElementTranslateTransform.X = position.X;
            mlc.MouseShapeElementTranslateTransform.Y = position.Y;
        }

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingRoutedEventArgs args)
        {
            args.TranslationBehavior.DesiredDeceleration = 0.002;

            if(args.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
            {
                PointerEventConsumer.ConsumeEvent(PointerEventConverter.ManipulationInertiaStartingArgsConverter(args));
            }
        }

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs args)
        {
            if (args.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
            {
                PointerEventConsumer.ConsumeEvent(PointerEventConverter.ManipulationDeltaArgsConverter(args));
            }
        }

        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs args)
        {
            if (args.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
            {
                PointerEventConsumer.ConsumeEvent(PointerEventConverter.ManipulationCompletedArgsConverter(args));
            }
        }

        protected override void OnPointerCanceled(PointerRoutedEventArgs args)
        {
            PointerEventConsumer.ConsumeEvent(PointerEventConverter.PointerArgsConverter(this, args));
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs args)
        {
            PointerEventConsumer.ConsumeEvent(PointerEventConverter.PointerArgsConverter(this, args));
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs args)
        {
            PointerEventConsumer.ConsumeEvent(PointerEventConverter.PointerArgsConverter(this, args));
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs args)
        {            
            PointerEventConsumer.ConsumeEvent(PointerEventConverter.PointerArgsConverter(this, args));
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
