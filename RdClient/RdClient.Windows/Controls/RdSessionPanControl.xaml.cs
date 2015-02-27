
namespace RdClient.Controls
{
    using RdClient.CxWrappers.Utils;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Converters;
    using RdClient.Shared.Helpers;    
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
            "PointerEventConsumer", 
            typeof(object),
            typeof(RdSessionPanControl), 
            new PropertyMetadata(null, PointerEventConsumerPropertyChanged));
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
                        panel.MovePanControl();
                        break;
                }
            }
        }

        public RdSessionPanControl()
        {
            this.InitializeComponent();
            this.SizeChanged += (sender, args) => { this.ViewSize = (args as SizeChangedEventArgs).NewSize; };
        }

        public void ShowPanControl()
        {
            // PanControl.Visibility = Visibility.Visible;
            ShowPanControlStoryboard.Begin();
        }

        public void HidePanControl()
        {
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
            PointerEventConsumer.ConsumeEvent(PointerEventConverter.PointerArgsConverter(this, args, TouchEventType.Up));

        }

        protected override void OnPointerPressed(PointerRoutedEventArgs args)
        {
            PointerEventConsumer.ConsumeEvent(PointerEventConverter.PointerArgsConverter(this, args, TouchEventType.Down));           
        }

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingRoutedEventArgs args)
        {
            args.TranslationBehavior.DesiredDeceleration = GlobalConstants.DesiredDeceleration;
            PointerEventConsumer.ConsumeEvent(PointerEventConverter.ManipulationInertiaStartingArgsConverter(args));
        }

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs args)
        {
            PointerEventConsumer.ConsumeEvent(PointerEventConverter.ManipulationDeltaArgsConverter(args));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
