namespace RdClient.Controls
{
    using RdClient.Shared.Converters;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Input.Pointer;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;
    using Windows.Foundation;
    using Windows.UI.Input;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;

    /// <summary>
    /// A panel shown in the remote session view (RemoteSessionView) that renders contents of the remote session
    /// and captures keyboard, mouse, pen and touch input.
    /// </summary>
    public sealed partial class RemoteSessionPanel : UserControl, IRemoteSessionView
    {
        public static readonly DependencyProperty RemoteSessionViewSiteProperty = DependencyProperty.Register("RemoteSessionViewSite",
            typeof(object),
            typeof(RemoteSessionPanel),
            new PropertyMetadata(null, OnRemoteSessionViewSiteChanged));

        private EventHandler _closed;
        private PropertyChangedEventHandler _propertyChanged;
        private bool _viewLoaded;
        private Size _renderingPanelSize;
        private ZoomScrollRecognizer _zoomScrollRecognizer;
        private GestureRecognizer _platformRecognizer;

        public RemoteSessionPanel()
        {
            this.InitializeComponent();

            this.RenderingPanel.SizeChanged += this.OnRenderingPanelSizeChanged;

            _viewLoaded = false;
            _renderingPanelSize = Size.Empty;
            this.RenderingPanel.SetViewport(new RenderingPanelViewport(this, this.RenderingPanel, this.Transformation));


        }

        public object RemoteSessionViewSite
        {
            get { return GetValue(RemoteSessionViewSiteProperty); }
            set { SetValue(RemoteSessionViewSiteProperty, value); }
        }
        //
        // IRemoteSessionView interface
        //
        public Size Size
        {
            get { return _renderingPanelSize; }
            private set { this.SetProperty(ref _renderingPanelSize, value); }
        }

        event EventHandler IRemoteSessionView.Closed
        {
            add { _closed += value; }
            remove { _closed -= value; }
        }

        IRenderingPanel IRemoteSessionView.ActivateNewRenderingPanel()
        {
            Contract.Assert(_viewLoaded);
            Contract.Ensures(null != Contract.Result<IRenderingPanel>());

            this.RenderingPanel.MouseCursor = this.MouseCursor;
            this.RenderingPanel.MouseTransform = this.MouseTransform;

            return this.RenderingPanel;
        }

        void IRemoteSessionView.RecycleRenderingPanel(IRenderingPanel renderingPanel)
        {
            if (null == renderingPanel)
                throw new ArgumentNullException("renderingPanel");

            if (!object.ReferenceEquals(renderingPanel, this.RenderingPanel))
                throw new ArgumentException("Invalid rendering panel passed for recycling", "renderingPanel");

            //
            // TODO: recycle the swap chain panel somehow
            //
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }

        //
        // Internals
        //
        private static void OnRemoteSessionViewSiteChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Contract.Assert(sender is RemoteSessionPanel);
            Contract.Assert(null == e.NewValue || e.NewValue is IRemoteSessionViewSite);

            ((RemoteSessionPanel)sender).OnRemoteSessionViewSiteChanged((IRemoteSessionViewSite)e.OldValue, (IRemoteSessionViewSite)e.NewValue);
        }

