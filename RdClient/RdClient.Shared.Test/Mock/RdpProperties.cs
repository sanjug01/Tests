using RdClient.Shared.CxWrappers;
using RdMock;

namespace RdClient.Shared.Test.Mock
{
    class RdpProperties : MockBase, IRdpProperties
    {
        public int GetIntProperty(string propertyName)
        {
            return (int)Invoke(new object[] { propertyName });
        }

        public void SetIntProperty(string propertyName, int value)
        {
            Invoke(new object[] { propertyName, value });
        }

        public string GetStringPropery(string propertyName)
        {
            return (string)Invoke(new object[] { propertyName });
        }

        public void SetStringProperty(string propertyName, string value)
        {
            Invoke(new object[] { propertyName, value });
        }

        public bool GetBoolProperty(string propertyName)
        {
            return (bool)Invoke(new object[] { propertyName });
        }

        public void SetBoolProperty(string propertyName, bool value)
        {
            Invoke(new object[] { propertyName, value });
        }

        public void SetLeftHandedMouseMode(bool value)
        {
            Invoke(new object[] { value });
        }
    }
}
