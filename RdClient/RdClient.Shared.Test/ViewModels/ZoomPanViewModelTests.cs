using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Input.ZoomPan;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class ZoomPanelViewModelTests
    {

        Rect _windowRect = new Rect(0, 0, 1920, 1080);
        Rect _transformRectNoZoom = new Rect(0, 0, 1920, 1080);
        Rect _transformRectWithZoom = new Rect(-480, -270, 2880, 1620);

        // default transform parameters
        PanLeftTransform _panLeftTransform = new PanLeftTransform();
        PanRightTransform _panRightTransform = new PanRightTransform();
        PanUpTransform _panUpTransform = new PanUpTransform();
        PanDownTransform _panDownTransform = new PanDownTransform();
        ZoomInTransform _zoomInTransform = new ZoomInTransform();
        ZoomOutTransform _zoomOutTransform = new ZoomOutTransform();
        ZoomPanViewModel _svm;

        [TestInitialize]
        public void SetUpTest()
        {
            _svm = new ZoomPanViewModel();
            _svm.WindowRect = _windowRect;
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _svm = null;
        }

        [TestMethod]
        public void ZoomPanViewModel_ZoomIn_ShouldUpdateTransformProperty()
        {
            bool notificationFired = false;

            _svm.PropertyChanged += ((sender, e) =>
            {
                if ("ZoomPanTransform".Equals(e.PropertyName))
                {
                    notificationFired = true;
                }
            });

            _svm.ToggleZoomCommand.Execute(_zoomInTransform);

            Assert.IsTrue(notificationFired);
            Assert.AreEqual(TransformType.ZoomIn, _svm.ZoomPanTransform.TransformType);
            Assert.IsTrue(1.0 < _svm.ScaleXTo);
            Assert.IsTrue(1.0 < _svm.ScaleYTo);            
        }

        [TestMethod]
        public void ZoomPanViewModel_ZoomOut_ShouldUpdateTransformProperty()
        {
            bool notificationFired = false;

            _svm.PropertyChanged += ((sender, e) =>
            {
                if ("ZoomPanTransform".Equals(e.PropertyName))
                {
                    notificationFired = true;
                }
            });

            _svm.ToggleZoomCommand.Execute(_zoomOutTransform);

            Assert.IsTrue(notificationFired);
            Assert.AreEqual(TransformType.ZoomOut, _svm.ZoomPanTransform.TransformType);
        }

        [TestMethod]
        public void ZoomPanViewModel_PanLeft_ShouldUpdateTransformProperty()
        {
            bool notificationFired = false;

            _svm.PropertyChanged += ((sender, e) =>
            {
                if ("ZoomPanTransform".Equals(e.PropertyName))
                {
                    notificationFired = true;
                }
            });

            _svm.PanCommand.Execute(_panLeftTransform);

            Assert.IsTrue(notificationFired);
            Assert.AreEqual(TransformType.Pan, _svm.ZoomPanTransform.TransformType);
        }

        [TestMethod]
        public void ZoomPanViewModel_PanRight_ShouldUpdateTransformProperty()
        {
            bool notificationFired = false;

            _svm.PropertyChanged += ((sender, e) =>
            {
                if ("ZoomPanTransform".Equals(e.PropertyName))
                {
                    notificationFired = true;
                }
            });

            _svm.PanCommand.Execute(_panRightTransform);

            Assert.IsTrue(notificationFired);
            Assert.AreEqual(TransformType.Pan, _svm.ZoomPanTransform.TransformType);
        }

        [TestMethod]
        public void ZoomPanViewModel_PanUp_ShouldUpdateTransformProperty()
        {
            bool notificationFired = false;

            _svm.PropertyChanged += ((sender, e) =>
            {
                if ("ZoomPanTransform".Equals(e.PropertyName))
                {
                    notificationFired = true;
                }
            });

            _svm.PanCommand.Execute(_panUpTransform);

            Assert.IsTrue(notificationFired);
            Assert.AreEqual(TransformType.Pan, _svm.ZoomPanTransform.TransformType);
        }

        [TestMethod]
        public void ZoomPanViewModel_PanDown_ShouldUpdateTransformProperty()
        {
            bool notificationFired = false;

            _svm.PropertyChanged += ((sender, e) =>
            {
                if ("ZoomPanTransform".Equals(e.PropertyName))
                {
                    notificationFired = true;
                }
            });

            _svm.PanCommand.Execute(_panDownTransform);

            Assert.IsTrue(notificationFired);
            Assert.AreEqual(TransformType.Pan, _svm.ZoomPanTransform.TransformType);
        }

        [TestMethod]
        public void ZoomPanViewModel_ZoomIn_UsesWindowRectCenter()
        {
            double centerX = (_windowRect.Right - _windowRect.Left) / 2;
            double centerY = (_windowRect.Bottom - _windowRect.Top) / 2;

            _svm.WindowRect = _windowRect;         
            _svm.ToggleZoomCommand.Execute(_zoomInTransform);

            Assert.AreEqual(centerX, _svm.ScaleCenterX);
            Assert.AreEqual(centerY, _svm.ScaleCenterY);
        }

        [TestMethod]
        public void ZoomPanViewModel_VerifyScaleFactorLimits()
        {
            double maxScale = 2.5;
            double minScale = 1.0;

            // max zoomIn
            do
	        {
                _svm.ToggleZoomCommand.Execute(_zoomInTransform);
                Assert.IsTrue(_svm.ScaleXFrom < _svm.ScaleXTo);
                Assert.IsTrue(_svm.ScaleYFrom <_svm.ScaleYTo);
	        } while (_svm.ScaleXTo < maxScale);

            Assert.AreEqual(maxScale, _svm.ScaleXTo);
            Assert.AreEqual(maxScale, _svm.ScaleYTo);
            
            // one more zoomIn does not change anything
            _svm.ToggleZoomCommand.Execute(_zoomInTransform);	         
            Assert.AreEqual(maxScale, _svm.ScaleXTo);
            Assert.AreEqual(_svm.ScaleXFrom, _svm.ScaleXTo);
            Assert.AreEqual(_svm.ScaleYFrom, _svm.ScaleYTo);

            // max zoomOut
            do
            {
                _svm.ToggleZoomCommand.Execute(_zoomOutTransform);
                Assert.IsTrue(_svm.ScaleXFrom > _svm.ScaleXTo);
                Assert.IsTrue(_svm.ScaleYFrom > _svm.ScaleYTo);
            } while (_svm.ScaleXTo > minScale);

            Assert.AreEqual(minScale, _svm.ScaleXTo);
            Assert.AreEqual(minScale, _svm.ScaleYTo);

            // one more zoomOut does not change anything
            _svm.ToggleZoomCommand.Execute(_zoomOutTransform);
            Assert.AreEqual(minScale, _svm.ScaleXTo);
            Assert.AreEqual(_svm.ScaleXFrom, _svm.ScaleXTo);
            Assert.AreEqual(_svm.ScaleYFrom, _svm.ScaleYTo);                
        }

        [TestMethod]
        public void ZoomPanViewModel_VerifyScaleFactorLimitsWithCustomZoom()
        {
            double maxScale = 2.5;
            double minScale = 1.0;
            double centerX = 150;
            double centerY = 250;
            double customScaleX = 2.0;
            double customScaleY = 1.5;

            CustomZoomTransform zoomTransform = new CustomZoomTransform(centerX, centerY, customScaleX, customScaleY);
            _svm.ToggleZoomCommand.Execute(zoomTransform);
            Assert.AreEqual(centerX, _svm.ScaleCenterX);
            Assert.AreEqual(centerY, _svm.ScaleCenterY);
            Assert.AreEqual(customScaleX, _svm.ScaleXTo);
            Assert.AreEqual(customScaleY, _svm.ScaleYTo);

            // test boundaries
            centerX += 25.0;
            centerY -= 25.0;
            zoomTransform = new CustomZoomTransform(centerX, centerY, customScaleX, maxScale + 0.5);
            _svm.ToggleZoomCommand.Execute(zoomTransform);
            Assert.AreEqual(centerX, _svm.ScaleCenterX);
            Assert.AreEqual(centerY, _svm.ScaleCenterY);
            Assert.AreEqual(customScaleX, _svm.ScaleXTo);
            Assert.AreEqual(maxScale, _svm.ScaleYTo);

            centerX += 25.0;
            centerY -= 25.0;
            zoomTransform = new CustomZoomTransform(centerX, centerY, minScale - 0.5, customScaleY);
            _svm.ToggleZoomCommand.Execute(zoomTransform);
            Assert.AreEqual(centerX, _svm.ScaleCenterX);
            Assert.AreEqual(centerY, _svm.ScaleCenterY);
            Assert.AreEqual(minScale, _svm.ScaleXTo);
            Assert.AreEqual(customScaleY, _svm.ScaleYTo);

            centerX += 25.0;
            centerY -= 25.0;
            zoomTransform = new CustomZoomTransform(centerX, centerY, maxScale + 0.5, minScale - 0.5);
            _svm.ToggleZoomCommand.Execute(zoomTransform);
            Assert.AreEqual(centerX, _svm.ScaleCenterX);
            Assert.AreEqual(centerY, _svm.ScaleCenterY);
            Assert.AreEqual(maxScale, _svm.ScaleXTo);
            Assert.AreEqual(minScale, _svm.ScaleYTo);
        }

        [TestMethod]
        public void ZoomPanViewModel_NoZoom_ShouldNotApplyPan()
        {
            _svm.WindowRect = _windowRect;
            _svm.TransformRect = _transformRectNoZoom;

            _svm.PanCommand.Execute(_panLeftTransform);
            Assert.AreEqual(_svm.ScaleXTo, _svm.ScaleXFrom);
            Assert.AreEqual(_svm.TranslateXTo, _svm.TranslateXFrom);
            Assert.AreEqual(_svm.TranslateYTo, _svm.TranslateYFrom);

            _svm.PanCommand.Execute(_panRightTransform);
            Assert.AreEqual(_svm.ScaleXTo, _svm.ScaleXFrom);
            Assert.AreEqual(_svm.TranslateXTo, _svm.TranslateXFrom);
            Assert.AreEqual(_svm.TranslateYTo, _svm.TranslateYFrom);

            _svm.PanCommand.Execute(_panUpTransform);
            Assert.AreEqual(_svm.ScaleXTo, _svm.ScaleXFrom);
            Assert.AreEqual(_svm.TranslateXTo, _svm.TranslateXFrom);
            Assert.AreEqual(_svm.TranslateYTo, _svm.TranslateYFrom);

            _svm.PanCommand.Execute(_panDownTransform);
            Assert.AreEqual(_svm.ScaleXTo, _svm.ScaleXFrom);
            Assert.AreEqual(_svm.TranslateXTo, _svm.TranslateXFrom);
            Assert.AreEqual(_svm.TranslateYTo, _svm.TranslateYFrom);
        }

        [TestMethod]
        public void ZoomPanViewModel_AfterZoom_ShouldApplyPan()
        {
            _svm.ToggleZoomCommand.Execute(_zoomInTransform);

            Assert.IsTrue(1.0 < _svm.ScaleXTo);
            Assert.IsTrue(1.0 < _svm.ScaleYTo);

            _svm.WindowRect = _windowRect;
            _svm.TransformRect = _transformRectWithZoom;

            _svm.PanCommand.Execute(_panLeftTransform);
            Assert.AreEqual(_svm.ScaleXTo, _svm.ScaleXFrom);
            Assert.IsTrue(_svm.TranslateXTo <_svm.TranslateXFrom);
            Assert.AreEqual(_svm.TranslateYTo, _svm.TranslateYFrom);

            _svm.PanCommand.Execute(_panRightTransform);
            Assert.AreEqual(_svm.ScaleXTo, _svm.ScaleXFrom);
            Assert.IsTrue(_svm.TranslateXTo > _svm.TranslateXFrom);
            Assert.AreEqual(_svm.TranslateYTo, _svm.TranslateYFrom);

            _svm.PanCommand.Execute(_panUpTransform);
            Assert.AreEqual(_svm.ScaleXTo, _svm.ScaleXFrom);
            Assert.AreEqual(_svm.TranslateXTo, _svm.TranslateXFrom);
            Assert.IsTrue(_svm.TranslateYTo >_svm.TranslateYFrom);

            _svm.PanCommand.Execute(_panDownTransform);
            Assert.AreEqual(_svm.ScaleXTo, _svm.ScaleXFrom);
            Assert.AreEqual(_svm.TranslateXTo, _svm.TranslateXFrom);
            Assert.IsTrue(_svm.TranslateYTo < _svm.TranslateYFrom);
        }


        [TestMethod]
        public void ZoomPanViewModel_DoesNotApplyUpPanOutsideBoundaries()
        {
            double maxUp = _windowRect.Top - _transformRectWithZoom.Top;
            double delta = 100.0;
            _svm.ToggleZoomCommand.Execute(_zoomInTransform);

            Assert.IsTrue(1.0 < _svm.ScaleXTo);
            Assert.IsTrue(1.0 < _svm.ScaleYTo);

            _svm.WindowRect = _windowRect;
            _svm.TransformRect = _transformRectWithZoom;

            PanTransform maxUpTransform = new PanTransform(0.0, maxUp + delta);
            _svm.PanCommand.Execute(maxUpTransform);
            Assert.AreEqual(_svm.ScaleXTo, _svm.ScaleXFrom);
            Assert.AreEqual(_svm.TranslateXTo, _svm.TranslateXFrom);
            Assert.IsTrue(_svm.TranslateYTo > _svm.TranslateYFrom);
            Assert.AreEqual(_svm.TranslateYTo, maxUp);

            // one more up does not change anything
            _svm.PanCommand.Execute(_panUpTransform);
            Assert.AreEqual(_svm.ScaleXTo, _svm.ScaleXFrom);
            Assert.AreEqual(_svm.TranslateXTo, _svm.TranslateXFrom);
            Assert.IsTrue(_svm.TranslateYTo == _svm.TranslateYFrom);
        }

        [TestMethod]
        public void ZoomPanViewModel_DoesNotApplyDownPanOutsideBoundaries()
        {
            double minDown = _windowRect.Bottom - _transformRectWithZoom.Bottom;
            double delta = 100.0;
            _svm.ToggleZoomCommand.Execute(_zoomInTransform);

            Assert.IsTrue(1.0 < _svm.ScaleXTo);
            Assert.IsTrue(1.0 < _svm.ScaleYTo);

            _svm.WindowRect = _windowRect;
            _svm.TransformRect = _transformRectWithZoom;

            PanTransform maxUpTransform = new PanTransform(0.0, minDown - delta);
            _svm.PanCommand.Execute(maxUpTransform);
            Assert.AreEqual(_svm.ScaleXTo, _svm.ScaleXFrom);
            Assert.AreEqual(_svm.TranslateXTo, _svm.TranslateXFrom);
            Assert.IsTrue(_svm.TranslateYTo < _svm.TranslateYFrom);
            Assert.AreEqual(_svm.TranslateYTo, minDown);

            // one more down does not change anything
            _svm.PanCommand.Execute(_panDownTransform);
            Assert.AreEqual(_svm.ScaleXTo, _svm.ScaleXFrom);
            Assert.AreEqual(_svm.TranslateXTo, _svm.TranslateXFrom);
            Assert.IsTrue(_svm.TranslateYTo == _svm.TranslateYFrom);
        }

        [TestMethod]
        public void ZoomPanViewModel_DoesNotApplyRightPanOutsideBoundaries()
        {
            double maxRight = _windowRect.Left - _transformRectWithZoom.Left;
            double delta = 100.0;
            _svm.ToggleZoomCommand.Execute(_zoomInTransform);

            Assert.IsTrue(1.0 < _svm.ScaleXTo);
            Assert.IsTrue(1.0 < _svm.ScaleYTo);

            _svm.WindowRect = _windowRect;
            _svm.TransformRect = _transformRectWithZoom;

            PanTransform maxRightTransform = new PanTransform(maxRight + delta, 0.0);
            _svm.PanCommand.Execute(maxRightTransform);
            Assert.AreEqual(_svm.ScaleXTo, _svm.ScaleXFrom);
            Assert.IsTrue(_svm.TranslateXTo > _svm.TranslateXFrom);
            Assert.AreEqual(_svm.TranslateYTo, _svm.TranslateYFrom);
            Assert.AreEqual(_svm.TranslateXTo, maxRight);

            // one more right does not change anything
            _svm.PanCommand.Execute(_panRightTransform);
            Assert.AreEqual(_svm.ScaleXTo, _svm.ScaleXFrom);
            Assert.IsTrue(_svm.TranslateXTo == _svm.TranslateXFrom);
            Assert.AreEqual(_svm.TranslateYTo, _svm.TranslateYFrom);
        }

        [TestMethod]
        public void ZoomPanViewModel_DoesNotApplyLeftPanOutsideBoundaries()
        {
            double minLeft = _windowRect.Right - _transformRectWithZoom.Right;
            double delta = 100.0;
            _svm.ToggleZoomCommand.Execute(_zoomInTransform);

            Assert.IsTrue(1.0 < _svm.ScaleXTo);
            Assert.IsTrue(1.0 < _svm.ScaleYTo);

            _svm.WindowRect = _windowRect;
            _svm.TransformRect = _transformRectWithZoom;

            PanTransform maxLeftTransform = new PanTransform(minLeft - delta, 0.0);
            _svm.PanCommand.Execute(maxLeftTransform);
            Assert.AreEqual(_svm.ScaleXTo, _svm.ScaleXFrom);
            Assert.IsTrue(_svm.TranslateXTo < _svm.TranslateXFrom);
            Assert.AreEqual(_svm.TranslateYTo, _svm.TranslateYFrom);
            Assert.AreEqual(_svm.TranslateXTo, minLeft);

            // one more left does not change anything
            _svm.PanCommand.Execute(_panLeftTransform);
            Assert.AreEqual(_svm.ScaleXTo, _svm.ScaleXFrom);
            Assert.IsTrue(_svm.TranslateXTo == _svm.TranslateXFrom);
            Assert.AreEqual(_svm.TranslateYTo, _svm.TranslateYFrom);
        }

        [TestMethod]
        public void ZoomPanViewModel_TouchModeDisablesToggleZoom()
        {
            _svm.PointerModeEnabled = false;

            // initial
            Assert.IsTrue(1.0 == _svm.ScaleXTo);
            Assert.IsTrue(1.0 == _svm.ScaleYTo);
            Assert.IsFalse(_svm.CanZoomIn);
            Assert.IsFalse(_svm.CanZoomOut);

            _svm.ToggleZoomCommand.Execute(_zoomInTransform);
            Assert.IsFalse(_svm.CanZoomIn);
            Assert.IsFalse(_svm.CanZoomOut);

            // zoomOut reverts
            _svm.ToggleZoomCommand.Execute(_zoomOutTransform);
            Assert.IsFalse(_svm.CanZoomIn);
            Assert.IsFalse(_svm.CanZoomOut);
        }

        [TestMethod]
        public void ZoomPanViewModel_PointerModeToggleZoom_CannotZoomMore()
        {
            _svm.PointerModeEnabled = true;
            
            // initial
            Assert.IsTrue(1.0 == _svm.ScaleXTo);
            Assert.IsTrue(1.0 == _svm.ScaleYTo);
            Assert.IsTrue(_svm.CanZoomIn);
            Assert.IsFalse(_svm.CanZoomOut);

            _svm.ToggleZoomCommand.Execute(_zoomInTransform);
            Assert.IsTrue(1.0 < _svm.ScaleXTo);

            Assert.IsFalse(_svm.CanZoomIn);
            Assert.IsTrue(_svm.CanZoomOut);

            // zoomOut reverts
            _svm.ToggleZoomCommand.Execute(_zoomOutTransform);
            Assert.IsTrue(_svm.CanZoomIn);
            Assert.IsFalse(_svm.CanZoomOut);
        }

        [TestMethod]
        public void ZoomPanViewModel_SwitchPointerMode_UpdatesCanZoomProperties()
        {
            bool updatedCanZoomIn = false;
            bool updatedCanZoomOut = false;

            _svm.PropertyChanged += (sender, e) =>
                {
                    if ("CanZoomIn".Equals(e.PropertyName))
                    {
                        updatedCanZoomIn = true;
                    }
                    if ("CanZoomOut".Equals(e.PropertyName))
                    {
                        updatedCanZoomOut = true;
                    }
                };

            // initial
            Assert.IsTrue(_svm.CanZoomIn);
            Assert.IsFalse(_svm.CanZoomOut);
            Assert.IsTrue(_svm.PointerModeEnabled);
            
            // switch
            _svm.PointerModeEnabled = false;
            Assert.IsTrue(updatedCanZoomIn);
            Assert.IsTrue(updatedCanZoomOut);
            Assert.IsFalse(_svm.CanZoomIn);
            Assert.IsFalse(_svm.CanZoomOut);

            // switch back
            updatedCanZoomIn = false;
            updatedCanZoomOut = false;
            _svm.PointerModeEnabled = true;
            Assert.IsTrue(updatedCanZoomIn);
            Assert.IsTrue(updatedCanZoomOut);
            Assert.IsTrue(_svm.CanZoomIn);
            Assert.IsFalse(_svm.CanZoomOut);

            // after zoomIn
            _svm.ToggleZoomCommand.Execute(_zoomInTransform);
            Assert.IsFalse(_svm.CanZoomIn);
            Assert.IsTrue(_svm.CanZoomOut);

            // switch
            updatedCanZoomIn = false;
            updatedCanZoomOut = false;
            _svm.PointerModeEnabled = false;
            Assert.IsTrue(updatedCanZoomIn);
            Assert.IsTrue(updatedCanZoomOut);
            Assert.IsFalse(_svm.CanZoomIn);
            Assert.IsFalse(_svm.CanZoomOut);

            // switch back
            updatedCanZoomIn = false;
            updatedCanZoomOut = false;
            _svm.PointerModeEnabled = true;
            Assert.IsTrue(updatedCanZoomIn);
            Assert.IsTrue(updatedCanZoomOut);
            Assert.IsFalse(_svm.CanZoomIn);
            Assert.IsTrue(_svm.CanZoomOut);
        }
    }
}
