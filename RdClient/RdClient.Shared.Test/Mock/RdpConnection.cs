using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.Mock
{
    class RdpConnection : IRdpConnection
    {
        private Credentials _credentials;
        private bool _usingSavedCreds;

        public Credentials Credentials { get { return _credentials; } }
        public bool UsingSavedCreds { get { return _usingSavedCreds; } }
        public int UserCredentialsReturnValue { get; set; }

        private int _connectCount = 0;
        private int _disconnectCount = 0;
        private int _suspendCount = 0;
        private int _resumeCount = 0;

        public int ConnectCount { get { return _connectCount; } }
        public int Disconnectcount { get { return _disconnectCount; } }
        public int SuspendCount { get { return _suspendCount; } }
        public int ResumeCount { get { return _resumeCount; } }

        public int SetUserCredentials(Credentials credentials, bool fUsingSavedCreds)
        {
            _credentials = credentials;
            _usingSavedCreds = fUsingSavedCreds;

            return UserCredentialsReturnValue;
        }

        public void Connect()
        {
            _connectCount++;
        }

        public void Disconnect()
        {
            _disconnectCount++;
        }

        public void Suspend()
        {
            _suspendCount++;
        }

        public void Resume()
        {
            _resumeCount++;
        }
    }
}
