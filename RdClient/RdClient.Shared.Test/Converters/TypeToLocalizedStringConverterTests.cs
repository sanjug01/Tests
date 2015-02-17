﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Converters;
using RdClient.Shared.Helpers;
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
    }
}
