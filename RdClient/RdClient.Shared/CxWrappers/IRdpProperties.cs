
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
    }
}
