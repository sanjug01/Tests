namespace RdClient.Shared.Test.ValidationRules
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Test.Helpers;
    using RdClient.Shared.ValidationRules;
    using System;
    using System.Collections.Generic;

    [TestClass]
    public class CompositeValidationRuleTests
    {
        TestData _testData;
        List<Mock.ValidationRule<object>> _rules;
        CompositeValidationRule<object> _underTest;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _rules = new List<Mock.ValidationRule<object>>();
            //Create a small amount of rules
            int numRules = _testData.RandomSource.Next(3, 11);
            for (int i = 0; i < numRules; i++)
            {
                _rules.Add(new Mock.ValidationRule<object>());
            }
            _underTest = new CompositeValidationRule<object>(_rules);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            foreach(var rule in _rules)
            {
                rule.Dispose();
            }
        }

        [TestMethod]
        public void CreatingWithNullRulesThrows()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _underTest = new CompositeValidationRule<object>(null));
        }

        [TestMethod]
        public void ValidResultReturnedIfNoValidationRulesExist()
        {
            var emptyList = new List<IValidationRule<object>>();
            _underTest = new CompositeValidationRule<object>(emptyList);
            Assert.IsTrue(_underTest.Validate(new object()).Status == ValidationResultStatus.Valid);
        }

        [TestMethod]
        public void TrueReturnedIfAllValidationRulesPass()
        {
            foreach (var rule in _rules)
            {
                rule.Expect("Validate", p => ValidationResult.Valid());
            }
            Assert.IsTrue(_underTest.Validate(new object()).Status == ValidationResultStatus.Valid);
        }

        [TestMethod]
        public void CorrectObjectPassedToAllValidationRules()
        {
            var passedObject = new object();
            foreach (var rule in _rules)
            {                
                rule.Expect("Validate", p =>
                {
                    Assert.AreEqual(passedObject, p[0]);//we should receive the passed object
                    return ValidationResult.Valid();//return Valid result so all rules will be validated
                });
            }
            _underTest.Validate(passedObject);
        }

        [TestMethod]
        public void FirstValidationFailureReturned()
        {
            int failureIndex = _testData.RandomSource.Next(_rules.Count);
            var failureResult = ValidationResult.Invalid();

            //All the passed validation rules (rules after failed one shouldn't be called)
            for (int i = 0; i < failureIndex; i++)
            {
                _rules[i].Expect("Validate", p => ValidationResult.Valid());
            }
            //failing falidation rule
            _rules[failureIndex].Expect("Validate", p => failureResult);

            //check the correct result is returned
            Assert.AreEqual(failureResult, _underTest.Validate(new object()));
        }
    }
}