        private void SetProperty<TProperty>(ref TProperty property, TProperty newValue, [CallerMemberName] string propertyName = null)
        {
            if (!object.Equals(property, newValue))
            {
                property = newValue;
                if (null != _propertyChanged)
                    _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnRemoteSessionViewSiteChanged(IRemoteSessionViewSite oldSite, IRemoteSessionViewSite newSite)
        {
            if (null != oldSite)
                oldSite.SetRemoteSessionView(null);

            if (null != newSite && _viewLoaded)
                newSite.SetRemoteSessionView(this);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewLoaded = true;
            _renderingPanelSize = this.RenderingPanel.RenderSize;
            //
            // Set self as the remote session view in the view model.
            //
            this.RemoteSessionViewSite.CastAndCall<IRemoteSessionViewSite>(site => site.SetRemoteSessionView(this));

            ITimer timer = null;
            this.RemoteSessionViewSite.CastAndCall<IRemoteSessionViewSite>(site => timer = site.TimerFactory.CreateTimer());
            _zoomScrollRecognizer = new ZoomScrollRecognizer(timer);
            _zoomScrollRecognizer.ZoomScrollEvent += OnZoomScrollEvent;
            _platformRecognizer = new GestureRecognizer();
            _platformRecognizer.GestureSettings = GestureSettings.Tap | GestureSettings.Hold | GestureSettings.Drag;
            _platformRecognizer.Tapped += OnTapped;
            _platformRecognizer.Holding += OnHolding;
        }

        private void OnTapped(object sender, TappedEventArgs e)
        {
            IGestureRoutedEventProperties w = new GestureRoutedEventPropertiesWrapper(new PointerEvent(PointerEventAction.Tapped, PointerEventType.TappedEventArgs, e, this));
            this.RenderingPanel.EmitPointerEvent(w);
        }

        private void OnHolding(object sender, HoldingEventArgs e)
        {
            IGestureRoutedEventProperties w = null;

            switch(e.HoldingState)
            {
                case HoldingState.Started:
                    w = new GestureRoutedEventPropertiesWrapper(new PointerEvent(PointerEventAction.HoldingStarted, PointerEventType.HoldingEventArgs, e, this));
                    break;
                case HoldingState.Completed:
                    w = new GestureRoutedEventPropertiesWrapper(new PointerEvent(PointerEventAction.HoldingCompleted, PointerEventType.HoldingEventArgs, e, this));
                    break;
                case HoldingState.Canceled:
                    w = new GestureRoutedEventPropertiesWrapper(new PointerEvent(PointerEventAction.HoldingCanceled, PointerEventType.HoldingEventArgs, e, this));
                    break;
            }

            this.RenderingPanel.EmitPointerEvent(w);
        }

        private void OnZoomScrollEvent(object sender, ZoomScrollEventArgs e)
        {
            this.RenderingPanel.EmitPointerEvent(e.ZoomScrollEvent);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (null != _closed)
                _closed(this, EventArgs.Empty);
        }

        private void OnRenderingPanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //
            // Set the Size property, which will emit an INotifyPropertyChanged event to the view model.
            //
            this.Size = e.NewSize;
        }

        protected override void OnManipulationStarting(ManipulationStartingRoutedEventArgs e)
        {
            IManipulationRoutedEventProperties w = new ManipulationRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.ManipulationStarting, PointerEventType.ManipulationStartingRoutedEventArgs, e, this));
            _zoomScrollRecognizer.Consume(w);
            this.RenderingPanel.EmitPointerEvent(w);
        }

        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            IManipulationRoutedEventProperties w = new ManipulationRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.ManipulationStarted, PointerEventType.ManipulationStartedRoutedEventArgs, e, this));
            _zoomScrollRecognizer.Consume(w);
            this.RenderingPanel.EmitPointerEvent(w);
        }

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingRoutedEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = GlobalConstants.DesiredDeceleration;

            IManipulationRoutedEventProperties w = new ManipulationRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.ManipulationInertiaStarting, PointerEventType.ManipulationInertiaStartingRoutedEventArgs, e, this));
            _zoomScrollRecognizer.Consume(w);
            this.RenderingPanel.EmitPointerEvent(w);
        }

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            IManipulationRoutedEventProperties w = new ManipulationRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.ManipulationDelta, PointerEventType.ManipulationDeltaRoutedEventArgs, e, this));
            _zoomScrollRecognizer.Consume(w);
            this.RenderingPanel.EmitPointerEvent(w);
        }

        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            IManipulationRoutedEventProperties w = new ManipulationRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.ManipulationCompleted, PointerEventType.ManipulationCompletedRoutedEventArgs, e, this));
            _zoomScrollRecognizer.Consume(w);
            this.RenderingPanel.EmitPointerEvent(w);
        }

        protected override void OnPointerCanceled(PointerRoutedEventArgs e)
        {
            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerCanceled, PointerEventType.PointerRoutedEventArgs, e, this));
            this.RenderingPanel.EmitPointerEvent(w);
            _platformRecognizer.CompleteGesture();
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerReleased, PointerEventType.PointerRoutedEventArgs, e, this));
            this.RenderingPanel.EmitPointerEvent(w);
            _platformRecognizer.ProcessUpEvent(e.GetCurrentPoint(this));
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerPressed, PointerEventType.PointerRoutedEventArgs, e, this));
            this.RenderingPanel.EmitPointerEvent(w);
            _platformRecognizer.ProcessDownEvent(e.GetCurrentPoint(this));
        }

        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerPressed, PointerEventType.PointerRoutedEventArgs, e, this));
            this.RenderingPanel.EmitPointerEvent(w);
        }


        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerMoved, PointerEventType.PointerRoutedEventArgs, e, this));
            this.RenderingPanel.EmitPointerEvent(w);

            _platformRecognizer.ProcessMoveEvents(e.GetIntermediatePoints(this));
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            //_exitCursor = Window.Current.CoreWindow.PointerCursor;
            //Window.Current.CoreWindow.PointerCursor = null;
            //this.MouseCursor.Visibility = Visibility.Visible;
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            //Window.Current.CoreWindow.PointerCursor = _exitCursor;
            //this.MouseCursor.Visibility = Visibility.Collapsed;
        }
    }
}
