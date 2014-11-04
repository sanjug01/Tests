using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using System;

namespace RdClient.Shared.Test.CxWrappers
{
    [TestClass]
    public class RdpEventArgsTests
    {
        [TestMethod]
        public void ClientAsyncDisconnectArgs_Constructor()
        {
            ClientAsyncDisconnectArgs cada = new ClientAsyncDisconnectArgs(new RdpDisconnectReason(
                RdpDisconnectCode.UserInitiated, 
                23, 
                42
                ));

            Assert.AreEqual(RdpDisconnectCode.UserInitiated, cada.DisconnectReason.Code);
            Assert.AreEqual((UInt32) 23, cada.DisconnectReason.ULegacyCode);
            Assert.AreEqual((UInt32) 42, cada.DisconnectReason.ULegacyExtendedCode);
        }
    }
}
