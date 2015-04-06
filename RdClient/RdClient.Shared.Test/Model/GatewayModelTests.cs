namespace RdClient.Shared.Test.Model
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Models;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    [TestClass]
    public sealed class GatewayModelTests
    {
        [TestMethod]
        public void GatewayModel_New_DefaultProperties()
        {
            GatewayModel model = new GatewayModel();

            Assert.IsNull(model.HostName);
            Assert.IsFalse(model.HasCredentials);
            Assert.AreEqual(Guid.Empty, model.CredentialsId);
        }

        [TestMethod]
        public void GatewayModel_ChangeCredentialsId_ChangeReported()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();
            GatewayModel model = new GatewayModel();
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
        public void GatewayModel_ChangeHostName_ChangeReported()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();
            GatewayModel model = new GatewayModel();
            model.PropertyChanged += (sender, e) => changes.Add(e);

            model.HostName = "NewHostName";

            Assert.AreEqual("NewHostName", model.HostName);
            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual("HostName", changes[0].PropertyName);
            Assert.AreEqual("Status", changes[1].PropertyName);
        }
    }
}
