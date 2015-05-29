namespace RdClient.Controls
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Input.Pointer;
    using RdClient.Shared.Input.Recognizers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Models.Viewport;
    using RdClient.Shared.Navigation;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;
    using Windows.Foundation;
    using Windows.UI.Core;
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
        private TapRecognizer _tapRecognizer;
        private CoreCursor _exitCursor;

        public RemoteSessionPanel()
        {
            this.InitializeComponent();

            this.RenderingPanel.SizeChanged += this.OnRenderingPanelSizeChanged;
            this.SizeChanged += OnSizeChanged;

            _viewLoaded = false;
            _renderingPanelSize = Size.Empty;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            EmitPropertyChanged("Width");
            EmitPropertyChanged("Height");
        }

        public object RemoteSessionViewSite
        {
            get { return GetValue(RemoteSessionViewSiteProperty); }
            set { SetValue(RemoteSessionViewSiteProperty, value); }
        }
        //
        // IRemoteSessionView interface
        //
        public Size RenderingPanelSize
        {
            get { return _renderingPanelSize; }
            private set { this.SetProperty(ref _renderingPanelSize, value); }
        }

        double IViewportPanel.Width
        {
            get
            {
                return this.ActualWidth;
            }
            set
            {
                this.Width = value;
            }
        }

        double IViewportPanel.Height
        {
            get
            {
                return this.ActualHeight;
            }
            set
            {
                this.Height = value;
            }
        }

        public IViewportTransform Transform
        {
            get
            {
                return null;
            }
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
            this.RenderingPanel.MouseScaleTransform = this.MouseScaleTransform;

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
                EmitPropertyChanged(propertyName);
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

            this.RenderingPanel.SetViewport(new Viewport(this.RenderingPanel, this));
            this.RenderingPanel.SetTransform(new ViewportTransformWrapper(this.RenderPanelTransform));

            _renderingPanelSize = this.RenderingPanel.RenderSize;
            //
            // Set self as the remote session view in the view model.
            //
            this.RemoteSessionViewSite.CastAndCall<IRemoteSessionViewSite>(site => site.SetRemoteSessionView(this));

            ITimer timer = null;
            this.RemoteSessionViewSite.CastAndCall<IRemoteSessionViewSite>(site => timer = new RdDispatcherTimer(site.TimerFactory.CreateTimer(), site.DeferredExecution));
            _zoomScrollRecognizer = new ZoomScrollRecognizer(timer);
            _zoomScrollRecognizer.ZoomScrollEvent += OnZoomScrollEvent;

            this.RemoteSessionViewSite.CastAndCall<IRemoteSessionViewSite>(site => timer = new RdDispatcherTimer(site.TimerFactory.CreateTimer(), site.DeferredExecution));
            _tapRecognizer = new TapRecognizer(timer);
            _tapRecognizer.Tapped += OnTapEvent;
        }

        private void OnTapEvent(object sender, ITapEvent e)
        {
            this.RenderingPanel.EmitPointerEvent(e);
        }

        private void OnZoomScrollEvent(object sender, IZoomScrollEvent e)
        {
            this.RenderingPanel.EmitPointerEvent(e);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            IRenderingPanel panel = this.RenderingPanel;
            panel.ChangeMouseVisibility(Visibility.Collapsed);
            
            if (null != _closed)
                _closed(this, EventArgs.Empty);
        }

        private void OnRenderingPanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.RenderingPanelSize = e.NewSize;
        }

        private void EmitPropertyChanged(string propertyName)
        {
            if(_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
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
            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerCancelled, PointerEventType.PointerRoutedEventArgs, e, this));
            _tapRecognizer.Consume(w);

            this.RenderingPanel.EmitPointerEvent(w);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerReleased, PointerEventType.PointerRoutedEventArgs, e, this));
            _tapRecognizer.Consume(w);

            this.RenderingPanel.EmitPointerEvent(w);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerPressed, PointerEventType.PointerRoutedEventArgs, e, this));
            _tapRecognizer.Consume(w);

            this.RenderingPanel.EmitPointerEvent(w);
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
        }


        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerEntered, PointerEventType.PointerRoutedEventArgs, e, this));
            this.RenderingPanel.EmitPointerEvent(w);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerExited, PointerEventType.PointerRoutedEventArgs, e, this));
            this.RenderingPanel.EmitPointerEvent(w);
        }
    }
}
