namespace RdClient.Shared.Helpers
{
    public interface IScaleFactor
    {
        int DesktopScaleFactor { get; }
        int DeviceScaleFactor { get; }
    }
}