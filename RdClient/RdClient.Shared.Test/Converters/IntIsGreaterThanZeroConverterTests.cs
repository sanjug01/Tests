namespace RdClient.Shared.Test.Converters
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Converters;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Windows.UI.Xaml;

    [TestClass]
    public class IntIsGreaterThanZeroConverterTests
    {
        [TestMethod]
        public void ValueEqualToZeroReturnsFalse()
        {
            var converter = new IntIsGreaterThanZeroConverter();
            int value = 0;
            Assert.AreEqual(false, converter.Convert(value, null, null, null));
        }

        [TestMethod]
        public void ValueGreaterThanZeroReturnsTrue()
        {
            var converter = new IntIsGreaterThanZeroConverter();
            int value = 1;
            Assert.AreEqual(true, converter.Convert(value, null, null, null));
        }

        [TestMethod]
        public void NegativeValueReturnsFalse()
        {
            var converter = new IntIsGreaterThanZeroConverter();
            int value = -1;
            Assert.AreEqual(false, converter.Convert(value, null, null, null));
        }

        [TestMethod]
        public void NonIntValueReturnsUnsetValue()
        {
            var converter = new IntIsGreaterThanZeroConverter();
            var value = new object();
            Assert.IsNull(value as ICollection);
            Assert.AreEqual(DependencyProperty.UnsetValue, converter.Convert(value, null, null, null));
        }

        [TestMethod]
        public void ConvertBackThrowsException()
        {
            var converter = new IntIsGreaterThanZeroConverter();
            Assert.ThrowsException<NotSupportedException>(() => converter.ConvertBack(true, null, null, null));
        }
    }
}
