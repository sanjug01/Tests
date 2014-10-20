using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.CxWrappers
{
    public interface IRdpConnection
    {
        int SetUserCredentials(string strUser, string strDomain, string strPassword, bool fUsingSavedCreds);
        void Connect();
        void Disconnect();
        void Suspend();
        void Resume();
    }
}
