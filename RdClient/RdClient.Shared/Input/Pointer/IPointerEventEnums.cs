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
        PointerCanceled,
        ZoomScrollStarted,
        ZoomScrollUpdating,
        ZoomScrollCompleted,
        Tapped,
        HoldingStarted,
        HoldingCompleted,
        HoldingCanceled,
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
