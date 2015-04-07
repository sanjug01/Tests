namespace RdClient.Shared.Test.Converters
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Converters;
    using RdClient.Shared.Test.UAP;
    using System;
    using Windows.UI.Xaml.Data;

    [TestClass]
    public sealed class EnumMemberToSymbolStringConverterTests
    {
        private enum TestEnumUShort : ushort
        {
            Value = 100
        }

        private enum TestEnumInt : int
        {
            Value = 0xE710
        }

        [TestMethod]
        public void EnumMemberToSymbolStringConverter_ConvertUShort_Converts()
        {
            IValueConverter converter = new EnumMemberToSymbolStringConverter();

            string s = (string)converter.Convert(TestEnumUShort.Value, typeof(object), null, null);

            Assert.IsNotNull(s);
            Assert.AreEqual(1, s.Length);
            Assert.AreEqual((ushort)TestEnumUShort.Value, (ushort)s[0]);
        }

        [TestMethod]
        public void EnumMemberToSymbolStringConverter_ConvertInt_Converts()
        {
            IValueConverter converter = new EnumMemberToSymbolStringConverter();

            string s = (string)converter.Convert(TestEnumInt.Value, typeof(object), null, null);

            Assert.IsNotNull(s);
            Assert.AreEqual(1, s.Length);
            Assert.AreEqual((int)TestEnumInt.Value, (int)s[0]);
        }

        [TestMethod]
        public void EnumMemberToSymbolStringConverter_ConvertNull_Throws()
        {
            IValueConverter converter = new EnumMemberToSymbolStringConverter();

            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
                converter.Convert(256, typeof(object), null, null)));
        }

        [TestMethod]
        public void EnumMemberToSymbolStringConverter_ConvertNonEnum_Throws()
        {
            IValueConverter converter = new EnumMemberToSymbolStringConverter();

            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
                converter.Convert(256, typeof(object), null, null)));
        }

        [TestMethod]
        public void EnumMemberToSymbolStringConverter_ConvertBack_Throws()
        {
            IValueConverter converter = new EnumMemberToSymbolStringConverter();

            Assert.IsTrue(ExceptionExpecter.ExpectException<NotImplementedException>(() =>
                converter.ConvertBack(TestEnumInt.Value, null, null, null)));
        }
    }
}
