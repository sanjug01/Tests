using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Converters;
using System;
using Windows.UI.Xaml;

namespace RdClient.Shared.Test.Converters
{
    [TestClass]
    public class StringLengthToInvisibilityConverterTests
    {
        [TestMethod]
        public void StringLengthToInvisibilityConverter_ConvertCollapsed()
        {
            string str = "hello world";
            StringLengthToInvisibilityConverter sltic = new StringLengthToInvisibilityConverter();
            Visibility visibility = (Visibility) sltic.Convert(str, typeof(Visibility), null, null);

            Assert.AreEqual(Visibility.Collapsed, visibility);
        }

        [TestMethod]
        public void StringLengthToInvisibilityConverter_ConvertVisible()
        {
            string str = null;
            StringLengthToInvisibilityConverter sltic = new StringLengthToInvisibilityConverter();
            Visibility visibility = (Visibility)sltic.Convert(str, typeof(Visibility), null, null);

            Assert.AreEqual(Visibility.Visible, visibility);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void StringLengthToInvisibilityConverter_ConvertBack()
        {
            StringLengthToInvisibilityConverter sltic = new StringLengthToInvisibilityConverter();
            sltic.ConvertBack(null, null, null, null);
        }
    }
}
