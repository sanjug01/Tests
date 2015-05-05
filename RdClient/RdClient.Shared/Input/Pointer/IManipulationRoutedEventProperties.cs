using Windows.UI.Input;

namespace RdClient.Shared.Input.Pointer
{
    public interface IManipulationRoutedEventProperties : IPointerEventBase
    {
        ManipulationDelta Cummulative { get; }
        ManipulationDelta Delta { get; }
        ManipulationVelocities Velocities { get; }
        bool IsInertial { get; }
    }
}
