// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Controls
{
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

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
    }
}
