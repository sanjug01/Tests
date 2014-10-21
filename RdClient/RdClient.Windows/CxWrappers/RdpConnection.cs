using System.Diagnostics.Contracts;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;

namespace RdClient.CxWrappers
{
    class RdpConnection : IRdpConnection, IRdpProperties
    {
        private RdClientCx.RdpConnection _rdpConnection;

        public static RdpConnection CreateInstance(CoreWindow spWindow, SwapChainPanel spPanel)
        {
            RdClientCx.RdpConnection rdpConnectionCx;
            RdClientCx.RdpConnection.CreateInstance(spWindow, spPanel, out rdpConnectionCx);

            return new RdpConnection(rdpConnectionCx);
        }

        public RdpConnection(RdClientCx.RdpConnection rdpConnectionCx)
        {
            Contract.Requires(rdpConnectionCx != null);
            _rdpConnection = rdpConnectionCx;
        }

        public int SetUserCredentials(Credentials credentials, bool fUsingSavedCreds)
        {
            return _rdpConnection.SetUserCredentials(credentials.username, credentials.domain, credentials.password, fUsingSavedCreds);
        }

        public void Connect()
        {
            _rdpConnection.Connect();
        }

        public void Disconnect()
        {
            _rdpConnection.Disconnect();
        }

        public void Suspend()
        {
            _rdpConnection.Suspend();
        }

        public void Resume()
        {
            _rdpConnection.Suspend();
        }

        public int GetIntProperty(string propertyName)
        {
            int value;
            int xRes = _rdpConnection.GetIntProperty(propertyName, out value);

            RdTrace.IfFailXResultThrow(xRes, "Failed to get int property: " + propertyName);

            return value;
        }

        public void SetIntProperty(string propertyName, int value)
        {
            int xRes = _rdpConnection.SetIntProperty(propertyName, value);
            RdTrace.IfFailXResultThrow(xRes, "Failed to set int property: " + propertyName);
        }

        public string GetStringPropery(string propertyName)
        {
            string value;
            int xRes = _rdpConnection.GetStringProperty(propertyName, out value);

            RdTrace.IfFailXResultThrow(xRes, "Failed to get string property: " + propertyName);

            return value;
        }

        public void SetStringProperty(string propertyName, string value)
        {
            int xRes = _rdpConnection.SetStringProperty(propertyName, value);
            RdTrace.IfFailXResultThrow(xRes, "Failed to set string property: " + propertyName);
        }


        public bool GetBoolProperty(string propertyName)
        {
            bool value;
            int xRes = _rdpConnection.GetBoolProperty(propertyName, out value);

            RdTrace.IfFailXResultThrow(xRes, "Failed to get bool property: " + propertyName);

            return value;
        }

        public void SetBoolProperty(string propertyName, bool value)
        {
            int xRes = _rdpConnection.SetBoolProperty(propertyName, value);
            RdTrace.IfFailXResultThrow(xRes, "Failed to set bool property: " + propertyName);
        }
    }
}
