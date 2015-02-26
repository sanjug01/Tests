using System.ComponentModel;
namespace RdClient.Shared.Models
{
    [DefaultValue(Idle)]
    public enum SessionState
    {
        Idle,
        Connecting,
        Connected
    }

    /// <summary>
    /// Representation of the state of a remote session synchronized with the UI thread.
    /// </summary>
    public interface IRemoteSessionState : INotifyPropertyChanged
    {
        SessionState State { get; }
    }
}
