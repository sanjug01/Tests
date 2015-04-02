using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
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
    using RdClient.Shared.Test.UAP;

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
        public void ConvertToZoomInNullThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
            {
                _converterZoomIn.Convert(null, null, null, null);
            }));
        }

        [TestMethod]
        public void ConvertToZoomInInvalidTypeThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
            {
                string someValue = "10";
                _converterZoomIn.Convert(someValue, null, null, null);
            }));
        }

        [TestMethod]
        public void InverseConvertToZoomInThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<InvalidOperationException>(() =>
            {
                _converterZoomIn.ConvertBack(null, null, null, null);
            }));
        }

        [TestMethod]
        public void ConvertToZoomOutNullThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
            {
                _converterZoomOut.Convert(null, null, null, null);
            }));
        }

        [TestMethod]
        public void ConvertToZoomOutInvalidTypeThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
            {
                string someValue = "10";
                _converterZoomOut.Convert(someValue, null, null, null);
            }));
        }

        [TestMethod]
        public void InverseConvertToZoomOutThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<InvalidOperationException>(() =>
            {
                _converterZoomOut.ConvertBack(null, null, null, null);
            }));
        }

        [TestMethod]
        public void ConvertToPanKnobNullThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
            {
                _converterPanKnob.Convert(null, null, null, null);
            }));
        }


        [TestMethod]
        public void ConvertToPanKnobInvalidTypeThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
            {
                string someValue = "10";
                _converterPanKnob.Convert(someValue, null, null, null);
            }));
        }

        [TestMethod]
        public void InverseConvertToPanKnobThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<InvalidOperationException>(() =>
            {
                _converterPanKnob.ConvertBack(null, null, null, null);
            }));
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
