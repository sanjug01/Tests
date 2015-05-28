namespace RdClient.Shared.Test.ValidationRules
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using RdClient.Shared.Test.Data;
    using RdClient.Shared.Test.Helpers;
    using RdClient.Shared.ValidationRules;
    using System;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    [TestClass]
    public class NotDuplicateValidationRuleTests
    {
        class TModel1 : IPersistentStatus
        {
            PersistentStatus IPersistentStatus.Status { get { throw new NotImplementedException(); } }
            public event PropertyChangedEventHandler PropertyChanged;

            void IPersistentStatus.SetClean() { throw new NotImplementedException(); }

            public string ItemName { get; set; }
        }

        class TestComparer : IModelEqualityComparer<TModel1, string>
        {
            bool _result;
            public TestComparer(bool result)
            {
                _result = result;
            }

            bool IModelEqualityComparer<TModel1, string>.Equals(TModel1 x, string y)
            {
                return _result;
            }
        }

        class TestNameComparer : IModelEqualityComparer<TModel1, string>
        {
            bool IModelEqualityComparer<TModel1, string>.Equals(TModel1 x, string y)
            {
                return y.Equals(x.ItemName);
            }
        }

        private TestData _testData;
        private TestComparer _alwaysEqualComparer;
        private TestComparer _neverEqualComparer;
        private object _invalidResult;
        private IModelCollection<TModel1> _testCollection;        
        private Guid _testGuid1, _testGuid2;
        private string _testName1, _testName2;
        private TModel1 _testItem;
        private string _testValue;

        NotDuplicateValidationRule<TModel1> _ruleEquals;
        NotDuplicateValidationRule<TModel1> _ruleNotEquals;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            IStorageFolder emptyFolder = new MemoryStorageFolder();
            IModelSerializer serializer = new TestModelSerializer();
            
            _alwaysEqualComparer = new TestComparer(true);
            _neverEqualComparer = new TestComparer(false);
            _invalidResult = "invalid_result";

            int cnt = _testData.RandomSource.Next(3, 20);

            _testCollection = PrimaryModelCollection<TModel1>.Load(emptyFolder, serializer);

            Guid id;
            for (int i = 0; i < cnt; ++i)
            {
                id = _testCollection.AddNewModel(new TModel1() { ItemName = "name" + i } );
            }

            _testGuid1 = _testCollection.Models[0].Id;
            _testName1 = _testCollection.Models[0].Model.ItemName;
            _testItem = _testCollection.Models[0].Model;
            _testGuid2 = _testCollection.Models[cnt - 1].Id;
            _testName2 = _testCollection.Models[cnt - 1].Model.ItemName;

            _testValue = _testData.NewRandomString();

            _ruleEquals = new NotDuplicateValidationRule<TModel1>(_testCollection, _alwaysEqualComparer, _invalidResult);
            _ruleNotEquals = new NotDuplicateValidationRule<TModel1>(_testCollection, _neverEqualComparer, _invalidResult);

        }

        [TestMethod]
        public void ReturnsValidForNonDuplicateItem()
        {
            //validation of non-duplicate itemName should return valid
            Assert.IsTrue(_ruleNotEquals.Validate(_testValue).Status == ValidationResultStatus.Valid);
        }

        [TestMethod]
        public void ReturnsNullOrEmptyForEmptyItemName()
        {
            Assert.IsTrue(_ruleNotEquals.Validate("").Status == ValidationResultStatus.Empty);
            Assert.IsTrue(_ruleEquals.Validate("").Status == ValidationResultStatus.Empty);
        }

        [TestMethod]
        public void ReturnsNullOrEmptyForNullItemName()
        {
            Assert.IsTrue(_ruleNotEquals.Validate(null).Status == ValidationResultStatus.Empty);
            Assert.IsTrue(_ruleEquals.Validate(null).Status == ValidationResultStatus.Empty);
        }

        [TestMethod]
        public void ReturnsInvalidForDuplicateItemName()
        {
            IValidationResult result = _ruleEquals.Validate(_testValue);
            Assert.IsTrue(result.Status == ValidationResultStatus.Invalid);
            Assert.AreEqual(result.ErrorContent,_invalidResult);
        }


        [TestMethod]
        public void ReturnsValidForDuplicateItemWithSameId()
        {
            NotDuplicateValidationRule<TModel1> rule = 
                new NotDuplicateValidationRule<TModel1>(
                    _testCollection, 
                    _testGuid1, 
                    new TestNameComparer(), 
                    _invalidResult);            

            Assert.IsTrue(rule.Validate(_testName1).Status == ValidationResultStatus.Valid);
        }


        [TestMethod]
        public void ReturnsInvalidForNameOfDifferentItem()
        {
            NotDuplicateValidationRule<TModel1> rule =
                new NotDuplicateValidationRule<TModel1>(
                    _testCollection,
                    _testGuid1,
                    new TestNameComparer(),
                    _invalidResult);

            Assert.IsTrue(rule.Validate(_testName2).Status == ValidationResultStatus.Invalid);
        }

    }
}
