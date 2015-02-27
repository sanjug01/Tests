using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Converters;
using Windows.UI.Xaml;

namespace RdClient.Shared.Test
{
    [TestClass]
    public class ByteArrayToStringConverterTests
    {
        [TestMethod]
        public void ByteArrayToString_ForwardEmptyArray()
        {
            ByteArrayToStringConverter converter = new ByteArrayToStringConverter();
            byte[] value = new byte[0];
            string outString;

            outString = (string)converter.Convert(value, typeof(string), null, "");
            Assert.AreEqual(string.Empty, outString);
        }

        [TestMethod]
        public void ByteArrayToString_ForwardNullArray()
        {
            ByteArrayToStringConverter converter = new ByteArrayToStringConverter();
            byte[] value = new byte[0];
            string outString;

            outString = (string)converter.Convert(value, typeof(string), null, "");
            Assert.AreEqual(string.Empty, outString);
        }


        [TestMethod]
        public void ByteArrayToString_ForwardNonEmpty()
        {
            ByteArrayToStringConverter converter = new ByteArrayToStringConverter();
            byte[] value = new byte[0];
            string outString;

            outString = (string)converter.Convert(value, typeof(string), null, "");
            Assert.AreEqual(string.Empty, outString);
        }

        [TestMethod]
        public void ByteArrayToString_BackNotImplemented()
        {
            ByteArrayToStringConverter converter = new ByteArrayToStringConverter();
            byte[] value ;
            string outString = "AbC";

            try 
            {
                value = (byte[]) converter.ConvertBack(outString, typeof(string), null, "");
                Assert.Fail();
            }
            catch(System.NotImplementedException notImplementedExc)
            {
                Assert.IsNotNull(notImplementedExc);
            }
        }
    }
}
