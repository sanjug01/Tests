namespace RdClient.Shared.Test.ValidationRules
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Test.Data;
    using RdClient.Shared.Test.Helpers;
    using RdClient.Shared.ValidationRules;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [TestClass]
    public class NotDuplicateUsernameValidationRuleTests
    {
        private TestData _testData;
        private IModelCollection<CredentialsModel> _credCollection;
        private NotDuplicateUsernameValidationRule _rule;   

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();

            var dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer(),
                DataScrambler = new Mock.DummyDataScrambler()
            };
            dataModel.Compose();
            IList<IModelContainer<CredentialsModel>> creds = _testData.NewSmallListOfCredentials();
            foreach (IModelContainer<CredentialsModel> cred in creds)
            {
                dataModel.Credentials.AddNewModel(cred.Model);
            }

            _credCollection = dataModel.Credentials;

            _rule = new NotDuplicateUsernameValidationRule(_credCollection);
        }

        [TestMethod]
        public void ReturnsValidForNonDuplicateUsername()
        {
            string username;

            //generate a string and ensure it's not a duplicate
            do
            {
                username = _testData.NewRandomString();
            }
            while (_credCollection.Models.Any(c => string.Equals(c.Model.Username, username)));

            //validation of non-duplicate username should return valid
            Assert.IsTrue(_rule.Validate(username).Status == ValidationResultStatus.Valid);
        }

        [TestMethod]
        public void ReturnsValidForDuplicateUsernameWithSameId()
        {
            int randomIndex = _testData.RandomSource.Next(0, _credCollection.Models.Count);
            Guid id = _credCollection.Models[randomIndex].Id;
            _rule = new NotDuplicateUsernameValidationRule(_credCollection, id);
            string username = _credCollection.GetModel(id).Username;

            Assert.IsTrue(_rule.Validate(username).Status == ValidationResultStatus.Valid);
        }

        [TestMethod]
        public void ReturnsNullOrEmptyForEmptyUsername()
        {
            Assert.IsTrue(_rule.Validate("").Status == ValidationResultStatus.NullOrEmpty);
        }

        [TestMethod]
        public void ReturnsNullOrEmptyForNullUsername()
        {
            Assert.IsTrue(_rule.Validate(null).Status == ValidationResultStatus.NullOrEmpty);
        }

        [TestMethod]
        public void ReturnsInvalidForDuplicateUsername()
        {
            int randomIndex = _testData.RandomSource.Next(0, _credCollection.Models.Count);
            string username = _credCollection.Models[randomIndex].Model.Username;

            Assert.IsTrue(_rule.Validate(username).Status == ValidationResultStatus.Invalid);
        }

        [TestMethod]
        public void ReturnsInvalidForUsernameOfDifferentCredential()
        {
            int randomIndex = _testData.RandomSource.Next(0, _credCollection.Models.Count);
            Guid id = _credCollection.Models[randomIndex].Id;
            string otherUsername = _credCollection.Models[(randomIndex + 1) % _credCollection.Models.Count].Model.Username;
            _rule = new NotDuplicateUsernameValidationRule(_credCollection, id);

            Assert.IsTrue(_rule.Validate(otherUsername).Status == ValidationResultStatus.Invalid);
        }

        [TestMethod]
        public void ReturnsInvalidForDuplicateUsernameWithDifferentCase()
        {
            int randomIndex;
            string username;
            do
            {
                randomIndex = _testData.RandomSource.Next(0, _credCollection.Models.Count);
                username = _credCollection.Models[randomIndex].Model.Username;
            }
            while (!username.Any(c => char.IsLetter(c))); //pick a random username that contains a letter

            //Create a new username by switching the case of all letters
            StringBuilder sb = new StringBuilder();
            foreach (char c in username)
            {
                char newC = c;
                if (char.IsLetter(c))
                {
                    if (char.IsUpper(c))
                    {
                        newC = char.ToLower(c);
                    }
                    else
                    {
                        newC = char.ToUpper(c);
                    }
                }
                sb.Append(newC);
            }
            string newUsername = sb.ToString();

            //username with only switched case is considered duplicate and should return invalid
            Assert.IsTrue(_rule.Validate(newUsername).Status == ValidationResultStatus.Invalid);
        }
    }
}
