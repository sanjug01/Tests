﻿namespace RdClient.Shared.Test.Model
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
        public void NewDesktopModel_DefaultExtraSettingsProperties()
        {
            DesktopModel model = new DesktopModel();

            Assert.IsFalse(model.IsAdminSession);
            Assert.IsFalse(model.IsSwapMouseButtons);
            Assert.AreEqual(AudioMode.Local, model.AudioMode);
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
            Assert.AreEqual(3, changes.Count);
            Assert.AreEqual("HostName", changes[0].PropertyName);
            Assert.AreEqual("Status", changes[1].PropertyName);
        }

        [TestMethod]
        public void DesktopModel_ChangeHostName_ChangesDisplayName()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();
            DesktopModel model = new DesktopModel();
            model.PropertyChanged += (sender, e) => changes.Add(e);

            model.HostName = "NewHostName";

            Assert.AreEqual("NewHostName", model.DisplayName);
            Assert.AreEqual(3, changes.Count);
            Assert.AreEqual("HostName", changes[0].PropertyName);
            Assert.AreEqual("DisplayName", changes[2].PropertyName);
        }

        [TestMethod]
        public void DesktopModel_ChangeFriendlyName_ChangesDisplayName()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();
            DesktopModel model = new DesktopModel();
            model.PropertyChanged += (sender, e) => changes.Add(e);

            model.FriendlyName = "NewFriendlyHostName";

            Assert.AreEqual("NewFriendlyHostName", model.FriendlyName);
            Assert.AreEqual("NewFriendlyHostName", model.DisplayName);
            Assert.AreEqual(3, changes.Count);
            Assert.AreEqual("FriendlyName", changes[0].PropertyName);
            Assert.AreEqual("DisplayName", changes[2].PropertyName);
        }

        [TestMethod]
        public void DesktopModel_ChangeFriendlyName_ReplacesDisplayName()
        {
            DesktopModel model = new DesktopModel();

            model.HostName = "NewHostName";
            Assert.AreEqual("NewHostName", model.HostName);
            Assert.AreEqual("NewHostName", model.DisplayName);

            model.FriendlyName = "NewFriendlyHostName";
            Assert.AreEqual("NewHostName", model.HostName);
            Assert.AreEqual("NewFriendlyHostName", model.FriendlyName);
            Assert.AreEqual("NewFriendlyHostName", model.DisplayName);
        }

        [TestMethod]
        public void DesktopModel_ChangeHostName_DoesNotReplaceDisplayName()
        {
            DesktopModel model = new DesktopModel();

            model.FriendlyName = "NewFriendlyHostName";
            Assert.AreEqual("NewFriendlyHostName", model.FriendlyName);
            Assert.AreEqual("NewFriendlyHostName", model.DisplayName);

            model.HostName = "NewHostName";
            Assert.AreEqual("NewHostName", model.HostName);
            Assert.AreEqual("NewFriendlyHostName", model.FriendlyName);
            Assert.AreEqual("NewFriendlyHostName", model.DisplayName);
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

        [TestMethod]
        public void DesktopModel_ChangeExtraSettings_ChangesReported()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();
            DesktopModel model = new DesktopModel();
            model.PropertyChanged += (sender, e) => changes.Add(e);

            model.AudioMode = AudioMode.NoSound;
            model.IsAdminSession = true;
            model.IsSwapMouseButtons = true;

            Assert.IsTrue(model.IsAdminSession);
            Assert.IsTrue(model.IsSwapMouseButtons);
            Assert.AreEqual(AudioMode.NoSound, model.AudioMode);

            Assert.AreEqual(4, changes.Count);
            Assert.AreEqual("AudioMode", changes[0].PropertyName);
            Assert.AreEqual("Status", changes[1].PropertyName);
            Assert.AreEqual("IsAdminSession", changes[2].PropertyName);
            Assert.AreEqual("IsSwapMouseButtons", changes[3].PropertyName);
        }
    }
}
