using RdClient.Shared.CxWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.Mock
{
    class RdpProperties : IRdpProperties
    {
        public IList<string> _getIntPropertyName = new List<string>();
        public IList<int> _getIntPropertyValue = new List<int>();

        public IList<string> _setIntPropertyName = new List<string>();
        public IList<int> _setIntPropertyValue = new List<int>();

        public IList<string> _getStringPropertyName = new List<string>();
        public IList<string> _getStringPropertyValue = new List<string>();

        public IList<string> _setStringPropertyName = new List<string>();
        public IList<string> _setStringPropertyValue = new List<string>();

        public IList<string> _getBoolPropertyName = new List<string>();
        public IList<bool> _getBoolPropertyValue = new List<bool>();

        public IList<string> _setBoolPropertyName = new List<string>();
        public IList<bool> _setBoolPropertyValue = new List<bool>();

        public int GetIntProperty(string propertyName)
        {
            _getIntPropertyName.Add(propertyName);
            int retval = _getIntPropertyValue[0];
            _getIntPropertyValue.RemoveAt(0);

            return retval;
        }

        public void SetIntProperty(string propertyName, int value)
        {
            _setIntPropertyName.Add(propertyName);
            _setIntPropertyValue.Add(value);
        }

        public string GetStringPropery(string propertyName)
        {
            _getStringPropertyName.Add(propertyName);
            string retval = _getStringPropertyValue[0];
            _getStringPropertyValue.RemoveAt(0);

            return retval;
        }

        public void SetStringProperty(string propertyName, string value)
        {
            _setStringPropertyName.Add(propertyName);
            _setStringPropertyValue.Add(value);
        }

        public bool GetBoolProperty(string propertyName)
        {
            _getBoolPropertyName.Add(propertyName);
            bool retval = _getBoolPropertyValue[0];
            _getStringPropertyValue.RemoveAt(0);

            return retval;
        }

        public void SetBoolProperty(string propertyName, bool value)
        {
            _setBoolPropertyName.Add(propertyName);
            _setBoolPropertyValue.Add(value);
        }
    }
}
