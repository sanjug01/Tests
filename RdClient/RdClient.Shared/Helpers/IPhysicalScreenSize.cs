
namespace RdClient.Shared
{
    public struct ScreenSize
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }

    public interface IPhysicalScreenSize
    {
        ScreenSize GetScreenSize();
    }
}
