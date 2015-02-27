using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Converters;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace RdClient.Shared.Test.Converters
{
    using RdClient.Shared.Input.ZoomPan;
    [TestClass]
    public class ZoomPanStateConvertersTests
    {
        private ZoomPanStateToZoomInVisibilityConverter _converterZoomIn;
        private ZoomPanStateToZoomOutVisibilityConverter _converterZoomOut;
        private ZoomPanStateToPanKnobVisibilityConverter _converterPanKnob;

        [TestInitialize]
        public void TestSetup()
        {            
            _converterZoomIn = new ZoomPanStateToZoomInVisibilityConverter();
            _converterZoomOut = new ZoomPanStateToZoomOutVisibilityConverter();
            _converterPanKnob = new ZoomPanStateToPanKnobVisibilityConverter();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertToZoomInNullThrows()
        {
            _converterZoomIn.Convert(null, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertToZoomInInvalidTypeThrows()
        {
            string someValue = "10";
            _converterZoomIn.Convert(someValue, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InverseConvertToZoomInThrows()
        {
            _converterZoomIn.ConvertBack(null, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertToZoomOutNullThrows()
        {
            _converterZoomOut.Convert(null, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertToZoomOutInvalidTypeThrows()
        {
            string someValue = "10";
            _converterZoomOut.Convert(someValue, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InverseConvertToZoomOutThrows()
        {
            _converterZoomOut.ConvertBack(null, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertToPanKnobNullThrows()
        {
            _converterPanKnob.Convert(null, null, null, null);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertToPanKnobInvalidTypeThrows()
        {
            string someValue = "10";
            _converterPanKnob.Convert(someValue, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InverseConvertToPanKnobThrows()
        {
            _converterPanKnob.ConvertBack(null, null, null, null);
        }

        [TestMethod]
        public void ConvertValidZoomPanState()
        {
            ZoomPanState state = ZoomPanState.TouchMode_MinScale;
            Visibility visibilityZoomIn;
            Visibility visibilityZoomOut;
            Visibility visibilityPanKnob;

            visibilityZoomIn = (Visibility)_converterZoomIn.Convert(state, typeof(Visibility), null, "");
            visibilityZoomOut = (Visibility)_converterZoomOut.Convert(state, typeof(Visibility), null, "");
            visibilityPanKnob = (Visibility)_converterPanKnob.Convert(state, typeof(Visibility), null, "");

            Assert.AreEqual(Visibility.Visible, visibilityZoomIn);
            Assert.AreEqual(Visibility.Collapsed, visibilityZoomOut);
            Assert.AreEqual(Visibility.Collapsed, visibilityPanKnob);

            state = ZoomPanState.TouchMode_MaxScale;
            visibilityZoomIn = (Visibility)_converterZoomIn.Convert(state, typeof(Visibility), null, "");
            visibilityZoomOut = (Visibility)_converterZoomOut.Convert(state, typeof(Visibility), null, "");
            visibilityPanKnob = (Visibility)_converterPanKnob.Convert(state, typeof(Visibility), null, "");

            Assert.AreEqual(Visibility.Collapsed, visibilityZoomIn);
            Assert.AreEqual(Visibility.Visible, visibilityZoomOut);
            Assert.AreEqual(Visibility.Visible, visibilityPanKnob);

            state = ZoomPanState.PointerMode_DefaultScale;
            visibilityZoomIn = (Visibility)_converterZoomIn.Convert(state, typeof(Visibility), null, "");
            visibilityZoomOut = (Visibility)_converterZoomOut.Convert(state, typeof(Visibility), null, "");
            visibilityPanKnob = (Visibility)_converterPanKnob.Convert(state, typeof(Visibility), null, "");

            Assert.AreEqual(Visibility.Collapsed, visibilityZoomIn);
            Assert.AreEqual(Visibility.Collapsed, visibilityZoomOut);
            Assert.AreEqual(Visibility.Collapsed, visibilityPanKnob);
        }
    }
}
