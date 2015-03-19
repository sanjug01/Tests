﻿namespace RdClient.Shared.Test.Converters
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Converters;
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    [TestClass]
    public sealed class NotNullToVisibilityConverterTests
    {
        [TestMethod]
        public void NotNullToVisibilityConverter_NullInputVisibilityType_Collapsed()
        {
            IValueConverter converter = new NotNullToVisibilityConverter();

            object converted = converter.Convert(null, typeof(Visibility), null, null);

            Assert.IsInstanceOfType(converted, typeof(Visibility));
            Assert.AreEqual(Visibility.Collapsed, converted);
        }

        [TestMethod]
        public void NotNullToVisibilityConverter_NotNullInputVisibilityType_Visible()
        {
            IValueConverter converter = new NotNullToVisibilityConverter();

            object converted = converter.Convert(new object(), typeof(Visibility), null, null);

            Assert.IsInstanceOfType(converted, typeof(Visibility));
            Assert.AreEqual(Visibility.Visible, converted);
        }

        [TestMethod]
        public void NotNullToVisibilityConverter_NullInputNullType_Collapsed()
        {
            IValueConverter converter = new NotNullToVisibilityConverter();

            object converted = converter.Convert(null, null, null, null);

            Assert.IsInstanceOfType(converted, typeof(Visibility));
            Assert.AreEqual(Visibility.Collapsed, converted);
        }

        [TestMethod]
        public void NotNullToVisibilityConverter_NotNullInputNullType_Visible()
        {
            IValueConverter converter = new NotNullToVisibilityConverter();

            object converted = converter.Convert(new object(), null, null, null);

            Assert.IsInstanceOfType(converted, typeof(Visibility));
            Assert.AreEqual(Visibility.Visible, converted);
        }

        [TestMethod, ExpectedException(typeof(NotImplementedException))]
        public void NotNullToVisibilityConverter_Inverse_Throws()
        {
            IValueConverter converter = new NotNullToVisibilityConverter();

            object converted = converter.ConvertBack(Visibility.Visible, null, null, null);
        }
    }
}