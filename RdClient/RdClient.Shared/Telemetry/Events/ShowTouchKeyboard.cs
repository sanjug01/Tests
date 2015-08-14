namespace RdClient.Shared.Telemetry.Events
{
    /// <summary>
    /// Telemetry data point reported when user has requested the touch keyboard.
    /// </summary>
    public sealed class ShowTouchKeyboard
    {
        public ShowTouchKeyboard(double mark)
        {
            this.timeMark = mark;
        }

        /// <summary>
        /// Time mark relative to the beginning of the session.
        /// </summary>
        public readonly double timeMark;
    }
}
