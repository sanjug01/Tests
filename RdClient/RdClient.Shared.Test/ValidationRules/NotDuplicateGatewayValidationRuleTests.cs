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
    public class NotDuplicateGatewayValidationRuleTests
    {
        private TestData _testData;
        private IModelCollection<GatewayModel> _gatewaysCollection;
        private NotDuplicateGatewayValidationRule _rule;   

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

            _rule = new NotDuplicateGatewayValidationRule(_gatewaysCollection);
        }

        [TestMethod]
        public void ReturnsValidForNonDuplicateGateway()
        {
            string gatewayName;

            //generate a string and ensure it's not a duplicate
            do
            {
                gatewayName = _testData.NewRandomString();
            }
            while (_gatewaysCollection.Models.Any(c => string.Equals(c.Model.HostName, gatewayName)));

            //validation of non-duplicate gatewayName should return valid
            Assert.IsTrue(_rule.Validate(gatewayName).Status == ValidationResultStatus.Valid);
        }

        [TestMethod]
        public void ReturnsValidForDuplicateGatewayNameWithSameId()
        {
            int randomIndex = _testData.RandomSource.Next(0, _gatewaysCollection.Models.Count);
            Guid id = _gatewaysCollection.Models[randomIndex].Id;
            _rule = new NotDuplicateGatewayValidationRule(_gatewaysCollection, id);
            string gatewayName = _gatewaysCollection.GetModel(id).HostName;

            Assert.IsTrue(_rule.Validate(gatewayName).Status == ValidationResultStatus.Valid);
        }

        [TestMethod]
        public void ReturnsNullOrEmptyForEmptyGatewayName()
        {
            Assert.IsTrue(_rule.Validate("").Status == ValidationResultStatus.Empty);
        }

        [TestMethod]
        public void ReturnsNullOrEmptyForNullGatewayName()
        {
            Assert.IsTrue(_rule.Validate(null).Status == ValidationResultStatus.Empty);
        }

        [TestMethod]
        public void ReturnsInvalidForDuplicateGatewayName()
        {
            int randomIndex = _testData.RandomSource.Next(0, _gatewaysCollection.Models.Count);
            string gatewayName = _gatewaysCollection.Models[randomIndex].Model.HostName;

            Assert.IsTrue(_rule.Validate(gatewayName).Status == ValidationResultStatus.Invalid);
        }

        [TestMethod]
        public void ReturnsInvalidForGatewayNameOfDifferentGateway()
        {
            int randomIndex = _testData.RandomSource.Next(0, _gatewaysCollection.Models.Count);
            Guid id = _gatewaysCollection.Models[randomIndex].Id;
            string otherGatewayName = _gatewaysCollection.Models[(randomIndex + 1) % _gatewaysCollection.Models.Count].Model.HostName;
            _rule = new NotDuplicateGatewayValidationRule(_gatewaysCollection, id);

            Assert.IsTrue(_rule.Validate(otherGatewayName).Status == ValidationResultStatus.Invalid);
        }

        [TestMethod]
        public void ReturnsInvalidForDuplicateGatewayNameWithDifferentCase()
        {
            int randomIndex;
            string gatewayName;
            do
            {
                randomIndex = _testData.RandomSource.Next(0, _gatewaysCollection.Models.Count);
                gatewayName = _gatewaysCollection.Models[randomIndex].Model.HostName;
            }
            while (!gatewayName.Any(c => char.IsLetter(c))); //pick a random gatewayName that contains a letter

            //Create a new gatewayName by switching the case of all letters
            StringBuilder sb = new StringBuilder();
            foreach (char c in gatewayName)
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
            string newGatewayName = sb.ToString();

            //gatewayName with only switched case is considered duplicate and should return invalid
            Assert.IsTrue(_rule.Validate(newGatewayName).Status == ValidationResultStatus.Invalid);
        }
    }
}
