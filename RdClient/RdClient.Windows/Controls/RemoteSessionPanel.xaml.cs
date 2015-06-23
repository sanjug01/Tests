namespace RdClient.Controls
{
    using RdClient.Helpers;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Input.Pointer;
    using RdClient.Shared.Input.Recognizers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Models.Viewport;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;



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
        private TapRecognizer _tapRecognizer;

        public event EventHandler<IPointerEventBase> PointerChanged;    

        public RemoteSessionPanel()
        {
            this.InitializeComponent();

            this.SizeChanged += OnSizeChanged;

            _viewLoaded = false;
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


            SessionSizeViewModel ssvm = (SessionSizeViewModel)this.Resources["SessionSizeViewModel"];
            Size size = ssvm.Size;

            this.RenderingPanel.MouseCursor = this.MouseCursor;
            this.RenderingPanel.MouseTransform = this.MouseTransform;
            this.RenderingPanel.MouseScaleTransform = this.MouseScaleTransform;

            this.RenderingPanel.SetViewport(new Viewport(this.RenderingPanel, this));
            this.RenderingPanel.SetTransform(new ViewportTransformWrapper(this.RenderPanelTransform));

            this.RenderingPanel.Width = size.Width;
            this.RenderingPanel.Height = size.Height;

            this.RenderPanelTransform.ScaleX = 1.0;
            this.RenderPanelTransform.ScaleY = 1.0;
            this.RenderPanelTransform.TranslateX = 0.0;
            this.RenderPanelTransform.TranslateY = 0.0;

            this.UpdateLayout();

            return this.RenderingPanel;
        }

        void IRemoteSessionView.RecycleRenderingPanel(IRenderingPanel renderingPanel)
        {
            if (null == renderingPanel)
                throw new ArgumentNullException("renderingPanel");

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

            //
            // Set self as the remote session view in the view model.
            //
            this.RemoteSessionViewSite.CastAndCall<IRemoteSessionViewSite>(site => site.SetRemoteSessionView(this));

            ITimer timer = null;
            this.RemoteSessionViewSite.CastAndCall<IRemoteSessionViewSite>(site => timer = new RdDispatcherTimer(site.TimerFactory.CreateTimer(), site.DeferredExecution));

            this.RemoteSessionViewSite.CastAndCall<IRemoteSessionViewSite>(site => timer = new RdDispatcherTimer(site.TimerFactory.CreateTimer(), site.DeferredExecution));
            _tapRecognizer = new TapRecognizer(timer);
            _tapRecognizer.Tapped += OnTapEvent;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (null != _closed)
                _closed(this, EventArgs.Empty);
        }

        private void EmitPointerEvent(IPointerEventBase e)
        {
            if(PointerChanged != null)
            {
                PointerChanged(this, e);
            }
        }

        private void OnTapEvent(object sender, ITapEvent e)
        {
            this.EmitPointerEvent(e);
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
            this.EmitPointerEvent(w);
            e.Handled = true;
        }

        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            IManipulationRoutedEventProperties w = new ManipulationRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.ManipulationStarted, PointerEventType.ManipulationStartedRoutedEventArgs, e, this));
            this.EmitPointerEvent(w);
            e.Handled = true;
        }

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingRoutedEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = GlobalConstants.DesiredDeceleration;

            IManipulationRoutedEventProperties w = new ManipulationRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.ManipulationInertiaStarting, PointerEventType.ManipulationInertiaStartingRoutedEventArgs, e, this));
            this.EmitPointerEvent(w);
            e.Handled = true;
        }

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            IManipulationRoutedEventProperties w = new ManipulationRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.ManipulationDelta, PointerEventType.ManipulationDeltaRoutedEventArgs, e, this));
            this.EmitPointerEvent(w);
            e.Handled = true;
        }

        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            IManipulationRoutedEventProperties w = new ManipulationRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.ManipulationCompleted, PointerEventType.ManipulationCompletedRoutedEventArgs, e, this));
            this.EmitPointerEvent(w);
            e.Handled = true;
        }

        protected override void OnPointerCanceled(PointerRoutedEventArgs e)
        {
            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerCancelled, PointerEventType.PointerRoutedEventArgs, e, this));
            _tapRecognizer.Consume(w);

            this.EmitPointerEvent(w);
            e.Handled = true;
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerReleased, PointerEventType.PointerRoutedEventArgs, e, this));
            _tapRecognizer.Consume(w);

            this.EmitPointerEvent(w);
            e.Handled = true;
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerPressed, PointerEventType.PointerRoutedEventArgs, e, this));
            _tapRecognizer.Consume(w);

            this.EmitPointerEvent(w);
            e.Handled = true;
        }

        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerPressed, PointerEventType.PointerRoutedEventArgs, e, this));
            this.EmitPointerEvent(w);
            e.Handled = true;
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerMoved, PointerEventType.PointerRoutedEventArgs, e, this));
            this.EmitPointerEvent(w);
            e.Handled = true;
        }


        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerEntered, PointerEventType.PointerRoutedEventArgs, e, this));
            this.EmitPointerEvent(w);
            e.Handled = true;
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            IPointerEventBase w = new PointerRoutedEventArgsWrapper(new PointerEvent(PointerEventAction.PointerExited, PointerEventType.PointerRoutedEventArgs, e, this));
            this.EmitPointerEvent(w);
            e.Handled = true;
        }
    }
}
