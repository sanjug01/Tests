namespace RdClient.Shared.Test.Model
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Test.Data;
    using System;

    [TestClass]
    public sealed class SessionGatewayTests
    {
        private ApplicationDataModel _dataModel;

        [TestInitialize]
        public void SetUpTest()
        {
            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer(),
                DataScrambler = new Mock.DummyDataScrambler()
            };
            _dataModel.Compose();

            Guid gUser1 = _dataModel.Credentials.AddNewModel(new CredentialsModel() { Username = "usr1", Password = "pwd1" });
            Guid gUser2 = _dataModel.Credentials.AddNewModel(new CredentialsModel() { Username = "usr2", Password = "pwd2" });
            Guid gUser3 = _dataModel.Credentials.AddNewModel(new CredentialsModel() { Username = "usr3" });

            _dataModel.Gateways.AddNewModel(new GatewayModel() { HostName = "gateway1", CredentialsId = gUser1 });
            _dataModel.Gateways.AddNewModel(new GatewayModel() { HostName = "gateway2", CredentialsId = gUser2 });
            _dataModel.Gateways.AddNewModel(new GatewayModel() { HostName = "gateway3" });
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _dataModel = null;
        }

        [TestMethod]
        public void NewSessionGateway_DefaultConstructor_Modified()
        {
            SessionGateway sc = new SessionGateway();

            Assert.IsNotNull(sc.Credentials);
            Assert.IsNotNull(sc.Gateway);
            Assert.IsTrue(sc.IsNewPassword);
            Assert.IsFalse(sc.HasGateway);
        }

        [TestMethod]
        public void NewSessionGateway_InitConstructor_Unmodified()
        {
            IModelContainer<CredentialsModel> userContainer = _dataModel.Credentials.Models[0];
            IModelContainer<GatewayModel> gatewayContainer = _dataModel.Gateways.Models[0];
            SessionGateway sc = new SessionGateway(gatewayContainer, userContainer);

            Assert.IsNotNull(sc.Credentials);
            Assert.IsFalse(sc.IsNewPassword);
            Assert.AreEqual(sc.Credentials.Username, userContainer.Model.Username);
            Assert.AreEqual(sc.Credentials.Password, userContainer.Model.Password);

            Assert.IsTrue(sc.HasGateway);
            Assert.AreEqual(sc.Gateway.HostName, gatewayContainer.Model.HostName);
            Assert.AreEqual(sc.Gateway.CredentialsId, userContainer.Id);
        }

        [TestMethod]
        public void SessionGateway_ApplyAnotherModel_Unmodified()
        {
            IModelContainer<CredentialsModel> userContainer = _dataModel.Credentials.Models[0];
            IModelContainer<GatewayModel> gatewayContainer = _dataModel.Gateways.Models[0];
            SessionGateway sc = new SessionGateway(gatewayContainer, userContainer);

            userContainer = _dataModel.Credentials.Models[1];
            sc.ApplySavedCredentials(userContainer);

            Assert.IsNotNull(sc.Credentials);
            Assert.IsFalse(sc.IsNewPassword);
            Assert.AreEqual(sc.Credentials.Username, userContainer.Model.Username);
            Assert.AreEqual(sc.Credentials.Password, userContainer.Model.Password);
        }

        [TestMethod]
        public void SessionGateway_ChangePasword_Modified()
        {
            IModelContainer<CredentialsModel> userContainer = _dataModel.Credentials.Models[0];
            IModelContainer<GatewayModel> gatewayContainer = _dataModel.Gateways.Models[0];
            SessionGateway sc = new SessionGateway(gatewayContainer, userContainer);

            sc.Credentials.Password = "newpassword";

            Assert.IsTrue(sc.IsNewPassword);
        }

        [TestMethod]
        public void SessionGateway_ChangeUserName_Modified()
        {
            IModelContainer<CredentialsModel> userContainer = _dataModel.Credentials.Models[0];
            IModelContainer<GatewayModel> gatewayContainer = _dataModel.Gateways.Models[0];
            SessionGateway sc = new SessionGateway(gatewayContainer, userContainer);

            sc.Credentials.Username = "newuser";

            Assert.IsTrue(sc.IsNewPassword);
        }

        [TestMethod]
        public void SessionGateway_ChangePasswordApplyAnotherModel_Unmodified()
        {
            IModelContainer<CredentialsModel> userContainer = _dataModel.Credentials.Models[0];
            IModelContainer<GatewayModel> gatewayContainer = _dataModel.Gateways.Models[0];
            SessionGateway sc = new SessionGateway(gatewayContainer, userContainer);

            sc.Credentials.Password = "buzzword";
            userContainer = _dataModel.Credentials.Models[1];
            sc.ApplySavedCredentials(userContainer);

            Assert.IsNotNull(sc.Credentials);
            Assert.IsFalse(sc.IsNewPassword);
            Assert.AreEqual(sc.Credentials.Username, userContainer.Model.Username);
            Assert.AreEqual(sc.Credentials.Password, userContainer.Model.Password);
        }


        [TestMethod]
        public void SessionGateway_AddUser_Modified()
        {
            IModelContainer<CredentialsModel> userContainer = _dataModel.Credentials.Models[0];
            IModelContainer<GatewayModel> gatewayContainer = _dataModel.Gateways.Models[2];
            SessionGateway sc = new SessionGateway(gatewayContainer, userContainer);

            Assert.IsNotNull(sc.Credentials);
            Assert.IsFalse(sc.IsNewPassword);
            Assert.IsTrue(sc.HasGateway);
            Assert.AreEqual(sc.Gateway.HostName, gatewayContainer.Model.HostName);
            Assert.AreEqual(sc.Gateway.CredentialsId, Guid.Empty);
            Assert.IsFalse(sc.Gateway.HasCredentials);

            userContainer = _dataModel.Credentials.Models[1];
            sc.ApplySavedCredentials(userContainer);

            Assert.IsNotNull(sc.Credentials);
            Assert.IsFalse(sc.IsNewPassword);
            Assert.AreEqual(sc.Credentials.Username, userContainer.Model.Username);
            Assert.AreEqual(sc.Credentials.Password, userContainer.Model.Password);

            sc.SaveCredentials(_dataModel);
            Assert.AreEqual(sc.Gateway.CredentialsId, userContainer.Id);
            Assert.IsTrue(sc.Gateway.HasCredentials);
        }
    }
}
