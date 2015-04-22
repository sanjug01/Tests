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
    }
}
