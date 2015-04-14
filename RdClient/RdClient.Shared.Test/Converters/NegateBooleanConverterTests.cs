namespace RdClient.Shared.Test.Converters
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Converters;
    using Windows.UI.Xaml.Data;

    [TestClass]
    public sealed class NegateBooleanConverterTests
    {
        [TestMethod]
        public void NegateBooleanConverter_ConvertTrue_Converts()
        {
            IValueConverter converter = new NegateBooleanConverter();
            Assert.IsFalse((bool)converter.Convert(true, null, null, null));
        }

        [TestMethod]
        public void NegateBooleanConverter_ConvertFalse_Converts()
        {
            IValueConverter converter = new NegateBooleanConverter();
            Assert.IsTrue((bool)converter.Convert(false, null, null, null));
        }
    }
}
