namespace RdClient.Shared.Models
{
    /// <summary>
    /// Interface for controlling input sent to an active remote session.
    /// </summary>
    public interface IRemoteSessionControl
    {
        IRenderingPanel RenderingPanel { get; }

        void SendKeystroke(int keyCode, bool isScanCode, bool isExtendedKey, bool isKeyReleased);
    }
}
