namespace RdClient.Controls
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Input;
    using RdClient.Shared.Input.Pointer;
    using RdClient.Shared.Models;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Wrapper of SwapChainPanel that adds the IRenderingPanel interface.
    /// </summary>
    public sealed class RenderingPanel : SwapChainPanel, IRenderingPanel, IDisposable
    {
        private readonly ReaderWriterLockSlim _monitor;
        private IViewport _viewport;
        private EventHandler _ready;
        private EventHandler<IPointerEventBase> _pointerChanged;

        public Image MouseCursor { private get; set; }
        private Point _hotspot = new Point(0,0);

        private TranslateTransform _mouseTransform;
        public TranslateTransform MouseTransform
        {
            get { return _mouseTransform; }
            set { _mouseTransform = value; }
        }

        private ScaleTransform _mouseScaleTransform;
        public ScaleTransform MouseScaleTransform
        {
            get { return _mouseScaleTransform; }
            set { _mouseScaleTransform = value; }
        }

        public RenderingPanel()
        {
            _monitor = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

            this.SizeChanged += this.OnSizeChanged;
        }

        ~RenderingPanel()
        {
            Dispose(false);
        }

        public void SetViewport(IViewport viewport)
        {
            Contract.Assert(null != viewport);
            Contract.Assert(null == _viewport);

            _viewport = viewport;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        event EventHandler IRenderingPanel.Ready
        {
            add
            {
                _ready += value;

                if (this.ActualHeight > 0 && this.ActualWidth > 0)
                    value(this, EventArgs.Empty);
            }
            remove { _ready -= value; }
        }

        event EventHandler<IPointerEventBase> IRenderingPanel.PointerChanged
        {
            add
            {
                using (ReadWriteMonitor.UpgradeableRead(_monitor))
                {
                    _pointerChanged += value;
                }
            }

            remove
            {
                using (ReadWriteMonitor.UpgradeableRead(_monitor))
                {
                    _pointerChanged -= value;
                }
            }
        }

        IViewport IRenderingPanel.Viewport
        {
            get
            {
                Contract.Assert(null != _viewport);
                Contract.Ensures(null != Contract.Result<IViewport>());
                return _viewport;
            }
        }

        void IRenderingPanel.ChangeMouseCursorShape(MouseCursorShape shape)
        {
            this.MouseCursor.Source = shape.ImageSource;
            _hotspot = shape.Hotspot;
        }

        void IRenderingPanel.MoveMouseCursor(Point point)
        {
            this.MouseTransform.X = point.X - _hotspot.X * this.MouseScaleTransform.ScaleX;
            this.MouseTransform.Y = point.Y - _hotspot.Y * this.MouseScaleTransform.ScaleY;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((e.PreviousSize.Width == 0 || e.PreviousSize.Height == 0) && e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                if (null != _ready)
                    _ready(this, EventArgs.Empty);
            }
        }

        public void EmitPointerEvent(IPointerEventBase e)
        {
            using (ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _pointerChanged)
                    _pointerChanged(this, e);
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _monitor.Dispose();
            }
        }
    }
}
