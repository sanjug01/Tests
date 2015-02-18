using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace RdClient.Shared.Test.Converters
{
    [TestClass]
    public class InverseBooleanToVisibilityConverterTests
    {
        [TestMethod]
        public void InverseBooleanToVisibilityConverterTests_ConvertTrue()
        {
            InverseBooleanToVisibilityConverter ibtvc = new InverseBooleanToVisibilityConverter();
            bool flag = true;
            Visibility visibility = (Visibility) ibtvc.Convert(flag, typeof(Visibility), null, null);

            Assert.AreEqual(Visibility.Collapsed, visibility);
        }

        [TestMethod]
        public void InverseBooleanToVisibilityConverterTests_ConvertFalse()
        {
            InverseBooleanToVisibilityConverter ibtvc = new InverseBooleanToVisibilityConverter();
            bool flag = false;
            Visibility visibility = (Visibility)ibtvc.Convert(flag, typeof(Visibility), null, null);

            Assert.AreEqual(Visibility.Visible, visibility);
        }

        [TestMethod]
        public void InverseBooleanToVisibilityConverterTests_ConvertBackCollapsed()
        {
            InverseBooleanToVisibilityConverter ibtvc = new InverseBooleanToVisibilityConverter();
            Visibility visibility = Visibility.Collapsed;
            bool flag = (bool)ibtvc.ConvertBack(visibility, typeof(Visibility), null, null);

            Assert.IsTrue(flag);
        }

        [TestMethod]
        public void InverseBooleanToVisibilityConverterTests_ConvertBackVisible()
        {
            InverseBooleanToVisibilityConverter ibtvc = new InverseBooleanToVisibilityConverter();
            Visibility visibility = Visibility.Visible;
            bool flag = (bool)ibtvc.ConvertBack(visibility, typeof(Visibility), null, null);

            Assert.IsFalse(flag);
        }
    }
}
