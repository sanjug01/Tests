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

    /// <summary>
    /// Wrapper of SwapChainPanel that adds the IRenderingPanel interface.
    /// </summary>
    public sealed class RenderingPanel : SwapChainPanel, IRenderingPanel, IDisposable
    {
        private readonly ReaderWriterLockSlim _monitor;
        private EventHandler _ready;
        private EventHandler<PointerEventArgs> _pointerChanged;
        private CancellationTokenSource _cts;
        private Task _pointerCaptureTask;

        public RenderingPanel()
        {
            _monitor = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            this.SizeChanged += this.OnSizeChanged;
        }

        ~RenderingPanel()
        {
            Dispose(false);
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
                    bool needToStart = null == _pointerChanged;
                    _pointerChanged += value;

                    if(needToStart)
                    {
                        StartPointerCapture();
                    }
                }
            }

            remove
            {
                using (ReadWriteMonitor.UpgradeableRead(_monitor))
                {
                    bool hadHandlers = null != _pointerChanged;

                    _pointerChanged -= value;

                    if(hadHandlers && null == _pointerChanged)
                    {
                        StopPointerCapture();
                    }
                }
            }
        }

        public void ChangeMouseCursorShape(MouseCursorShape shape)
        {
            throw new NotImplementedException();
        }

        public void MoveMouseCursor(Point point)
        {
            throw new NotImplementedException();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((e.PreviousSize.Width == 0 || e.PreviousSize.Height == 0) && e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                if (null != _ready)
                    _ready(this, EventArgs.Empty);
            }
        }

        private void StartPointerCapture()
        {
            Contract.Assert(null == _pointerCaptureTask);
            Contract.Assert(null == _cts);

            _cts = new CancellationTokenSource();

            _pointerCaptureTask = new Task(()=>
            {
                Windows.UI.Core.CoreIndependentInputSource inputSource = this.CreateCoreIndependentInputSource(
                    Windows.UI.Core.CoreInputDeviceTypes.Mouse
                    | Windows.UI.Core.CoreInputDeviceTypes.Pen
                    | Windows.UI.Core.CoreInputDeviceTypes.Touch);

                _cts.Token.Register(() => inputSource.Dispatcher.StopProcessEvents());

                inputSource.PointerMoved += this.OnPointerMoved;
                inputSource.PointerPressed += this.OnPointerPressed;
                inputSource.PointerReleased += this.OnPointerReleased;
                inputSource.PointerEntered += this.OnPointerEntered;
                inputSource.PointerExited += this.OnPointerExited;
                inputSource.PointerWheelChanged += this.OnPointerWheelChanged;

                if (!_cts.Token.IsCancellationRequested)
                {
                    inputSource.Dispatcher.ProcessEvents(Windows.UI.Core.CoreProcessEventsOption.ProcessUntilQuit);
                }

                inputSource.PointerMoved -= this.OnPointerMoved;
                inputSource.PointerPressed -= this.OnPointerPressed;
                inputSource.PointerReleased -= this.OnPointerReleased;
                inputSource.PointerEntered -= this.OnPointerEntered;
                inputSource.PointerExited -= this.OnPointerExited;
                inputSource.PointerWheelChanged -= this.OnPointerWheelChanged;

            }, _cts.Token, TaskCreationOptions.LongRunning);

            _pointerCaptureTask.Start();
        }

        private void StopPointerCapture()
        {
            Contract.Assert(null != _cts);
            Contract.Assert(null != _pointerCaptureTask);

            _cts.Cancel();
            _pointerCaptureTask.Wait();
            _pointerCaptureTask = null;
            _cts.Dispose();
            _cts = null;
        }

        private void EmitPointerEvent(PointerEvent e)
        {
            using(ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _pointerChanged)
                    _pointerChanged(this, new PointerEventArgs(e));
            }
        }

        private void OnPointerMoved(object sender, Windows.UI.Core.PointerEventArgs e)
        {
            EmitPointerEvent(PointerEventConverter.PointerArgsConverter(e, Shared.CxWrappers.TouchEventType.Update));
        }

        private void OnPointerPressed(object sender, Windows.UI.Core.PointerEventArgs e)
        {
            EmitPointerEvent(PointerEventConverter.PointerArgsConverter(e, Shared.CxWrappers.TouchEventType.Down));
        }

        private void OnPointerReleased(object sender, Windows.UI.Core.PointerEventArgs e)
        {
            EmitPointerEvent(PointerEventConverter.PointerArgsConverter(e, Shared.CxWrappers.TouchEventType.Up));
        }

        private void OnPointerEntered(object sender, Windows.UI.Core.PointerEventArgs e)
        {
            // ??? EmitPointerEvent(PointerEventConverter.PointerArgsConverter(e, Shared.CxWrappers.TouchEventType.Update));
        }

        private void OnPointerExited(object sender, Windows.UI.Core.PointerEventArgs e)
        {
            // ??? EmitPointerEvent(PointerEventConverter.PointerArgsConverter(e, Shared.CxWrappers.TouchEventType.Update));
        }

        private void OnPointerWheelChanged(object sender, Windows.UI.Core.PointerEventArgs e)
        {
            EmitPointerEvent(PointerEventConverter.PointerArgsConverter(e, Shared.CxWrappers.TouchEventType.Update));
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _monitor.Dispose();

                if (null != _cts)
                    _cts.Dispose();
            }
        }
    }
}
