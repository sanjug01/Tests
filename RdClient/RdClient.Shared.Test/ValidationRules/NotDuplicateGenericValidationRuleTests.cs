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
    using System.Reflection;

    [TestClass]
    public class NotDuplicateGenericValidationRuleTests
    {
        private TestData _testData;
        private IModelCollection<GatewayModel> _gatewaysCollection;
        private IModelCollection<CredentialsModel> _usersCollection;
        private NotDuplicateValidationRule<GatewayModel>  _ruleGateways;
        private NotDuplicateValidationRule<CredentialsModel> _ruleUsers;
        private CredentialsEqualityComparer _userComparer;
        private GatewayEqualityComparer _gatewayComparer;

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

            IList<IModelContainer<GatewayModel>> gateways = _testData.NewSmallListOfGateways();
            foreach (IModelContainer<GatewayModel> gateway in gateways)
            {
                dataModel.Gateways.AddNewModel(gateway.Model);
            }
            _gatewaysCollection = dataModel.Gateways;
            _gatewayComparer = new GatewayEqualityComparer();

            IList<IModelContainer<CredentialsModel>> users = _testData.NewSmallListOfCredentials();
            foreach (IModelContainer<CredentialsModel> user in users)
            {
                dataModel.Credentials.AddNewModel(user.Model);

            }
            _usersCollection = dataModel.Credentials;
            _userComparer = new CredentialsEqualityComparer();

            _ruleGateways = new NotDuplicateValidationRule<GatewayModel>(_gatewaysCollection, _gatewayComparer, HostnameValidationFailure.DuplicateGateway);
            _ruleUsers = new NotDuplicateValidationRule<CredentialsModel>(_usersCollection, _userComparer, HostnameValidationFailure.DuplicateGateway);
        }

        [TestMethod]
        public void ReturnsValidForNonDuplicateItem()
        {
            string itemName="abc";

            //validation of non-duplicate itemName should return valid
            Assert.IsTrue(_ruleUsers.Validate(itemName).Status == ValidationResultStatus.Valid);
            Assert.IsTrue(_ruleGateways.Validate(itemName).Status == ValidationResultStatus.Valid);
        }

        [TestMethod]
        public void ReturnsNullOrEmptyForEmptyItemName()
        {
            Assert.IsTrue(_ruleGateways.Validate("").Status == ValidationResultStatus.Empty);
            Assert.IsTrue(_ruleUsers.Validate("").Status == ValidationResultStatus.Empty);
        }

        [TestMethod]
        public void ReturnsNullOrEmptyForNullItemName()
        {
            Assert.IsTrue(_ruleGateways.Validate(null).Status == ValidationResultStatus.Empty);
            Assert.IsTrue(_ruleUsers.Validate(null).Status == ValidationResultStatus.Empty);
        }

        [TestMethod]
        public void ReturnsInvalidForDuplicateItemName()
        {
            int randomIndex;
            string itemName;

            randomIndex = _testData.RandomSource.Next(0, _gatewaysCollection.Models.Count);
            itemName = _gatewaysCollection.Models[randomIndex].Model.HostName;
            Assert.IsTrue(_ruleGateways.Validate(itemName).Status == ValidationResultStatus.Invalid);

            randomIndex = _testData.RandomSource.Next(0, _usersCollection.Models.Count);
            itemName = _usersCollection.Models[randomIndex].Model.Username;
            Assert.IsTrue(_ruleUsers.Validate(itemName).Status == ValidationResultStatus.Invalid);
        }


        [TestMethod]
        public void ReturnsValidForDuplicateGatewayWithSameId()
        {
            int randomIndex = _testData.RandomSource.Next(0, _gatewaysCollection.Models.Count);
            Guid id = _gatewaysCollection.Models[randomIndex].Id;
            _ruleGateways = new NotDuplicateValidationRule<GatewayModel>(_gatewaysCollection, id, _gatewayComparer, HostnameValidationFailure.DuplicateGateway);
            string itemName = _gatewaysCollection.GetModel(id).HostName;

            Assert.IsTrue(_ruleGateways.Validate(itemName).Status == ValidationResultStatus.Valid);
        }

        [TestMethod]
        public void ReturnsValidForDuplicateUserWithSameId()
        {
            int randomIndex = _testData.RandomSource.Next(0, _usersCollection.Models.Count);
            Guid id = _usersCollection.Models[randomIndex].Id;
            _ruleUsers = new NotDuplicateValidationRule<CredentialsModel>(_usersCollection, id, _userComparer, UsernameValidationFailure.Duplicate);
            string itemName = _usersCollection.GetModel(id).Username;

            Assert.IsTrue(_ruleUsers.Validate(itemName).Status == ValidationResultStatus.Valid);
        }

        [TestMethod]
        public void ReturnsInvalidForNameOfDifferentGateway()
        {
            int randomIndex = _testData.RandomSource.Next(0, _gatewaysCollection.Models.Count);
            Guid id = _gatewaysCollection.Models[randomIndex].Id;
            string otherItemName = _gatewaysCollection.Models[(randomIndex + 1) % _gatewaysCollection.Models.Count].Model.HostName;
            _ruleGateways = new NotDuplicateValidationRule<GatewayModel>(_gatewaysCollection, id, _gatewayComparer, HostnameValidationFailure.DuplicateGateway);

            Assert.IsTrue(_ruleGateways.Validate(otherItemName).Status == ValidationResultStatus.Invalid);
        }

        [TestMethod]
        public void ReturnsInvalidForNameOfDifferentUser()
        {
            int randomIndex = _testData.RandomSource.Next(0, _usersCollection.Models.Count);
            Guid id = _usersCollection.Models[randomIndex].Id;
            string otherItemName = _usersCollection.Models[(randomIndex + 1) % _usersCollection.Models.Count].Model.Username;
            _ruleUsers = new NotDuplicateValidationRule<CredentialsModel>(_usersCollection, id, _userComparer, HostnameValidationFailure.DuplicateGateway);

            Assert.IsTrue(_ruleUsers.Validate(otherItemName).Status == ValidationResultStatus.Invalid);
        }

        [TestMethod]
        public void ReturnsInvalidForDuplicateGatewayNameWithDifferentCase()
        {
            int randomIndex;
            string itemName;
            do
            {
                randomIndex = _testData.RandomSource.Next(0, _gatewaysCollection.Models.Count);
                itemName = _gatewaysCollection.Models[randomIndex].Model.HostName;
            }
            while (!itemName.Any(c => char.IsLetter(c))); //pick a random itemName that contains a letter

            //itemName with only switched case is considered duplicate and should return invalid
            string newItemName = ChangeCase(itemName);
            Assert.IsTrue(_ruleGateways.Validate(newItemName).Status == ValidationResultStatus.Invalid);
        }

        [TestMethod]
        public void ReturnsInvalidForDuplicateUserNameWithDifferentCase()
        {
            int randomIndex;
            string itemName;
            do
            {
                randomIndex = _testData.RandomSource.Next(0, _usersCollection.Models.Count);
                itemName = _usersCollection.Models[randomIndex].Model.Username;
            }
            while (!itemName.Any(c => char.IsLetter(c))); //pick a random itemName that contains a letter

            //itemName with only switched case is considered duplicate and should return invalid
            string newItemName = ChangeCase(itemName);
            Assert.IsTrue(_ruleUsers.Validate(newItemName).Status == ValidationResultStatus.Invalid);
        }

        // Create a new itemName by switching the case of all letters
        private string ChangeCase(string itemName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in itemName)
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

            return sb.ToString();
        }
    }
}
