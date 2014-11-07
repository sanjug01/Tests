using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Converters;
using Windows.UI.Xaml;

namespace RdClient.Windows.Test
{
    [TestClass]
    public class InverseBooleanToVisibilityConverterTests
    {
        [TestMethod]
        public void ForwardTrue()
        {
            InverseBooleanToVisibilityConverter btvc = new InverseBooleanToVisibilityConverter();
            bool value = true;
            Visibility visibility;

            visibility = (Visibility)btvc.Convert(value, typeof(Visibility), null, "");
            Assert.AreEqual(Visibility.Collapsed, visibility);
        }

        [TestMethod]
        public void ForwardFalse()
        {
            InverseBooleanToVisibilityConverter btvc = new InverseBooleanToVisibilityConverter();
            bool value = false;
            Visibility visibility;

            visibility = (Visibility)btvc.Convert(value, typeof(Visibility), null, "");
            Assert.AreEqual(Visibility.Visible, visibility);
        }

        [TestMethod]
        public void BackVisible()
        {
            InverseBooleanToVisibilityConverter btvc = new InverseBooleanToVisibilityConverter();
            bool value;
            Visibility visibility = Visibility.Visible;

            value = (bool)btvc.ConvertBack(visibility, typeof(bool), null, "");
            Assert.AreEqual(false, value);
        }

        [TestMethod]
        public void BackCollapsed()
        {
            InverseBooleanToVisibilityConverter btvc = new InverseBooleanToVisibilityConverter();
            bool value;
            Visibility visibility = Visibility.Collapsed;

            value = (bool)btvc.ConvertBack(visibility, typeof(bool), null, "");
            Assert.AreEqual(true, value);
        }
    }
}
