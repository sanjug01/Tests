namespace RdClient.Shared.Test.ValidationRules
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.ValidationRules;

    [TestClass]
    public class ValidatedPropertyTests
    {
        ValidatedProperty<object> _prop;
        Mock.ValidationRule<object> _rule;

        [TestInitialize]
        public void TestSetup()
        {
            _rule = new Mock.ValidationRule<object>();
            _prop = new ValidatedProperty<object>(_rule);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _rule.Dispose();
        }

        [TestMethod]
        public void InitiallyNullOrEmpty()
        {
            Assert.IsTrue(_prop.State.Status == ValidationResultStatus.NullOrEmpty);
        }

        [TestMethod]
        public void SettingValueSetsStateToValidationRuleResult()
        {
            var expectedState = new ValidationResult(ValidationResultStatus.Invalid);
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
            var value = new object();

            _rule.Expect("Validate", p => null);
            _prop.Value = value;

            _rule.Expect("Validate", p => null);
            _prop.Value = value;
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
