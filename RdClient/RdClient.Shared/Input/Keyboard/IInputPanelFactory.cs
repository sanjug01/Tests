namespace RdClient.Shared.Input.Keyboard
{
    /// <summary>
    /// Interface of a factory of IInputPanel objects. The interface is injected into components
    /// that need to access the input panel, so the objects later may obtain their own panels.
    /// </summary>
    /// <remarks>Input panel in most cases represents the touch keyboard, but it may also accept
    /// pen or finger painting input to produce text.</remarks>
    public interface IInputPanelFactory
    {
        IInputPanel GetInputPanel();
    }
}
