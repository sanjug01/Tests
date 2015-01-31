
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

        // left handed mouse mode cannot be used as a property
        void SetLeftHandedMouseMode(bool value);
    }
}
