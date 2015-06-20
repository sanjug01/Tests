namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Input.Keyboard;

    public interface IInputPanelFactorySite
    {
        void SetInputPanelFactory(IInputPanelFactory inputPanelFactory);
    }
}
