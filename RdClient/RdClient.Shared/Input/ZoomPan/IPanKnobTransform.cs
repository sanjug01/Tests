using RdClient.Shared.CxWrappers;
using Windows.Foundation;

namespace RdClient.Shared.Input.ZoomPan
{
    public enum PanKnobTransformType
    {
        ShowKnob = 1,     
        HideKnob = 2,    
        MoveKnob = 3
    }

    public interface IPanKnobTransform
    {
        PanKnobTransformType TransformType { get; }
    }
}
