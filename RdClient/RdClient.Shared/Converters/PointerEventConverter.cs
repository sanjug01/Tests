using RdClient.Shared.CxWrappers;
using RdClient.Shared.Input.Pointer;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace RdClient.Shared.Converters
{
    public class PointerEventConverter
    {
        public static PointerTypeOld PointerTypeConverter(PointerDeviceType type)
        {
            PointerTypeOld rtype = PointerTypeOld.Unknown;

            switch(type)
            {
                case PointerDeviceType.Mouse:
                    rtype = PointerTypeOld.Mouse;
                    break;
                case PointerDeviceType.Pen:
                    rtype = PointerTypeOld.Pen;
                    break;
                case PointerDeviceType.Touch:
                    rtype = PointerTypeOld.Touch;
                    break;
                default:
                    rtype = PointerTypeOld.Unknown;
                    break;
            }

            return rtype;
        }

        public static PointerEventOld ManipulationStartedArgsConverter(ManipulationStartedRoutedEventArgs args)
        {
            PointerEventOld pe = new PointerEventOld(
                args.Position, 
                false, 
                args.Cumulative.Translation, 
                false, 
                false, 
                PointerTypeConverter(args.PointerDeviceType), 
                0);

            return pe;
        }

        public static PointerEventOld ManipulationInertiaStartingArgsConverter(ManipulationInertiaStartingRoutedEventArgs args)
        {
            PointerEventOld pe = new PointerEventOld(new Point(0.0, 0.0), true, args.Delta.Translation, false, false, PointerTypeConverter(args.PointerDeviceType), 0);

            return pe;
        }

        public static PointerEventOld ManipulationDeltaArgsConverter(ManipulationDeltaRoutedEventArgs args)
        {
            PointerEventOld pe = new PointerEventOld(args.Position, args.IsInertial, args.Delta.Translation, false, false, PointerTypeConverter(args.PointerDeviceType), 0);

            return pe;
        }

        public static PointerEventOld ManipulationCompletedArgsConverter(ManipulationCompletedRoutedEventArgs args)
        {
            PointerEventOld pe = new PointerEventOld(args.Position, false, args.Cumulative.Translation, false, false, PointerTypeConverter(args.PointerDeviceType), 0);

            return pe;
        }

        public static PointerEventOld PointerArgsConverter(UIElement receiver, PointerRoutedEventArgs args, TouchEventType actionType)
        {
            PointerPoint ppoint = args.GetCurrentPoint(receiver);
            Point position = new Point(ppoint.Position.X, ppoint.Position.Y);
            PointerEventOld pe = new PointerEventOld(
                position, false, new Point(0.0, 0.0),
                ppoint.Properties.IsLeftButtonPressed, ppoint.Properties.IsRightButtonPressed,
                PointerTypeConverter(ppoint.PointerDevice.PointerDeviceType), ppoint.PointerId,
                args.GetCurrentPoint(receiver).Timestamp,
                actionType,
                ppoint.Properties.MouseWheelDelta,
                ppoint.Properties.IsHorizontalMouseWheel);

            return pe;
        }

        public static PointerEventOld PointerArgsConverter(Windows.UI.Core.PointerEventArgs args, TouchEventType actionType)
        {
            PointerPoint ppoint = args.CurrentPoint;
            Point position = new Point(ppoint.Position.X, ppoint.Position.Y);
            PointerEventOld pe = new PointerEventOld(
                position, false, new Point(0.0, 0.0),
                ppoint.Properties.IsLeftButtonPressed, ppoint.Properties.IsRightButtonPressed,
                PointerTypeConverter(ppoint.PointerDevice.PointerDeviceType), ppoint.PointerId,
                args.CurrentPoint.Timestamp,
                actionType,
                ppoint.Properties.MouseWheelDelta,
                ppoint.Properties.IsHorizontalMouseWheel);

            return pe;
        }
    }
}
