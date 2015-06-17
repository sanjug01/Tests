namespace RdClient.Shared.Models
{
    using System.ComponentModel;

    public interface IDeviceCapabilities : INotifyPropertyChanged
    {
        /// <summary>
        /// Touch digitizer is present in the system.
        /// </summary>
        bool TouchPresent { get; }

        /// <summary>
        /// Minimum number of touch contacts supported by all digitizers in the system.
        /// </summary>
        uint TouchPoints { get; }

        /// <summary>
        /// True if the touch keyboard (input panel) can be shown by the InputPanel.TryShow function.
        /// </summary>
        /// <remarks>The property is mutable; changes of its value are reported through the INotifyPropertyChanged interface.
        /// One of the scenarios in which value of the property may change is when the user attaches or detaches a USB
        /// keyboard.</remarks>
        bool CanShowInputPanel { get; }

        /// <summary>
        /// Human-readable name of the current user interaction mode (may be "Mouse" or "Touch",
        /// more values may be available later as the platform evolves).
        /// </summary>
        string UserInteractionMode { get; }
    }
}
