using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Converters;
using System;

namespace RdClient.Shared.Test.Converters
{
    [TestClass]
    public class ByteArrayToStringConverterTests
    {
        [TestMethod]
        public void ByteArrayToStringConverterTests_Convert()
        {
            ByteArrayToStringConverter batsc = new ByteArrayToStringConverter();
            byte[] array = { 0, 1, 2, 3 };
            string converted = batsc.Convert(array, typeof(string), null, null) as string;

            Assert.AreEqual("00-01-02-03", converted);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ByteArrayToStringConverterTests_ConvertBack()
        {
            ByteArrayToStringConverter batsc = new ByteArrayToStringConverter();
            batsc.ConvertBack(null, null, null, null);
        }

    }
}
