namespace RdClient.Shared.Data
{
    using System.Windows.Input;

    /// <summary>
    /// Interface of an object that may save its state on some persistent media.
    /// </summary>
    public interface IPersistentObject
    {
        /// <summary>
        /// Save the object to the same media from that it was loaded.
        /// </summary>
        ICommand Save { get; }
    }
}
