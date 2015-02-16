namespace RdClient.Shared.Data
{
    using System.ComponentModel;

    /// <summary>
    /// Persistent status of an object - an observable read-only property "Status" and a method that resets the object
    /// to Clean state.
    /// </summary>
    public interface IPersistentStatus : INotifyPropertyChanged
    {
        PersistentStatus Status { get; }

        void SetClean();
    }
}
