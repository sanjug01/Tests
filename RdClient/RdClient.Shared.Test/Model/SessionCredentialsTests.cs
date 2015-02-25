namespace RdClient.Shared.Test.Model
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Test.Data;

    [TestClass]
    public sealed class SessionCredentialsTests
    {
        private ApplicationDataModel _dataModel;

        [TestInitialize]
        public void SetUpTest()
        {
            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer()
            };
            _dataModel.LocalWorkspace.Credentials.AddNewModel(new CredentialsModel() { Username = "usr1", Password = "pwd1" });
            _dataModel.LocalWorkspace.Credentials.AddNewModel(new CredentialsModel() { Username = "usr2", Password = "pwd2" });
            _dataModel.LocalWorkspace.Credentials.AddNewModel(new CredentialsModel() { Username = "usr3" });
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _dataModel = null;
        }

        [TestMethod]
        public void NewSessionCredentials_DefaultConstructor_Modified()
        {
            SessionCredentials sc = new SessionCredentials();

            Assert.IsNotNull(sc.Credentials);
            Assert.IsTrue(sc.IsNewPassword);
        }

        [TestMethod]
        public void NewSessionCredentials_InitConstructor_Unmodified()
        {
            IModelContainer<CredentialsModel> container = _dataModel.LocalWorkspace.Credentials.Models[0];
            SessionCredentials sc = new SessionCredentials(container);

            Assert.IsNotNull(sc.Credentials);
            Assert.IsFalse(sc.IsNewPassword);
            Assert.AreEqual(sc.Credentials.Username, container.Model.Username);
            Assert.AreEqual(sc.Credentials.Password, container.Model.Password);
        }

        [TestMethod]
        public void SessionCredentials_ApplyAnotherModel_Unmodified()
        {
            IModelContainer<CredentialsModel> container = _dataModel.LocalWorkspace.Credentials.Models[0];
            SessionCredentials sc = new SessionCredentials(container);

            container = _dataModel.LocalWorkspace.Credentials.Models[1];
            sc.ApplySavedCredentials(container);

            Assert.IsNotNull(sc.Credentials);
            Assert.IsFalse(sc.IsNewPassword);
            Assert.AreEqual(sc.Credentials.Username, container.Model.Username);
            Assert.AreEqual(sc.Credentials.Password, container.Model.Password);
        }

        [TestMethod]
        public void SessionCredentials_ChangePasword_Modified()
        {
            IModelContainer<CredentialsModel> container = _dataModel.LocalWorkspace.Credentials.Models[0];
            SessionCredentials sc = new SessionCredentials(container);

            sc.Credentials.Password = "newpassword";

            Assert.IsTrue(sc.IsNewPassword);
        }

        [TestMethod]
        public void SessionCredentials_ChangeUserName_Modified()
        {
            IModelContainer<CredentialsModel> container = _dataModel.LocalWorkspace.Credentials.Models[0];
            SessionCredentials sc = new SessionCredentials(container);

            sc.Credentials.Username = "newuser";

            Assert.IsTrue(sc.IsNewPassword);
        }

        [TestMethod]
        public void SessionCredentials_ChangePasswordApplyAnotherModel_Unmodified()
        {
            IModelContainer<CredentialsModel> container = _dataModel.LocalWorkspace.Credentials.Models[0];
            SessionCredentials sc = new SessionCredentials(container);

            sc.Credentials.Password = "buzzword";
            container = _dataModel.LocalWorkspace.Credentials.Models[1];
            sc.ApplySavedCredentials(container);

            Assert.IsNotNull(sc.Credentials);
            Assert.IsFalse(sc.IsNewPassword);
            Assert.AreEqual(sc.Credentials.Username, container.Model.Username);
            Assert.AreEqual(sc.Credentials.Password, container.Model.Password);
        }
    }
}
