namespace RdClient.Shared.Test.Data
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System.IO;

    [TestClass]
    public sealed class SerializableModelSerializerTests
    {
        [TestMethod]
        public void SerializableModelSerializer_SerializeLoadCredentialsModel_Loads()
        {
            IDataScrambler scrambler = new Rc4DataScrambler();
            IModelSerializer serializer = new SerializableModelSerializer();
            MemoryStream stream = new MemoryStream();
            CredentialsModel model = new CredentialsModel();
            model.SetScrambler(scrambler);
            model.Username = "User";
            model.Password = "Password";

            serializer.WriteModel(model, stream);
            stream.Seek(0, SeekOrigin.Begin);
            CredentialsModel loadedModel = serializer.ReadModel<CredentialsModel>(stream);
            loadedModel.SetScrambler(scrambler);
            Assert.IsNotNull(loadedModel);
            Assert.AreNotSame(model, loadedModel);
            Assert.AreEqual(model.Username, loadedModel.Username);
            Assert.AreEqual(model.Password, loadedModel.Password);
        }

        [TestMethod]
        public void SerializableModelSerializer_SerializeLoadEmptyPassword_Loads()
        {
            IDataScrambler scrambler = new Rc4DataScrambler();
            IModelSerializer serializer = new SerializableModelSerializer();
            MemoryStream stream = new MemoryStream();
            CredentialsModel model = new CredentialsModel();
            model.SetScrambler(scrambler);
            model.Username = "User";
            model.Password = string.Empty;

            serializer.WriteModel(model, stream);
            stream.Seek(0, SeekOrigin.Begin);
            CredentialsModel loadedModel = serializer.ReadModel<CredentialsModel>(stream);
            loadedModel.SetScrambler(scrambler);
            Assert.IsNotNull(loadedModel);
            Assert.AreNotSame(model, loadedModel);
            Assert.AreEqual(model.Username, loadedModel.Username);
            Assert.AreEqual(model.Password, loadedModel.Password);
        }

        [TestMethod]
        public void SerializableModelSerializer_SerializeLoadNullPassword_Loads()
        {
            IDataScrambler scrambler = new Rc4DataScrambler();
            IModelSerializer serializer = new SerializableModelSerializer();
            MemoryStream stream = new MemoryStream();
            CredentialsModel model = new CredentialsModel();
            model.SetScrambler(scrambler);
            model.Username = "User";
            model.Password = null;

            serializer.WriteModel(model, stream);
            stream.Seek(0, SeekOrigin.Begin);
            CredentialsModel loadedModel = serializer.ReadModel<CredentialsModel>(stream);
            loadedModel.SetScrambler(scrambler);
            Assert.IsNotNull(loadedModel);
            Assert.AreNotSame(model, loadedModel);
            Assert.AreEqual(model.Username, loadedModel.Username);
            Assert.IsNull(loadedModel.Password);
        }
    }
}
