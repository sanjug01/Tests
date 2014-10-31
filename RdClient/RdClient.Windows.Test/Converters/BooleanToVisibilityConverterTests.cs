using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Converters;
using Windows.UI.Xaml;

namespace RdClient.Windows.Test
{
    [TestClass]
    public class BooleanToVisibilityConverterTests
    {
        [TestMethod]
        public void ForwardTrue()
        {
            BooleanToVisibilityConverter btvc = new BooleanToVisibilityConverter();
            bool value = true;
            Visibility visibility;

            visibility = (Visibility)btvc.Convert(value, typeof(Visibility), null, "");
            Assert.AreEqual(Visibility.Visible, visibility);
        }

        [TestMethod]
        public void ForwardFalse()
        {
            BooleanToVisibilityConverter btvc = new BooleanToVisibilityConverter();
            bool value = false;
            Visibility visibility;

            visibility = (Visibility)btvc.Convert(value, typeof(Visibility), null, "");
            Assert.AreEqual(Visibility.Collapsed, visibility);
        }

        [TestMethod]
        public void BackVisible()
        {
            BooleanToVisibilityConverter btvc = new BooleanToVisibilityConverter();
            bool value;
            Visibility visibility = Visibility.Visible;

            value = (bool)btvc.ConvertBack(visibility, typeof(bool), null, "");
            Assert.AreEqual(true, value);
        }

        [TestMethod]
        public void BackCollapsed()
        {
            BooleanToVisibilityConverter btvc = new BooleanToVisibilityConverter();
            bool value;
            Visibility visibility = Visibility.Collapsed;

            value = (bool)btvc.ConvertBack(visibility, typeof(bool), null, "");
            Assert.AreEqual(false, value);
        }
    }
}
