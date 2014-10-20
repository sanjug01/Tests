using System.Diagnostics.Contracts;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using RdClient.Shared.CxWrappers;

namespace RdClient.CxWrappers
{
    class RdpConnection : IRdpConnection
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

        public int SetUserCredentials(string strUser, string strDomain, string strPassword, bool fUsingSavedCreds)
        {
            return _rdpConnection.SetUserCredentials(strUser, strDomain, strPassword, fUsingSavedCreds);
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
    }
}
