namespace RdClient.Shared.Test.ValidationRules
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.ValidationRules;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestClass]
    public class ValidatedPropertyTests
    {
        ValidatedProperty<object> _prop;
        Mock.ValidationRule<object> _rule;
        object _initialValue;

        [TestInitialize]
        public void TestSetup()
        {
            _initialValue = new object();
            _rule = new Mock.ValidationRule<object>();
            _prop = new ValidatedProperty<object>(_rule, _initialValue);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _rule.Dispose();
        }

        [TestMethod]
        public void ValueSetToInitialValue()
        {
            Assert.AreEqual(_initialValue, _prop.Value);
        }

        [TestMethod]
        public void InitiallyValid()
        {
            Assert.IsTrue(_prop.State.IsValid);
        }

        [TestMethod]
        public void SettingValueSetsStateToValidationRuleResult()
        {
            var expectedState = new ValidationResult(false);
            var setValue = new object();
            _rule.Expect("Validate", p =>
            {
                Assert.AreEqual(setValue, p[0]);
                return expectedState;
            });
            _prop.Value = setValue;
            Assert.AreEqual(expectedState, _prop.State);
        }

        [TestMethod]
        public void SettingValueToTheSameValueValidates()
        {
            _rule.Expect("Validate", p => null);
            _prop.Value = _initialValue;
        }

        [TestMethod]
        public void CallingValidateSetsStateToValidationRuleResult()
        {
            var expectedState = new ValidationResult(false);
            _rule.Expect("Validate", p =>
            {
                Assert.AreEqual(_initialValue, p[0]);
                return expectedState;
            });
            _prop.ValidateNow();
            Assert.AreEqual(expectedState, _prop.State);
        }

        [TestMethod]
        public void PropertyChangedEventsFiredWhenCallingValidate()
        {
            int stateChangedEvents = 0;
            _prop.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName.Equals("State"))                
                    stateChangedEvents++;                
            };
            _rule.Expect("Validate", p => null);
            _prop.ValidateNow();
            Assert.IsTrue(stateChangedEvents == 1);
        }

        [TestMethod]
        public void PropertyChangedEventsFiredWhenValueSet()
        {
            int stateChangedEvents = 0;
            int valueChangedEvents = 0;
            _prop.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName.Equals("State"))
                    stateChangedEvents++;
                if (e.PropertyName.Equals("Value"))
                    valueChangedEvents++;
            };
            _rule.Expect("Validate", p => null);
            _prop.Value = new object();
            Assert.IsTrue(valueChangedEvents == 1);
            Assert.IsTrue(stateChangedEvents == 1);
        }

    }
}
