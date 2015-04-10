using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Converters;
using RdClient.Shared.Helpers;
using RdClient.Shared.Test.UAP;
using System;

namespace RdClient.Shared.Test.Converters
{
    enum TestEnum
    {
        TestValue,
        AnotherTestValue
    }

    public class TestStringLocalizer : IStringTable
    {

        public string GetLocalizedString(string key)
        {
            return key + "loc";
        }
    }

    [TestClass]
    public class TypeToLocalizedStringConverterTests
    {
        

        [TestMethod]
        public void TypeToLocalizedStringConverterTests_Enum()
        {
            TypeToLocalizedStringConverter ttlsc = new TypeToLocalizedStringConverter();
            ttlsc.LocalizedString = new TestStringLocalizer();
            TestEnum te = TestEnum.TestValue;

            Assert.AreEqual("TestEnum_TestValue_Stringloc", ttlsc.Convert(te, typeof(string), null, null));
        }

        [TestMethod]
        public void TypeToLocalizedStringConverterTests_Enum_Invalid()
        {
            TypeToLocalizedStringConverter ttlsc = new TypeToLocalizedStringConverter();
            ttlsc.LocalizedString = new TestStringLocalizer();
            TestEnum te = (TestEnum) 3;

            Assert.AreEqual("TestEnum_3_Stringloc", ttlsc.Convert(te, typeof(string), null, null));
        }

        [TestMethod]
        public void TypeToLocalizedStringConverterTests_ConvertBack()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<NotImplementedException>(() =>
            {
                TypeToLocalizedStringConverter ttlsc = new TypeToLocalizedStringConverter();
                ttlsc.ConvertBack(null, null, null, null);
            }));
        }

        [TestMethod]
        public void ValidStringParameterAppendedToKey()
        {
            TypeToLocalizedStringConverter ttlsc = new TypeToLocalizedStringConverter();
            ttlsc.LocalizedString = new TestStringLocalizer();
            TestEnum te = TestEnum.TestValue;
            string parameter = "ParameterString";
            Assert.AreEqual("TestEnum_TestValue_ParameterString_Stringloc", ttlsc.Convert(te, typeof(string), parameter, null));
        }

        [TestMethod]
        public void NullStringParameterIgnored()
        {
            TypeToLocalizedStringConverter ttlsc = new TypeToLocalizedStringConverter();
            ttlsc.LocalizedString = new TestStringLocalizer();
            TestEnum te = TestEnum.TestValue;
            string parameter = null;
            Assert.AreEqual("TestEnum_TestValue_Stringloc", ttlsc.Convert(te, typeof(string), parameter, null));
        }

        [TestMethod]
        public void NonStringParameterIgnored()
        {
            TypeToLocalizedStringConverter ttlsc = new TypeToLocalizedStringConverter();
            ttlsc.LocalizedString = new TestStringLocalizer();
            TestEnum te = TestEnum.TestValue;
            object parameter = new Object();
            Assert.AreEqual("TestEnum_TestValue_Stringloc", ttlsc.Convert(te, typeof(string), parameter, null));
        }
    }
}
