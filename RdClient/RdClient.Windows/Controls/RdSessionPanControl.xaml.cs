
namespace RdClient.Controls
{
    using RdClient.CxWrappers.Utils;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Converters;
    using RdClient.Shared.Models;
    using RdClient.Shared.Input.Mouse;
    using RdClient.Shared.Input.ZoomPan;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Core;
    using Windows.UI.Xaml.Input;


    public sealed partial class RdSessionPanControl : UserControl
    {
        public static readonly DependencyProperty ViewSizeProperty = DependencyProperty.Register(
            "ViewSize", typeof(Size),
            typeof(RdSessionPanControl), 
            new PropertyMetadata(true));
        public Size ViewSize
        {
            private get { return (Size)GetValue(ViewSizeProperty); }
            set { SetValue(ViewSizeProperty, value); }
        }

        public static readonly DependencyProperty PointerEventConsumerProperty = DependencyProperty.Register(
            "PointerEventConsumer", typeof(IPointerEventConsumer),
            typeof(MouseLayerControl), new PropertyMetadata(true, PointerEventConsumerPropertyChanged));
        public IPointerEventConsumer PointerEventConsumer
        {
            private get { return (IPointerEventConsumer)GetValue(PointerEventConsumerProperty); }
            set { SetValue(PointerEventConsumerProperty, value); }
        }
        private static void PointerEventConsumerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IPointerEventConsumer pec = e.NewValue as IPointerEventConsumer;
        }

        static readonly DependencyProperty PanKnobTransformProperty = DependencyProperty.Register(
           "PanKnobTransform",
            typeof(object),
            typeof(RdSessionPanControl),
            new PropertyMetadata(null, OnPanKnobChanged)
            );

        public IPanKnobTransform PanKnobTransform
        {
            get { return (IPanKnobTransform)GetValue(PanKnobTransformProperty); }
            set { SetValue(PanKnobTransformProperty, value); }
        }

        private static void OnPanKnobChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Contract.Assert(sender != null);
            Contract.Assert(e != null);
            RdSessionPanControl panel = sender as RdSessionPanControl;

            IPanKnobTransform transform =  e.NewValue as IPanKnobTransform;
            if(null != transform)
            {
                switch(transform.TransformType)
                {
                    case PanKnobTransformType.ShowKnob:
                        panel.ShowPanControl();
                        break;
                    case PanKnobTransformType.HideKnob:
                        panel.HidePanControl();
                        break;
                    case PanKnobTransformType.MoveKnob:
                        break;
                }

            }
        }

        public static DependencyProperty PanControlForegroundBrushProperty = DependencyProperty.Register(
            "PanControlForegroundBrush", typeof(SolidColorBrush),
            typeof(RdSessionPanControl), 
            new PropertyMetadata((SolidColorBrush)Application.Current.Resources["rdBlackBrush"])
            );

        public SolidColorBrush PanControlForegroundBrush
        {
            get { return (SolidColorBrush)GetValue(PanControlForegroundBrushProperty); }
            set { SetValue(PanControlForegroundBrushProperty, value); }
        }

        public static DependencyProperty PanControlBackgroundBrushProperty = DependencyProperty.Register(
            "PanControlBackgroundBrush", typeof(SolidColorBrush),
            typeof(RdSessionPanControl), 
            new PropertyMetadata((SolidColorBrush)Application.Current.Resources["rdWhiteBrush"])
            );

        public SolidColorBrush PanControlBackgroundBrush
        {
            get { return (SolidColorBrush)GetValue(PanControlBackgroundBrushProperty); }
            set { SetValue(PanControlBackgroundBrushProperty, value); }
        }

        public static DependencyProperty PanControlOrbOpacityProperty = DependencyProperty.Register(
            "PanControlOrbOpacity", typeof(double),
            typeof(RdSessionPanControl), 
            new PropertyMetadata(0.35)
            );

        public double PanControlOrbOpacity
        {
            get { return (double)GetValue(PanControlOrbOpacityProperty); }
            set { SetValue(PanControlOrbOpacityProperty, value); }
        }

        public RdSessionPanControl()
        {
            this.InitializeComponent();
            this.SizeChanged += (sender, args) => { this.ViewSize = (args as SizeChangedEventArgs).NewSize; };

            PanControlForegroundBrush = (SolidColorBrush)Application.Current.Resources["rdWhiteBrush"];
            PanControlBackgroundBrush = (SolidColorBrush)Application.Current.Resources["rdBlackBrush"];
            PanControlOrbOpacity = 1.0;

            // PanControl.PointerPressed += PanControl_PointerPressed;
        }

        public void ShowPanControl()
        {
            PanControl.Visibility = Visibility.Visible;
            ShowPanControlAnimation.From = PanControl.Opacity;
            ShowPanControlStoryboard.Begin();
        }

        public void HidePanControl()
        {
            HidePanControlAnimation.From = PanControl.Opacity;
            HidePanControlStoryboard.Begin();
        }

        public void MovePanControl()
        {
            // TODO: may need to consider if the keyboard is on/off
            PositionPanControlStoryboard.Begin();
        }


        protected override void OnPointerCanceled(PointerRoutedEventArgs args)
        {
            PointerEventConsumer.ConsumeEvent(PointerEventConverter.PointerArgsConverter(this, args, TouchEventType.Up));

        }

        protected override void OnPointerReleased(PointerRoutedEventArgs args)
        {
            //PointerEventConsumer.ConsumeEvent(PointerEventConverter.PointerArgsConverter(this, args, TouchEventType.Up));

            //// reset colors
            PanControlForegroundBrush = (SolidColorBrush)Application.Current.Resources["rdWhiteBrush"];
            PanControlBackgroundBrush = (SolidColorBrush)Application.Current.Resources["rdBlackBrush"];            
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs args)
        {
            //PointerEventConsumer.ConsumeEvent(PointerEventConverter.PointerArgsConverter(this, args, TouchEventType.Down));
            //// reverse colors 
            PanControlForegroundBrush = (SolidColorBrush)Application.Current.Resources["rdBlackBrush"];
            PanControlBackgroundBrush = (SolidColorBrush)Application.Current.Resources["rdWhiteBrush"];
            PanControlOrb.Opacity = 1.0;

            // TODO : manage PanControl
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
