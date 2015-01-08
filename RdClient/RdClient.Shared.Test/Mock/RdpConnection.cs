﻿using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Models;
using RdMock;

namespace RdClient.Shared.Test.Mock
{
    class RdpConnection : MockBase, IRdpConnection, IRdpProperties
    {
        private readonly IRdpEvents _events;

        public RdpConnection(IRdpEvents events)
        {
            if (events == null)
            {
                _events = new Mock.RdpEvents();
            }
            else
            {
                _events = events;
            }
        }

        public void Connect(Credentials credentials, bool fUsingSavedCreds)
        {
            Invoke(new object[] { credentials, fUsingSavedCreds });
        }

        public void Disconnect()
        {
            Invoke(new object[] {});
        }

        public void Suspend()
        {
            Invoke(new object[] { });
        }

        public void Resume()
        {
            Invoke(new object[] { });
        }

        public void TerminateInstance()
        {
            Invoke(new object[] { });
        }

        public void HandleAsyncDisconnectResult(RdpDisconnectReason disconnectReason, bool reconnectToServer)
        {
            Invoke(new object[] { disconnectReason, reconnectToServer });
        }

        public IRdpEvents Events
        {
            get { return _events; }
        }

        public int GetIntProperty(string propertyName)
        {
            return (int)Invoke(new object[] { propertyName });
        }

        public void SetIntProperty(string propertyName, int value)
        {
            Invoke(new object[] { propertyName, value });
        }

        public string GetStringPropery(string propertyName)
        {
            return (string)Invoke(new object[] { propertyName });
        }

        public void SetStringProperty(string propertyName, string value)
        {
            Invoke(new object[] { propertyName, value });
        }

        public bool GetBoolProperty(string propertyName)
        {
            return (bool)Invoke(new object[] { propertyName });
        }

        public void SetBoolProperty(string propertyName, bool value)
        {
            Invoke(new object[] { propertyName, value });
        }

        public IRdpScreenSnapshot GetSnapshot()
        {
            return (IRdpScreenSnapshot) Invoke(new object[] { });
        }        

        public IRdpCertificate GetServerCertificate()
        {
            return (IRdpCertificate)Invoke(new object[] { });
        }

        public void SendMouseEvent(MouseEventType type, float xPos, float yPos)
        {
            Invoke(new object[] { type, xPos, yPos });
        }

        public void SendKeyEvent(int keyValue, bool scanCode, bool extended, bool keyUp)
        {
            Invoke(new object[] { keyValue, scanCode, extended, keyUp });
        }
    }
}
