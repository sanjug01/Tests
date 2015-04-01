namespace RdClient.Shared.Test.Data
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using System.IO;

    [TestClass]
    public sealed class SerializableModelSerializerTests
    {
        [TestMethod]
        public void SerializableModelSerializer_SerializeLoadCredentialsModel_Loads()
        {
            IModelSerializer serializer = new SerializableModelSerializer();
            CredentialsModel model = new CredentialsModel() { Username = "User", Password = "Password" };
            MemoryStream stream = new MemoryStream();

            serializer.WriteModel(model, stream);
            stream.Seek(0, SeekOrigin.Begin);
            CredentialsModel loadedModel = serializer.ReadModel<CredentialsModel>(stream);
            Assert.IsNotNull(loadedModel);
            Assert.AreNotSame(model, loadedModel);
            Assert.AreEqual(model.Username, loadedModel.Username);
            Assert.AreEqual(model.Password, loadedModel.Password);
        }
    }
}
