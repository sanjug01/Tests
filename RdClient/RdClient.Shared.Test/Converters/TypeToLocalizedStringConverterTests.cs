using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.Converters
{
    enum TestEnum
    {
        TestValue,
        AnotherTestValue
    }

    [TestClass]
    public class TypeToLocalizedStringConverterTests
    {
        

        [TestMethod]
        public void TypeToLocalizedStringConverterTests_Enum()
        {
            TypeToLocalizedStringConverter ttlsc = new TypeToLocalizedStringConverter();
            TestEnum te = TestEnum.TestValue;

            Assert.AreEqual("TestEnum_TestValue", ttlsc.Convert(te, typeof(string), null, null));
        }

        [TestMethod]
        public void TypeToLocalizedStringConverterTests_Enum_Invalid()
        {
            TypeToLocalizedStringConverter ttlsc = new TypeToLocalizedStringConverter();
            TestEnum te = (TestEnum) 3;

            Assert.AreEqual("TestEnum_3", ttlsc.Convert(te, typeof(string), null, null));
        }
    }
}
