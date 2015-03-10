using RdClient.Shared.CxWrappers;
using RdMock;

namespace RdClient.Shared.Test.Mock
{
    public sealed class RdpConnectionFactory : MockBase, IRdpConnectionFactory
    {
        public IRdpConnection CreateDesktop()
        {
            return (IRdpConnection) Invoke(new object[] { });
        }

        IRdpConnection IRdpConnectionFactory.CreateApplication(string rdpFile)
        {
            return (IRdpConnection)Invoke(new object[] { rdpFile });
        }
    }
}
