using RdClient.Shared.CxWrappers;
using Windows.Foundation;

namespace RdClient.Shared.Input.ZoomPan
{

    public enum ZoomPanState
    {
        TouchMode_MinScale,
        TouchMode_MaxScale, 
        TouchMode_PanKnobPanning, 
        TouchMode_PanKnobMoving,
        PointerMode_DefaultScale, 
        PointerMode_Zooming, 
        PointerMode_Zoomed, 
        PointerMode_Panning
    }
}
