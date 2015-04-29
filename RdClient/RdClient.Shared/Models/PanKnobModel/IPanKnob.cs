using Windows.Foundation;

namespace RdClient.Shared.Models.PanKnobModel
{
    public interface IPanKnob
    {
        bool IsVisible { get; set; }
        Point Position { get; set; }

        Size Size { get; }
    }
}
