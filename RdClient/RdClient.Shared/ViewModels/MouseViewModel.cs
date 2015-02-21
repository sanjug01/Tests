using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Navigation.Extensions;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Input.Mouse;
    using RdClient.Shared.Input.ZoomPan;
    using System.Windows.Input;
    using Windows.Foundation;
    using Windows.UI.Xaml;

    public class MouseViewModel : MutableObject, IPointerManipulator
    {
        private PointerEventDispatcher _pointerEventConsumer;
        public PointerEventDispatcher PointerEventConsumer
        {
            get { return _pointerEventConsumer; }
            set { SetProperty(ref _pointerEventConsumer, value); }
        }

        private ImageSource _mouseShape;
        public ImageSource MouseShape
        {
            get { return _mouseShape; }
            set
            {
                SetProperty(ref _mouseShape, value);
            }
        }

        private Point _mousePosition = new Point(0.0, 0.0);
        public Point MousePosition
        {
            get { return new Point(_mousePosition.X, _mousePosition.Y); }
            set
            {
                this.DeferredExecution.DeferToUI(() =>
                {
                    Point p = new Point(
                        Math.Max(0.0, Math.Min(value.X, this.ViewSize.Width)),
                        Math.Max(0.0, Math.Min(value.Y, this.ViewSize.Height))
                    );

                    SetProperty(ref _mousePosition, p);
                });
            }
        }

        private double _mouseAcceleration = 1.4;
        public double MouseAcceleration
        {
            get
            {
                return _mouseAcceleration;
            }
            set
            {
                _mouseAcceleration = value;
            }
        }

        private Size _viewSize = new Size(0.0, 0.0);
        public Size ViewSize
        {
            get { return _viewSize; }
            set { SetProperty(ref _viewSize, value); }
        }

        private Point _hotSpot = new Point(0.0, 0.0);
        public Point HotSpot
        {
            get { return _hotSpot; }
            set { SetProperty(ref _hotSpot, value); }
        }

        private bool _multiTouchEnabled;
        private ICommand _toggleInputModeCommand;
        public ICommand ToggleInputModeCommand
        {
            get { return _toggleInputModeCommand; }
            set { SetProperty(ref _toggleInputModeCommand, value); }
        }

        public IElephantEarsViewModel ElephantEarsViewModel { private get; set; }

        private IRdpConnection _rdpConnection;
        public IRdpConnection RdpConnection
        {
            set
            {
                if (_rdpConnection != null)
                {
                    _rdpConnection.Events.MouseCursorShapeChanged -= OnMouseCursorShapeChanged;
                    _rdpConnection.Events.MouseCursorPositionChanged -= OnMouseCursorPositionChanged;
                    _rdpConnection.Events.MultiTouchEnabledChanged -= OnMultiTouchEnabledChanged;
                }

                _rdpConnection = value;
                _rdpConnection.Events.MouseCursorShapeChanged += OnMouseCursorShapeChanged;
                _rdpConnection.Events.MouseCursorPositionChanged += OnMouseCursorPositionChanged;
                _rdpConnection.Events.MultiTouchEnabledChanged += OnMultiTouchEnabledChanged;
            }
        }

        public IExecutionDeferrer DeferredExecution { private get; set; }

        public event EventHandler<PanEventArgs> PanChange;
        public event EventHandler<ZoomEventArgs> ScaleChange;

        public MouseViewModel()
        {
            this.PointerEventConsumer = new PointerEventDispatcher(new WinrtThreadPoolTimer(), this);
            this.PointerEventConsumer.ConsumptionMode = ConsumptionMode.Pointer;
            this.PointerEventConsumer.ConsumedEvent += (s, o) => 
            {
                PointerEvent pE = (o as PointerEvent);
                if(o.LeftButton || o.RightButton)
                {
                    this.ElephantEarsViewModel.ElephantEarsVisible = Visibility.Collapsed; 
                }
            };
            this.ToggleInputModeCommand = new RelayCommand(OnToggleInputModeCommand);
            _multiTouchEnabled = true;
        }

        private void OnToggleInputModeCommand(object args)
        {
            if (this.PointerEventConsumer.ConsumptionMode == ConsumptionMode.Pointer)
            {
                if(_multiTouchEnabled)
                {
                    this.PointerEventConsumer.ConsumptionMode = ConsumptionMode.MultiTouch;
                    Debug.WriteLine(ConsumptionMode.MultiTouch);
                }
                else
                {
                    this.PointerEventConsumer.ConsumptionMode = ConsumptionMode.DirectTouch;
                    Debug.WriteLine(ConsumptionMode.DirectTouch);
                }
            }
            else 
            {
                this.PointerEventConsumer.ConsumptionMode = ConsumptionMode.Pointer;
                Debug.WriteLine(ConsumptionMode.Pointer);
            }

            this.ElephantEarsViewModel.ElephantEarsVisible= Visibility.Collapsed;
        }

        private void OnMultiTouchEnabledChanged(object sender, MultiTouchEnabledChangedArgs args)
        {
            _multiTouchEnabled = args.MultiTouchEnabled;            
        }

        private void OnMouseCursorShapeChanged(object sender, MouseCursorShapeChangedArgs args)
        {
            this.DeferredExecution.DeferToUI(() =>
            {
                WriteableBitmap spBitmap = null;

                if (null != args.Buffer)
                {
                    try
                    {
                        spBitmap = new WriteableBitmap(args.Width, args.Height);
                    }
                    catch (OutOfMemoryException e)
                    {
                        RdTrace.TraceErr(String.Format("Failed to allocate memory for WriteableBitmap! Exception: {0}", e.Message));
                        return;
                    }

                    //
                    // Write pixel buffer to bitmap stream.
                    //
                    using (System.IO.Stream stream = spBitmap.PixelBuffer.AsStream())
                    {
                        byte alpha;

                        stream.Position = 0;

                        //
                        // The format used by the WriteableBitmap is ARGB32 (premultiplied RGB).
                        // So the pixels in the Pixel array of a WriteableBitmap are stored as
                        // pre-multiplied colors. Each color channel is pre-multiplied by the alpha value.
                        //
                        for (int i = 0; i < args.Buffer.Length; i += 4)
                        {
                            alpha = args.Buffer[i];

                            //
                            // Copy the ARGB color in reverse order, using WriteByte.
                            // WriteByte writes a byte to the current position in the stream and advances
                            // the position within the stream by one byte.
                            //
                            stream.WriteByte((byte)((args.Buffer[i + 3] * alpha) / 255));
                            stream.WriteByte((byte)((args.Buffer[i + 2] * alpha) / 255));
                            stream.WriteByte((byte)((args.Buffer[i + 1] * alpha) / 255));
                            stream.WriteByte(alpha);
                        };
                    }

                    this.MouseShape = spBitmap;
                    this.HotSpot = new Point(args.XHotspot, args.YHotspot);
                }
            });

        }

        private void OnMouseCursorPositionChanged(object sender, MouseCursorPositionChangedArgs args)
        {
            
        }

        public void SendMouseAction(MouseEventType eventType)
        {
            if (_rdpConnection != null)
            {
                _rdpConnection.SendMouseEvent(eventType, (float)this.MousePosition.X, (float)this.MousePosition.Y);
            }
        }

        public void SendTouchAction(TouchEventType type, uint contactId, Point position, ulong frameTime)
        {
            if(_rdpConnection != null)
            {
                _rdpConnection.SendTouchEvent(type, contactId, position, frameTime);
            }
        }

        public void SendMouseWheel(int delta, bool isHorizontal)
        {
            float x = 0.0f;
            float y = 0.0f;
            MouseEventType type;

            if(isHorizontal)
            {
                x = delta;
                type = MouseEventType.MouseHWheel;
            }
            else
            {
                y = delta;
                type = MouseEventType.MouseWheel;
            }

            if(_rdpConnection != null)
            {
                _rdpConnection.SendMouseEvent(type, x, y);
            }
        }

        public void SendPinchAndZoom(double centerX, double centerY, double fromLength, double toLength)
        {
            ScaleChange.Invoke(this, new ZoomEventArgs(centerX, centerY, fromLength, toLength));
        }

        public void SendPanAction(double deltaX, double deltaY)
        {
            PanChange.Invoke(this, new PanEventArgs(deltaX, deltaY));
        }
    }
}
