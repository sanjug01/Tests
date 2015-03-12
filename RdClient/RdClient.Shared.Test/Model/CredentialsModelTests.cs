namespace RdClient.Shared.Test.Model
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Models;

    [TestClass]
    public sealed class CredentialsModelTests
    {
        [TestMethod]
        public void CredentialsModel_CopyTo_Copied()
        {
            CredentialsModel
                model = new CredentialsModel(),
                copy = new CredentialsModel();

            model.Username = "user";
            model.Password = "password";
            model.CopyTo(copy);

            Assert.AreNotSame(model, copy);
            Assert.AreEqual(model.Username, copy.Username);
            Assert.AreEqual(model.Password, copy.Password);
        }
    }
}
