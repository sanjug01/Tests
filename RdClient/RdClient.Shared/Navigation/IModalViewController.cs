namespace FadeTest.Navigation
{
    /// <summary>
    /// Interface of a controller of a modal view passed to views that are being presented modally.
    /// The view may use the controller to dismiss itself.
    /// If the dismissed view is not at the top of the modal stack, all modal views above the dismissed one
    /// are also dismissed.
    /// </summary>
    public interface IModalViewController
    {
        void Dismiss();
    }
}
