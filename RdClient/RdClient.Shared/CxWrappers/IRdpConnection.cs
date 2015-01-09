﻿using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Models;

namespace RdClient.Shared.CxWrappers
{
    public interface IRdpConnection
    {
        IRdpEvents Events { get; }

        void Connect(Credentials credentials, bool fUsingSavedCreds);
        void Disconnect();
        void Suspend();
        void Resume();
        void TerminateInstance();
        void Cleanup();
        void HandleAsyncDisconnectResult(RdpDisconnectReason disconnectReason, bool reconnectToServer);
        IRdpScreenSnapshot GetSnapshot();
        IRdpCertificate GetServerCertificate();
        void SendMouseEvent(MouseEventType type, float xPos, float yPos);
    }
}
