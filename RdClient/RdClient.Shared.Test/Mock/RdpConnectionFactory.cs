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

        {
            return (IRdpConnection)Invoke(new object[] { rdpFile });
        }
    }
}
