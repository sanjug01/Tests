namespace RdClient.Controls
{
    using RdClient.Shared.Converters;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Input.Pointer;
    using RdClient.Shared.Models;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// Wrapper of SwapChainPanel that adds the IRenderingPanel interface.
    /// </summary>
    public sealed class RenderingPanel : SwapChainPanel, IRenderingPanel, IDisposable
    {
        private readonly ReaderWriterLockSlim _monitor;
        private IViewport _viewport;
        private EventHandler _ready;
        private EventHandler<RdClient.Shared.Input.Pointer.PointerEventArgs> _pointerChanged;   

        public Image MouseCursor { private get; set; }
        private Point _hotspot = new Point(0,0);

        public TranslateTransform MouseTransform { private get; set; }

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

        event EventHandler<PointerEventArgs> IRenderingPanel.PointerChanged
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

        public void ChangeMouseCursorShape(MouseCursorShape shape)
        {
            this.MouseCursor.Source = shape.ImageSource;
            _hotspot = shape.Hotspot;
        }

        public void MoveMouseCursor(Point point)
        {
            this.MouseTransform.X = point.X - _hotspot.X;
            this.MouseTransform.Y = point.Y - _hotspot.Y;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((e.PreviousSize.Width == 0 || e.PreviousSize.Height == 0) && e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                if (null != _ready)
                    _ready(this, EventArgs.Empty);
            }
        }

        public void EmitPointerEvent(PointerEvent e)
        {
            using(ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _pointerChanged)
                    _pointerChanged(this, new PointerEventArgs(e));
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
