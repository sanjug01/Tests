
namespace RdClient.Shared.CxWrappers
{
    public interface IRdpProperties
    {
        int GetIntProperty(string propertyName);
        void SetIntProperty(string propertyName, int value);
        string GetStringPropery(string propertyName);
        void SetStringProperty(string propertyName, string value);
        bool GetBoolProperty(string propertyName);
        void SetBoolProperty(string propertyName, bool value);

        // left handed mouse mode is a special property that needs to be applied directly to the RDClientCX.RdpConnection
        void SetLeftHandedMouseModeProperty(bool value);
    }
}
