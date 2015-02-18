using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Converters;
using Windows.UI.Xaml;

namespace RdClient.Shared.Test.Converters
{
    [TestClass]
    public class BooleanToVisibilityConverterTests
    {
        [TestMethod]
        public void BooleanToVisibilityConverter_ConvertTrue()
        {
            BooleanToVisibilityConverter btvc = new BooleanToVisibilityConverter();
            bool flag = true;
            Visibility visibility = (Visibility) btvc.Convert(flag, typeof(Visibility), null, null);

            Assert.AreEqual(Visibility.Visible, visibility);
        }

        [TestMethod]
        public void BooleanToVisibilityConverter_ConvertFalse()
        {
            BooleanToVisibilityConverter btvc = new BooleanToVisibilityConverter();
            bool flag = false;
            Visibility visibility = (Visibility)btvc.Convert(flag, typeof(Visibility), null, null);

            Assert.AreEqual(Visibility.Collapsed, visibility);
        }

        [TestMethod]
        public void BooleanToVisibilityConverter_ConvertBackTrue()
        {
            BooleanToVisibilityConverter btvc = new BooleanToVisibilityConverter();
            Visibility visibility = Visibility.Visible;
            bool flag = (bool)btvc.ConvertBack(visibility, typeof(bool), null, null);

            Assert.IsTrue(flag);
        }

        [TestMethod]
        public void BooleanToVisibilityConverter_ConvertBackFalse()
        {
            BooleanToVisibilityConverter btvc = new BooleanToVisibilityConverter();
            Visibility visibility = Visibility.Collapsed;
            bool flag = (bool)btvc.ConvertBack(visibility, typeof(bool), null, null);

            Assert.IsFalse(flag);
        }
    }
}
