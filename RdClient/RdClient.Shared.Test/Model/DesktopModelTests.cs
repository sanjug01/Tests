namespace RdClient.Shared.Test.Model
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Models;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    [TestClass]
    public sealed class DesktopModelTests
    {
        [TestMethod]
        public void NewDesktopModel_DefaultProperties()
        {
            DesktopModel model = new DesktopModel();

            Assert.IsNull(model.HostName);
            Assert.IsFalse(model.HasCredentials);
            Assert.AreEqual(Guid.Empty, model.CredentialsId);
        }

        [TestMethod]
        public void DesktopModel_ChangeCredentialsId_ChangeReported()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();
            DesktopModel model = new DesktopModel();
            Guid id = Guid.NewGuid();
            model.PropertyChanged += (sender, e) => changes.Add(e);
            
            model.CredentialsId = id;

            Assert.AreEqual(id, model.CredentialsId);
            Assert.AreEqual(3, changes.Count);
            Assert.AreEqual("CredentialsId", changes[0].PropertyName);
            Assert.AreEqual("Status", changes[1].PropertyName);
            Assert.AreEqual("HasCredentials", changes[2].PropertyName);
        }

        [TestMethod]
        public void DesktopModel_ChangeHostName_ChangeReported()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();
            DesktopModel model = new DesktopModel();
            model.PropertyChanged += (sender, e) => changes.Add(e);

            model.HostName = "NewHostName";

            Assert.AreEqual("NewHostName", model.HostName);
            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual("HostName", changes[0].PropertyName);
            Assert.AreEqual("Status", changes[1].PropertyName);
        }

        [TestMethod]
        public void DesktopModel_ChangeGatewayId_ChangeReported()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();
            DesktopModel model = new DesktopModel();
            Guid id = Guid.NewGuid();
            model.PropertyChanged += (sender, e) => changes.Add(e);

            model.GatewayId = id;

            Assert.AreEqual(id, model.GatewayId);
            Assert.AreEqual(3, changes.Count);
            Assert.AreEqual("GatewayId", changes[0].PropertyName);
            Assert.AreEqual("Status", changes[1].PropertyName);
            Assert.AreEqual("HasGateway", changes[2].PropertyName);
        }
    }
}
