namespace RdClient.Shared.Helpers
{
    /// <summary>
    /// Interface of an object that moves input focus between UI elements.
    /// </summary>
    public interface IInputFocusController
    {
        /// <summary>
        /// Set input focus to the default UI element.
        /// </summary>
        void SetDefault();
    }
}
