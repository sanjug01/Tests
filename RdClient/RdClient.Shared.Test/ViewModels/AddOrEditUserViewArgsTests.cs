using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class AddOrEditUserViewArgsTests
    {
        private TestData _testData;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
        }

        [TestMethod]
        public void AddUserSetsParametersCorrectly()
        {
            var args = AddOrEditUserViewArgs.AddUser();
            Assert.IsNotNull(args);
            Assert.IsFalse(args.CanDelete);
            Assert.IsNotNull(args.Credentials);
            Assert.AreEqual(Guid.Empty, args.Credentials.Id);
            Assert.AreEqual(string.Empty, args.Credentials.Model.Username);
            Assert.AreEqual(string.Empty, args.Credentials.Model.Password);
            Assert.AreEqual(CredentialPromptMode.EnterCredentials, args.Mode);
            Assert.IsTrue(args.Save);
            Assert.IsFalse(args.ShowMessage);
            Assert.IsFalse(args.ShowSave);
        }

        [TestMethod]
        public void EditUserSetsParametersCorrectly()
        {
            var user = _testData.NewValidCredential();
            var args = AddOrEditUserViewArgs.EditUser(user);
            Assert.IsNotNull(args);
            Assert.IsTrue(args.CanDelete);
            Assert.AreEqual(user, args.Credentials);
            Assert.AreEqual(CredentialPromptMode.EditCredentials, args.Mode);
            Assert.IsFalse(args.Save);
            Assert.IsFalse(args.ShowMessage);
            Assert.IsFalse(args.ShowSave);
        }
    }
}
