namespace RdClient.Shared.Telemetry.Events
{
    public sealed class HideTouchKeyboard
    {
        public HideTouchKeyboard(double mark)
        {
            this.timeMark = mark;
        }

        /// <summary>
        /// Time mark relative to the beginning of the session.
        /// </summary>
        public readonly double timeMark;
    }
}
