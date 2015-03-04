using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Converters;
using System;
using Windows.UI.Xaml;

namespace RdClient.Shared.Test.Converters
{
    [TestClass]
    public class StringLengthToVisbilityConverter
    {
        [TestMethod]
        public void StringLengthToInvisibilityConverter_ConvertVisible()
        {
            string str = "hello world";
            StringLengthToVisibilityConverter sltic = new StringLengthToVisibilityConverter();
            Visibility visibility = (Visibility)sltic.Convert(str, typeof(Visibility), null, null);

            Assert.AreEqual(Visibility.Visible, visibility);
        }

        [TestMethod]
        public void StringLengthToInvisibilityConverter_ConvertCollapsed()
        {
            string str = null;
            StringLengthToVisibilityConverter sltic = new StringLengthToVisibilityConverter();
            Visibility visibility = (Visibility)sltic.Convert(str, typeof(Visibility), null, null);

            Assert.AreEqual(Visibility.Collapsed, visibility);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void StringLengthToInvisibilityConverter_ConvertBack()
        {
            StringLengthToVisibilityConverter sltic = new StringLengthToVisibilityConverter();
            sltic.ConvertBack(null, null, null, null);
        }
    }
}
