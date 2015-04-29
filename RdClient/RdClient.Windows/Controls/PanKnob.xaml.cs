using RdClient.Shared.Helpers;
using RdClient.Shared.Navigation;
using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Models.PanKnobModel;
using System.Diagnostics.Contracts;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using System;
using RdClient.Shared.Input.Recognizers;
using System.Diagnostics;

namespace RdClient.Controls
{
    public sealed partial class PanKnob : UserControl, IPanKnob
    {
        public static readonly DependencyProperty PanKnobSiteProperty = DependencyProperty.Register("PanKnobSite",
       typeof(object),
       typeof(PanKnob),
       new PropertyMetadata(null, OnPanKnobSiteChanged));

        private static void OnPanKnobSiteChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Contract.Assert(sender is PanKnob);
            Contract.Assert(null == e.NewValue || e.NewValue is IPanKnobSite);

            ((PanKnob)sender).OnPanKnobSiteChanged((IPanKnobSite)e.OldValue, (IPanKnobSite)e.NewValue);
        }
        private void OnPanKnobSiteChanged(IPanKnobSite oldSite, IPanKnobSite newSite)
        {
            this.PanKnobSite = newSite;
        }

        private IPanKnobSite _panKnobSite;
        public IPanKnobSite PanKnobSite
        {
            get { return _panKnobSite; }
            set
            {
                _panKnobSite = value;
                _panKnobSite.SetPanKnob(this);


                ITimer timer = null;
                _panKnobSite.CastAndCall<IPanKnobSite>(
                    site =>
                    {
                        timer = site.TimerFactory.CreateTimer();
                    });
                _zoomScrollRecognizer = new ZoomScrollRecognizer(timer);
                _zoomScrollRecognizer.ZoomScrollEvent += OnZoomScrollEvent;

                _panKnobSite.CastAndCall<IPanKnobSite>(
                    site =>
                    {
                        timer = site.TimerFactory.CreateTimer();
                    });
                _tapRecognizer = new TapRecognizer(timer);
                _tapRecognizer.TapEvent += OnTapEvent;
            }
        }


        private TapRecognizer _tapRecognizer;
        private ZoomScrollRecognizer _zoomScrollRecognizer;

        bool IPanKnob.IsVisible
        {
            get
            {
                return PanKnobGrid.Visibility == Visibility.Visible;
            }

            set
            {
                if(value == true)
                {
                    PanKnobGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    PanKnobGrid.Visibility = Visibility.Collapsed;
                }
            }
        }

        Point IPanKnob.Position
        {
            get
            {
                return new Point(PanKnobTranslateTransform.X, PanKnobTranslateTransform.Y);
            }
            set
            {
                PanKnobTranslateTransform.X = value.X;
                PanKnobTranslateTransform.Y = value.Y;
            }
        }

        Size IPanKnob.Size
        {
            get
            {
                return this.PanKnobGrid.RenderSize;
            }
        }

        public PanKnob()
        {
            this.InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void OnZoomScrollEvent(object sender, IZoomScrollEvent e)
        {
            _panKnobSite.Consume(e);
        }

        private void OnTapEvent(object sender, ITapEvent e)
        {
            _panKnobSite.Consume(e);
        }

        private void InternalConsume(IPointerEventBase pointer)
        {
            _panKnobSite.Consume(pointer);
        }

        private void OnTapped(object sender, TappedEventArgs e)
        {
            IGestureRoutedEventProperties w = new GestureRoutedEventPropertiesWrapper(new PointerEvent(PointerEventAction.Tapped, PointerEventType.TappedEventArgs, e, this));
            InternalConsume(w);
        }

        protected override void OnManipulationStarting(ManipulationStartingRoutedEventArgs e)
        {
            IManipulationRoutedEventProperties w = new ManipulationRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.ManipulationStarting, PointerEventType.ManipulationStartingRoutedEventArgs, e, this));

            _zoomScrollRecognizer.Consume(w);
            _tapRecognizer.Consume(w);
            InternalConsume(w);
        }

        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            IManipulationRoutedEventProperties w = new ManipulationRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.ManipulationStarted, PointerEventType.ManipulationStartedRoutedEventArgs, e, this));
            _zoomScrollRecognizer.Consume(w);
            _tapRecognizer.Consume(w);
            InternalConsume(w);
        }

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            IManipulationRoutedEventProperties w = new ManipulationRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.ManipulationDelta, PointerEventType.ManipulationDeltaRoutedEventArgs, e, this));
            _zoomScrollRecognizer.Consume(w);
            _tapRecognizer.Consume(w);
            InternalConsume(w);
        }

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingRoutedEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 0.005;
            IManipulationRoutedEventProperties w = new ManipulationRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.ManipulationInertiaStarting, PointerEventType.ManipulationInertiaStartingRoutedEventArgs, e, this));
            _zoomScrollRecognizer.Consume(w);
            _tapRecognizer.Consume(w);
            InternalConsume(w);
        }

        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            IManipulationRoutedEventProperties w = new ManipulationRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.ManipulationCompleted, PointerEventType.ManipulationCompletedRoutedEventArgs, e, this));
            _zoomScrollRecognizer.Consume(w);
            _tapRecognizer.Consume(w);
            InternalConsume(w);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            this.CapturePointer(e.Pointer);

            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerPressed, PointerEventType.PointerRoutedEventArgs, e, this));
            _tapRecognizer.Consume(w);
            InternalConsume(w);
        }

        protected override void OnPointerCanceled(PointerRoutedEventArgs e)
        {
            this.ReleasePointerCapture(e.Pointer);

            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerCancelled, PointerEventType.PointerRoutedEventArgs, e, this));
            _tapRecognizer.Consume(w);
            InternalConsume(w);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            this.ReleasePointerCapture(e.Pointer);            

            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerReleased, PointerEventType.PointerRoutedEventArgs, e, this));
            _tapRecognizer.Consume(w);
            InternalConsume(w);
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerMoved, PointerEventType.PointerRoutedEventArgs, e, this));
            _tapRecognizer.Consume(w);
            InternalConsume(w);
        }
    }
}
