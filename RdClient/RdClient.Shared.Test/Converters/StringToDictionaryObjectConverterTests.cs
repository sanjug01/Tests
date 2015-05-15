namespace RdClient.Shared.Test.Converters
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Converters;
    using System;
    using System.Collections.Generic;

    [TestClass]
    public sealed class StringToDictionaryObjectConverterTests
    {
        private enum Keys
        {
            DemoKey,
            AnotherKey
        }

        private interface IBody { }
        private class Body : IBody { }
        private sealed class Superbody : Body { }
        private class Unrelated { }

        [TestMethod]
        public void NewConverter_CorrectDefaultProperties()
        {
            using(StringToDictionaryObjectConverter converter = new StringToDictionaryObjectConverter())
            {
                Assert.IsNotNull(converter.Dictionary);
                Assert.IsNull(converter.DefaultObject);
            }
        }

        [TestMethod]
        public void NewConverter_ChangeDefaultObject_ChangeReported()
        {
            using (StringToDictionaryObjectConverter converter = new StringToDictionaryObjectConverter())
            {
                IList<string> reportedProperties = new List<string>();
                const string TestValue = "DefaultObject";

                converter.PropertyChanged += (s, e) => reportedProperties.Add(e.PropertyName);
                converter.DefaultObject = TestValue;
                Assert.AreEqual(1, reportedProperties.Count);
                Assert.AreEqual(TestValue, reportedProperties[0]);
                Assert.AreEqual(TestValue, converter.DefaultObject);
            }
        }

        [TestMethod]
        public void NewConverter_ChangeDictionary_ChangeReported()
        {
            using (StringToDictionaryObjectConverter converter = new StringToDictionaryObjectConverter())
            {
                IList<string> reportedProperties = new List<string>();
                Dictionary<string, object> dict = new Dictionary<string, object>();

                converter.PropertyChanged += (s, e) => reportedProperties.Add(e.PropertyName);
                converter.Dictionary = dict;
                Assert.AreEqual(1, reportedProperties.Count);
                Assert.AreEqual("Dictionary", reportedProperties[0]);
                Assert.AreSame(dict, converter.Dictionary);
            }
        }

        [TestMethod]
        public void ConvertExistingValue_CorrectValueReturned()
        {
            using (StringToDictionaryObjectConverter converter = new StringToDictionaryObjectConverter())
            {
                object demoObject = new object();

                converter.Dictionary.Add("DemoKey", demoObject);
                Assert.AreSame(demoObject, converter.Convert(Keys.DemoKey, typeof(object), null, null));
            }
        }

        [TestMethod]
        public void ConvertNonExistingValue_ThrowsKeyNotFoundException()
        {
            using (StringToDictionaryObjectConverter converter = new StringToDictionaryObjectConverter())
            {
                object demoObject = new Body();

                converter.Dictionary.Add("DemoKey", demoObject);
                try
                {
                    converter.Convert(Keys.AnotherKey, typeof(Body), null, null);
                    Assert.Fail("Unexpected success");
                }
                catch(KeyNotFoundException)
                {
                    // Success
                }
                catch(Exception ex)
                {
                    Assert.Fail(string.Format("Unexpected exception {0}", ex.GetType().Name));
                }
            }
        }

        [TestMethod]
        public void ConvertNullValue_ThrowsKeyNotFoundException()
        {
            using (StringToDictionaryObjectConverter converter = new StringToDictionaryObjectConverter())
            {
                object demoObject = new Body();

                converter.Dictionary.Add("DemoKey", demoObject);
                try
                {
                    converter.Convert(null, typeof(Body), null, null);
                    Assert.Fail("Unexpected success");
                }
                catch (KeyNotFoundException)
                {
                    // Success
                }
                catch (Exception ex)
                {
                    Assert.Fail(string.Format("Unexpected exception {0}", ex.GetType().Name));
                }
            }
        }

        [TestMethod]
        public void SetDefault_ConvertNonExistingValue_ReturnsDefaultObject()
        {
            using (StringToDictionaryObjectConverter converter = new StringToDictionaryObjectConverter())
            {
                object defaultObject = new Body();

                converter.Dictionary.Add("DemoKey", defaultObject);
                converter.DefaultObject = "DemoKey";
                Assert.AreSame(defaultObject, converter.Convert(Keys.AnotherKey, typeof(Body), null, null));
            }
        }

        [TestMethod]
        public void SetDefault_ConvertNullValue_ReturnsDefaultObject()
        {
            using (StringToDictionaryObjectConverter converter = new StringToDictionaryObjectConverter())
            {
                object defaultObject = new Body();

                converter.Dictionary.Add("DemoKey", defaultObject);
                converter.DefaultObject = "DemoKey";
                Assert.AreSame(defaultObject, converter.Convert(null, typeof(Body), null, null));
            }
        }

        [TestMethod]
        public void ConvertToInterface_Converts()
        {
            using (StringToDictionaryObjectConverter converter = new StringToDictionaryObjectConverter())
            {
                converter.Dictionary.Add("DemoKey", new Body());
                Assert.IsInstanceOfType(converter.Convert(Keys.DemoKey, typeof(IBody), null, null), typeof(IBody));
            }
        }

        [TestMethod]
        public void ConvertToBaseClass_Converts()
        {
            using (StringToDictionaryObjectConverter converter = new StringToDictionaryObjectConverter())
            {
                converter.Dictionary.Add("DemoKey", new Superbody());
                Assert.IsInstanceOfType(converter.Convert(Keys.DemoKey, typeof(Body), null, null), typeof(Body));
            }
        }

        [TestMethod]
        public void ConvertSuperclassToInterface_Converts()
        {
            using (StringToDictionaryObjectConverter converter = new StringToDictionaryObjectConverter())
            {
                converter.Dictionary.Add("DemoKey", new Superbody());
                Assert.IsInstanceOfType(converter.Convert(Keys.DemoKey, typeof(IBody), null, null), typeof(IBody));
            }
        }

        [TestMethod]
        public void ConverToUnrelated_Throws()
        {
            using (StringToDictionaryObjectConverter converter = new StringToDictionaryObjectConverter())
            {
                converter.Dictionary.Add("DemoKey", new Superbody());
                try
                {
                    converter.Convert(Keys.DemoKey, typeof(Unrelated), null, null);
                    Assert.Fail("Unexpected success");
                }
                catch(InvalidCastException)
                {
                    // Success
                }
                catch(Exception ex)
                {
                    Assert.Fail(string.Format("Unexpected exception {0}", ex.GetType().Name));
                }
            }
        }
    }
}
