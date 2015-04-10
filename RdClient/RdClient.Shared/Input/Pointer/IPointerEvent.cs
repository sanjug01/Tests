using System;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;

namespace RdClient.Shared.Input
{
    public enum PointerEventAction
    {
        ManipulationStarting,
        ManipulationStarted,
        ManipulationDelta,
        ManipulationInertiaStarting,
        ManipulationCompleted,
        PointerPressed,
        PointerMoved,
        PointerReleased,
        PointerCanceled,
        ZoomScrollStarted,
        ZoomScrollUpdating,
        ZoomScrollCompleted
    }

    public enum PointerEventType
    {
        PointerRoutedEventArgs,
        ManipulationCompletedRoutedEventArgs,
        ManipulationInertiaStartingRoutedEventArgs,
        ManipulationDeltaRoutedEventArgs,
        ManipulationStartedRoutedEventArgs,
        ManipulationStartingRoutedEventArgs
    }

    public interface IPointerEventBase
    {
        PointerEventAction Action { get; }
        Point Position { get; }
    }

    public interface IPointerRoutedEventProperties : IPointerEventBase
    {
        PointerDeviceType DeviceType { get; }
        UInt32 PointerId { get; }
        ulong Timestamp { get; }
    }
    public interface IManipulationRoutedEventProperties : IPointerEventBase
    {
        ManipulationDelta Cummulative { get; }
        ManipulationDelta Delta { get; }
        bool IsInertial { get; }
    }


}
