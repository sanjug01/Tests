namespace RdClient.Shared.Input.Pointer
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
        PointerCancelled,
        ZoomScrollStarted,
        ZoomScrollUpdating,
        ZoomScrollCompleted,
        Tapped,
        HoldingStarted,
        HoldingCompleted,
        HoldingCancelled,
        DraggingStarted,
        DraggingUpdated,
        DraggingCompleted
    }

    public enum PointerEventType
    {
        PointerRoutedEventArgs,
        ManipulationCompletedRoutedEventArgs,
        ManipulationInertiaStartingRoutedEventArgs,
        ManipulationDeltaRoutedEventArgs,
        ManipulationStartedRoutedEventArgs,
        ManipulationStartingRoutedEventArgs,
        TappedEventArgs,
        HoldingEventArgs,
        DraggingEventArgs
    }
}
