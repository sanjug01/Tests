using RdClient.Shared.CxWrappers;
using RdMock;

namespace RdClient.Shared.Test.Mock
{
    public sealed class RdpConnectionFactory : MockBase, IRdpConnectionFactory
    {
        public IRdpConnection CreateDesktop(string rdpFile)
        {
            return (IRdpConnection) Invoke(new object[] { rdpFile });
        }

        public IRdpConnection CreateApplication(string rdpFile)
        {
            return (IRdpConnection)Invoke(new object[] { rdpFile });
        }        
    }
}
