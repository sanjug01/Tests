using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Navigation.Extensions;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace RdClient.Shared.ViewModels
{

    using MousePointer = Tuple<int, float, float>;
    using Position = Tuple<float, float>;

    public class MouseViewModel : MutableObject
    {
        private float _xHotspot = (float)0.0;
        private float _yHotspot = (float)0.0;
        private MousePointer _mousePointer;
        public MousePointer MousePointer { 
            get { return _mousePointer; } 
            set {
                SetProperty(ref _mousePointer, value);
                OnMousePointerChanged();
            } 
        }

        private ImageSource _mousePointerShape;
        public ImageSource MousePointerShape { get { return _mousePointerShape; } set { SetProperty(ref _mousePointerShape, value); } }

        private Position _mousePointerShapePosition = new Position((float) 0.0, (float) 0.0);
        public Position MousePointerShapePosition { 
            get { return new Position(_mousePointerShapePosition.Item1 - _xHotspot, _mousePointerShapePosition.Item2 - _yHotspot); } 
            set { SetProperty(ref _mousePointerShapePosition, value); } 
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
        }

        private void OnMouseCursorShapeChanged(object sender, MouseCursorShapeChangedArgs args)
        {
            _xHotspot = args.XHotspot;
            _yHotspot = args.YHotspot;

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

                    this.MousePointerShape = spBitmap;
                }
            });
        }

        private void OnMouseCursorPositionChanged(object sender, MouseCursorPositionChangedArgs args)
        {

        }

        private void OnMousePointerChanged()
        {
            float xPos = this.MousePointer.Item2;
            float yPos = this.MousePointer.Item3;
            MouseEventType eventType = MouseEventType.Unknown;

            switch(this.MousePointer.Item1)
            {
                case(0):
                    eventType = MouseEventType.LeftPress;
                    break;
                case (1):
                    eventType = MouseEventType.LeftRelease;
                    break;
                case (2):
                    eventType = MouseEventType.MouseHWheel;
                    break;
                case (3):
                    eventType = MouseEventType.MouseWheel;
                    break;
                case (4):
                    eventType = MouseEventType.Move;
                    break;
                case (5):
                    eventType = MouseEventType.RightPress;
                    break;
                case (6):
                    eventType = MouseEventType.RightRelease;
                    break;
                default:
                    eventType = MouseEventType.Unknown;
                    break;
            }

            this.MousePointerShapePosition = new Position(xPos, yPos);

            if(_rdpConnection != null)
            {
                _rdpConnection.SendMouseEvent(eventType, xPos, yPos);            
            }
        }
    }
}
