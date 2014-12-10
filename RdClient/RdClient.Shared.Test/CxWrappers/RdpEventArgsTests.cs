using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Errors;
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

        [TestMethod]
        public void ClienDisconnectedArgs_Constructor()
        {
            ClientDisconnectedArgs cda = new ClientDisconnectedArgs(new RdpDisconnectReason(
                RdpDisconnectCode.UserInitiated,
                23,
                42
                ));

            Assert.AreEqual(RdpDisconnectCode.UserInitiated, cda.DisconnectReason.Code);
            Assert.AreEqual((UInt32)23, cda.DisconnectReason.ULegacyCode);
            Assert.AreEqual((UInt32)42, cda.DisconnectReason.ULegacyExtendedCode);
        }

        [TestMethod]
        public void UserCredentialsRequestArgs_Constructor()
        {
            UserCredentialsRequestArgs ucra = new UserCredentialsRequestArgs(23);

            Assert.AreEqual((int)23, ucra.SecLayer);
        }

        [TestMethod]
        public void MouseCursorShapeChangedArgs_Constructor()
        {
            byte[] buffer = new byte[] { 23 };
            MouseCursorShapeChangedArgs mcsca = new MouseCursorShapeChangedArgs(buffer, 5, 6, 7, 8);

            Assert.AreEqual(buffer, mcsca.Buffer);
            Assert.AreEqual(5, mcsca.Width);
            Assert.AreEqual(6, mcsca.Height);
            Assert.AreEqual(7, mcsca.XHotspot);
            Assert.AreEqual(8, mcsca.YHotspot);
        }

        [TestMethod]
        public void MouseCursorPositionChangedArgs_Constructor()
        {
            MouseCursorPositionChangedArgs mcpca = new MouseCursorPositionChangedArgs(23, 42);

            Assert.AreEqual(23, mcpca.XPos);
            Assert.AreEqual(42, mcpca.YPos);
        }

        [TestMethod]
        public void MultiTouchEnabledChangedArgs_Constructor()
        {
            MultiTouchEnabledChangedArgs mteca = new MultiTouchEnabledChangedArgs(true);

            Assert.IsTrue(mteca.MultiTouchEnabled);
        }

        [TestMethod]
        public void ConnectionHealthStateChangedArgs_Constructor()
        {
            ConnectionHealthStateChangedArgs chsca = new ConnectionHealthStateChangedArgs(23);

            Assert.AreEqual(23, chsca.ConnectionState);
        }

        [TestMethod]
        public void ClientAutoReconnectingArgs_Constructor()
        {
            bool reconnecting = false;
            ClientAutoReconnectingArgs cara = new ClientAutoReconnectingArgs(23, 42, (reconnecting_) => { reconnecting = reconnecting_; });

            cara.ContinueDelegate(true);

            Assert.AreEqual(23, cara.DisconnectReason);
            Assert.AreEqual(42, cara.AttemptCount);
            Assert.IsTrue(reconnecting);
        }

        [TestMethod]
        public void ClientAutoReconnectCompleteArgs_Constructor()
        {
            ClientAutoReconnectCompleteArgs carca = new ClientAutoReconnectCompleteArgs();
        }

        [TestMethod]
        public void LoginCompletedArgs_Constructor()
        {
            LoginCompletedArgs lca = new LoginCompletedArgs();
        }

        [TestMethod]
        public void StatusInfoReceivedArgs_Constructor()
        {
            StatusInfoReceivedArgs sira = new StatusInfoReceivedArgs(23);

            Assert.AreEqual(23, sira.StatusCode);
        }

        [TestMethod]
        public void FirstGraphicsUpdateArgs_Constructor()
        {
            FirstGraphicsUpdateArgs fgua = new FirstGraphicsUpdateArgs();
        }

        [TestMethod]
        public void RemoteAppWindowCreatedArgs_Constructor()
        {
            byte[] icon = new byte[] { 52 };
            RemoteAppWindowCreatedArgs rawca = new RemoteAppWindowCreatedArgs(23, "narf", icon, 42, 65);

            Assert.AreEqual((UInt32)23, rawca.WindowId);
            Assert.AreEqual("narf", rawca.Title);
            Assert.AreEqual(icon, rawca.Icon);
            Assert.AreEqual((UInt32)42, rawca.IconWidth);
            Assert.AreEqual((UInt32)65, rawca.IconHeight);
        }

        [TestMethod]
        public void RemoteAppWindowDeletedArgs_Constructor()
        {
            RemoteAppWindowDeletedArgs rawda = new RemoteAppWindowDeletedArgs(23);

            Assert.AreEqual((UInt32)23, rawda.WindowId);
        }

        [TestMethod]
        public void RemoteAppWindowTitleUpdatedArgs_Constructor()
        {
            RemoteAppWindowTitleUpdatedArgs rawtua = new RemoteAppWindowTitleUpdatedArgs(23, "narf");

            Assert.AreEqual((UInt32)23, rawtua.WindowId);
            Assert.AreEqual("narf", rawtua.Title);
        }

        [TestMethod]
        public void RemoteAppWindowIconUpdatedArgs_Constructor()
        {
            byte[] icon = new byte[] { 23 };
            RemoteAppWindowIconUpdatedArgs rawiua = new RemoteAppWindowIconUpdatedArgs(23, icon, 42, 56);

            Assert.AreEqual((UInt32)23, rawiua.WindowId);
            Assert.AreEqual(icon, rawiua.Icon);
            Assert.AreEqual((UInt32)42, rawiua.IconWidth);
            Assert.AreEqual((UInt32)56, rawiua.IconHeight);
        }
    }
}
