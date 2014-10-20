using RdClient.Shared.CxWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.Mock
{
    class RdpConnection : IRdpConnection
    {
        private string _user;
        private string _domain;
        private string _password;
        private bool _usingSavedCreds;

        public string User { get { return _user; } }
        public string Domain { get { return _domain;  } }
        public string Password { get { return _password; } }
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

        public int SetUserCredentials(string strUser, string strDomain, string strPassword, bool fUsingSavedCreds)
        {
            _user = strUser;
            _domain = strDomain;
            _password = strPassword;
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
