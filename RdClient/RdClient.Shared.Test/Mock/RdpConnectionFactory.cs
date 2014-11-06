using RdClient.Shared.CxWrappers;
using RdMock;

namespace RdClient.Shared.Test.Mock
{
    public class RdpConnectionFactory : MockBase, IRdpConnectionFactory
    {
        public IRdpConnection CreateInstance()
        {
            return (IRdpConnection) Invoke(new object[] { });
        }
    }
}
