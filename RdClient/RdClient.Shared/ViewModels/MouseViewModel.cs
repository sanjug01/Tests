using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Navigation.Extensions;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace RdClient.Shared.ViewModels
{

    using RdClient.Shared.Input.Mouse;
    using Windows.Foundation;

    public class MouseViewModel : MutableObject, IPointerManipulator
    {
        private PointerEventConsumer _pointerEventConsumer;
        public PointerEventConsumer PointerEventConsumer
        {
            get { return _pointerEventConsumer; }
            set { SetProperty(ref _pointerEventConsumer, value); }
        }

        private ImageSource _mouseShape;
        public ImageSource MouseShape 
        { 
            get { return _mouseShape; } 
            set { SetProperty(ref _mouseShape, value); } 
        }

        private Point _mousePosition = new Point(0.0, 0.0);
        public Point MousePosition 
        {
            get { return new Point(_mousePosition.X, _mousePosition.Y); }
            set {
                Point p = new Point(
                    Math.Max(0.0, Math.Min(value.X, this.ViewSize.Width)),
                    Math.Max(0.0, Math.Min(value.Y, this.ViewSize.Height))
                );

                SetProperty(ref _mousePosition, p); 
            } 
        }

        private Size _viewSize = new Size(0.0,0.0);
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

        private IRdpConnection _rdpConnection;
        public IRdpConnection RdpConnection { 
            set { 
                if(_rdpConnection != null)
                {
                    _rdpConnection.Events.MouseCursorShapeChanged -= OnMouseCursorShapeChanged;
                    _rdpConnection.Events.MouseCursorPositionChanged -= OnMouseCursorPositionChanged;

                }

                _rdpConnection = value;
                _rdpConnection.Events.MouseCursorShapeChanged += OnMouseCursorShapeChanged;
                _rdpConnection.Events.MouseCursorPositionChanged += OnMouseCursorPositionChanged;
            } 
        }

        public IExecutionDeferrer DeferredExecution { private get; set; }

        public MouseViewModel()
        {
            this.PointerEventConsumer = new PointerEventConsumer(new WinrtThreadPoolTimer(), this);
        }

        private void OnMouseCursorShapeChanged(object sender, MouseCursorShapeChangedArgs args)
        {

            this.DeferredExecution.DeferToUI(() => {
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
                _rdpConnection.SendMouseEvent(eventType, (float) this.MousePosition.X, (float) this.MousePosition.Y);
            }
        }
    }
}
