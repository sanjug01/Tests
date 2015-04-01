using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Converters;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;

namespace RdClient.Shared.Test.Converters
{
    using RdClient.Shared.Input.ZoomPan;
    using RdClient.Shared.Test.UAP;

    [TestClass]
    public class PanKnobStateConvertersTests
    {
        private PanKnobStateToBackgroundBrushConverter _converterBackground;
        private PanKnobStateToForegroundBrushConverter _converterForeground;

        [TestInitialize]
        public void TestSetup()
        {            
            _converterBackground = new PanKnobStateToBackgroundBrushConverter();
            _converterForeground = new PanKnobStateToForegroundBrushConverter();
        }


        [TestMethod]
        public void ConvertToBackgroundNullThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
            {
                _converterBackground.Convert(null, null, null, null);
            }));
        }


        [TestMethod]
        public void ConvertToBackgroundInvalidTypeThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
            {
                string someValue = "10";
                _converterBackground.Convert(someValue, null, null, null);
            }));
        }

        [TestMethod]
        public void InverseConvertToBackgroundThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<InvalidOperationException>(() =>
            {
                _converterBackground.ConvertBack(null, null, null, null);
            }));        
        }

        [TestMethod]
        public void ConvertToForegroundNullThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
            {
                _converterForeground.Convert(null, null, null, null);
            }));
        }

        [TestMethod]
        public void ConvertToForegroundInvalidTypeThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
            {
                string someValue = "10";
                _converterForeground.Convert(someValue, null, null, null);
            }));
        }

        [TestMethod]
        public void InverseConvertToForegroundThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<InvalidOperationException>(() =>
            {
                _converterForeground.ConvertBack(null, null, null, null);
            }));
        }
    }
}
