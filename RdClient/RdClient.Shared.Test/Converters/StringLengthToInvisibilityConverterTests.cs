using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Converters;
using Windows.UI.Xaml;

namespace RdClient.Shared.Test
{
    [TestClass]
    public class StringLengthToInvisibilityConverterTests
    {
        [TestMethod]
        public void ForwardEmptyString()
        {
            StringLengthToInvisibilityConverter converter = new StringLengthToInvisibilityConverter();
            string value = "";
            Visibility visibility;

            visibility = (Visibility)converter.Convert(value, typeof(Visibility), null, "");
            Assert.AreEqual(Visibility.Visible, visibility);
        }

        [TestMethod]
        public void ForwardNullString()
        {
            StringLengthToInvisibilityConverter converter = new StringLengthToInvisibilityConverter();
            string value = null;
            Visibility visibility;

            visibility = (Visibility)converter.Convert(value, typeof(Visibility), null, "");
            Assert.AreEqual(Visibility.Visible, visibility);
        }


        [TestMethod]
        public void ForwardNonEmpty()
        {
            StringLengthToInvisibilityConverter converter = new StringLengthToInvisibilityConverter();
            string value = "12Ab";
            Visibility visibility;

            visibility = (Visibility)converter.Convert(value, typeof(Visibility), null, "");
            Assert.AreEqual(Visibility.Collapsed, visibility);
        }

        [TestMethod]
        public void BackVisible_VerifyNotImplemented()
        {
            StringLengthToInvisibilityConverter converter = new StringLengthToInvisibilityConverter();
            string value;
            Visibility visibility = Visibility.Visible;

            try 
            { 
                value = (string)converter.ConvertBack(visibility, typeof(string), null, "");
                Assert.Fail();
            }
            catch(System.NotImplementedException notImplementedExc)
            {
                Assert.IsNotNull(notImplementedExc);
            }
        }

        [TestMethod]
        public void BackCollapsed_VerifyNotImplemented()
        {
            StringLengthToInvisibilityConverter converter = new StringLengthToInvisibilityConverter();
            string value;
            Visibility visibility = Visibility.Collapsed;

            try
            {
                value = (string)converter.ConvertBack(visibility, typeof(string), null, "");
                Assert.Fail();
            }
            catch (System.NotImplementedException notImplementedExc)
            {
                Assert.IsNotNull(notImplementedExc);
            }


        }
    }
}
