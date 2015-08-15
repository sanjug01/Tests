using RdClient.Shared.CxWrappers;

namespace RdClient.Shared.Input.Pointer
{

    public enum InputMode
    {
        Touch,
        Mouse
    }

    // this interface implements the callbacks from IRemoteSessionControl
    // typically the implementation will also initiate the setup of the pointer input infrastructure
    public interface IPointerCapture
    {
        IConsumptionModeTracker ConsumptionMode { get; }
        IInputDeviceTracker InputDevice { get; }

        InputMode InputMode { get; set; }

        void OnPointerChanged(object sender, IPointerEventBase e);
        void OnMouseCursorPositionChanged(object sender, MouseCursorPositionChangedArgs args);
        void OnMouseCursorShapeChanged(object sender, MouseCursorShapeChangedArgs args);
        void OnMultiTouchEnabledChanged(object sender, MultiTouchEnabledChangedArgs args);
    }
}
