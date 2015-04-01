using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Converters;
using RdClient.Shared.Test.UAP;
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
        public void StringLengthToInvisibilityConverter_ConvertBack()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<NotImplementedException>(() =>
            {
                StringLengthToVisibilityConverter sltic = new StringLengthToVisibilityConverter();
                sltic.ConvertBack(null, null, null, null);
            }));
        }
    }
}
