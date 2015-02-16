using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    [TestClass]
    public class PanKnobStateConvertersTests
    {
        private PanKnobStateToBackGroundBrushConverter _converterBackground;
        private PanKnobStateToForegroundBrushConverter _converterForeground;

        [TestInitialize]
        public void TestSetup()
        {            
            _converterBackground = new PanKnobStateToBackGroundBrushConverter();
            _converterForeground = new PanKnobStateToForegroundBrushConverter();
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertToBackgroundNullThrows()
        {
            _converterBackground.Convert(null, null, null, null);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertToBackgroundInvalidTypeThrows()
        {
            string someValue = "10";
            _converterBackground.Convert(someValue, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InverseConvertToBackgroundThrows()
        {
            _converterBackground.ConvertBack(null, null, null, null);            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertToForegroundNullThrows()
        {
            _converterForeground.Convert(null, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertToForegroundInvalidTypeThrows()
        {
            string someValue = "10";
            _converterForeground.Convert(someValue, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InverseConvertToForegroundThrows()
        {
            _converterForeground.ConvertBack(null, null, null, null);
        }
    }
}
