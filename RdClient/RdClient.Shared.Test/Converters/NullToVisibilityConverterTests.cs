namespace RdClient.Shared.Test.Converters
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Converters;
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    [TestClass]
    public sealed class NullToVisibilityConverterTests
    {
        [TestMethod]
        public void NullToVisibilityConverter_NullInputVisibilityType_Visible()
        {
            IValueConverter converter = new NullToVisibilityConverter();

            object converted = converter.Convert(null, typeof(Visibility), null, null);

            Assert.IsInstanceOfType(converted, typeof(Visibility));
            Assert.AreEqual(Visibility.Visible, converted);
        }

        [TestMethod]
        public void NullToVisibilityConverter_NotNullInputVisibilityType_Collapsed()
        {
            IValueConverter converter = new NullToVisibilityConverter();

            object converted = converter.Convert(new object(), typeof(Visibility), null, null);

            Assert.IsInstanceOfType(converted, typeof(Visibility));
            Assert.AreEqual(Visibility.Collapsed, converted);
        }

        [TestMethod]
        public void NullToVisibilityConverter_NullInputNullType_Visible()
        {
            IValueConverter converter = new NullToVisibilityConverter();

            object converted = converter.Convert(null, null, null, null);

            Assert.IsInstanceOfType(converted, typeof(Visibility));
            Assert.AreEqual(Visibility.Visible, converted);
        }

        [TestMethod]
        public void NullToVisibilityConverter_NotNullInputNullType_Collapsed()
        {
            IValueConverter converter = new NullToVisibilityConverter();

            object converted = converter.Convert(new object(), null, null, null);

            Assert.IsInstanceOfType(converted, typeof(Visibility));
            Assert.AreEqual(Visibility.Collapsed, converted);
        }

        [TestMethod, ExpectedException(typeof(NotImplementedException))]
        public void NullToVisibilityConverter_Inverse_Throws()
        {
            IValueConverter converter = new NullToVisibilityConverter();

            object converted = converter.ConvertBack(Visibility.Visible, null, null, null);
        }
    }
}
