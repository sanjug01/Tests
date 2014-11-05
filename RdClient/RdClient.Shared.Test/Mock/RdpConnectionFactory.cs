using RdClient.Shared.CxWrappers;
using RdMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
