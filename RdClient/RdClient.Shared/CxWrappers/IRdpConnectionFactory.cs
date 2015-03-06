
using RdClient.Shared.Models;
namespace RdClient.Shared.CxWrappers
{
    public interface IRdpConnectionFactory
    {
        IRdpConnection CreateInstance();
        ConnectionInformation ConnectionInformation { set; }
    }
}
