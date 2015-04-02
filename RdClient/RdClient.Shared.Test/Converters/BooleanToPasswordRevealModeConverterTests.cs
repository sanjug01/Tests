namespace RdClient.Shared.Test.Converters
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Converters;
    using RdClient.Shared.Test.UAP;
    using System;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;

    [TestClass]
    public sealed class BooleanToPasswordRevealModeConverterTests
    {
        [TestMethod]
        public void NewBooleanToPasswordRevealModeConverter_CorrectDefaultMode()
        {
            BooleanToPasswordRevealModeConverter converter = new BooleanToPasswordRevealModeConverter();

            Assert.AreEqual(PasswordRevealMode.Peek, converter.RevealMode);
        }

        [TestMethod]
        public void NewBooleanToPasswordRevealModeConverter_SetMode_CorrectModeSet()
        {
            BooleanToPasswordRevealModeConverter converter = new BooleanToPasswordRevealModeConverter()
            {
                RevealMode = PasswordRevealMode.Visible
            };

            Assert.AreEqual(PasswordRevealMode.Visible, converter.RevealMode);
        }

        [TestMethod]
        public void BooleanToPasswordRevealModeConverter_ConvertTrueNoTargetType_Converted()
        {
            IValueConverter converter = new BooleanToPasswordRevealModeConverter();

            Assert.AreEqual(PasswordRevealMode.Peek, converter.Convert(true, null, null, null));
        }

        [TestMethod]
        public void BooleanToPasswordRevealModeConverter_ConvertFalseNoTargetType_Converted()
        {
            IValueConverter converter = new BooleanToPasswordRevealModeConverter();

            Assert.AreEqual(PasswordRevealMode.Hidden, converter.Convert(false, null, null, null));
        }

        [TestMethod]
        public void BooleanToPasswordRevealModeConverter_ConvertTrueCorrectTargetType_Converted()
        {
            IValueConverter converter = new BooleanToPasswordRevealModeConverter();

            Assert.AreEqual(PasswordRevealMode.Peek, converter.Convert(true, typeof(PasswordRevealMode), null, null));
        }

        [TestMethod]
        public void BooleanToPasswordRevealModeConverter_ConvertFalseCorrectTargetType_Converted()
        {
            IValueConverter converter = new BooleanToPasswordRevealModeConverter();

            Assert.AreEqual(PasswordRevealMode.Hidden, converter.Convert(false, typeof(PasswordRevealMode), null, null));
        }

        [TestMethod]
        public void BooleanToPasswordRevealModeConverter_ConvertTrueBadTargetType_Throws()
        {
            IValueConverter converter = new BooleanToPasswordRevealModeConverter();

            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
                converter.Convert(true, typeof(bool), null, null)));
        }

        [TestMethod]
        public void BooleanToPasswordRevealModeConverter_ConvertBack_Throws()
        {
            IValueConverter converter = new BooleanToPasswordRevealModeConverter();

            Assert.IsTrue(ExceptionExpecter.ExpectException<NotImplementedException>(() =>
                converter.ConvertBack(PasswordRevealMode.Hidden, null, null, null)));
        }
    }
}
