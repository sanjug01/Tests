using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Input.Mouse;
using RdClient.Shared.Input.ZoomPan;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class PanKnobViewModelTests
    {

        Rect _windowRect = new Rect(0, 0, 1280, 800);
        PanKnobViewModel _vvm;
        uint _pointerId;
        PointerType _pointerType;

        [TestInitialize]
        public void SetUpTest()
        {
            _vvm = new PanKnobViewModel();
            _vvm.ViewSize = new Size(_windowRect.Width, _windowRect.Height);
            _pointerId = 3;
            _pointerType = PointerType.Touch;
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _vvm = null;
        }

        [TestMethod]
        public void PanKnobViewModel_DefaultStateIsInactive()
        {
            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);               
        }

        [TestMethod]
        public void PanKnobViewModel_DefaultTransformParamsNotInitialized()
        {
            Assert.AreEqual(0, _vvm.TranslateXFrom);
            Assert.AreEqual(0, _vvm.TranslateYFrom);

            Assert.AreEqual(0, _vvm.TranslateXTo);
            Assert.AreEqual(0, _vvm.TranslateYTo);
        }

        [TestMethod]
        public void PanKnobViewModel_ShowEmitsPropertyChange()
        {
            bool notificationTransform = false;

            _vvm.PropertyChanged += ((sender, e) =>
            {
                if ("PanKnobTransform".Equals(e.PropertyName))
                {
                    notificationTransform = true;
                }
            });

            _vvm.ShowKnobCommand.Execute(null);

            Assert.IsTrue(notificationTransform);
        }

        [TestMethod]
        public void PanKnobViewModel_HideEmitsPropertyChange()
        {
            bool notificationTransform = false;

            _vvm.PropertyChanged += ((sender, e) =>
            {
                if ("PanKnobTransform".Equals(e.PropertyName))
                {
                    notificationTransform = true;
                }
            });

            _vvm.HideKnobCommand.Execute(null);

            Assert.IsTrue(notificationTransform);
        }


        [TestMethod]
        public void PanKnobViewModel_SinglePressChangeState()
        {
            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);

            ConsumeEvent(SingleTouchEvent());
            Assert.AreEqual(PanKnobState.Active, _vvm.State);

            ConsumeEvent(ReleaseTouchEvent());
            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);
        }

        [TestMethod]
        public void PanKnobViewModel_DoublePressChangeState()
        {
            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);

            ConsumeEvents(DoubleTouchEvents());
            Assert.AreEqual(PanKnobState.Moving, _vvm.State);

            ConsumeEvent(ReleaseTouchEvent());
            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);
        }

        [TestMethod]
        public void PanKnobViewModel_PanningDoesNotUpdatePosition()
        {
            bool notificationTransform = false;
            _vvm.PropertyChanged += ((sender, e) =>
            {
                if ("PanKnobTransform".Equals(e.PropertyName))
                {
                    notificationTransform = true;
                }
            });

            ConsumeEvent(SingleTouchEvent());
            Assert.AreEqual(PanKnobState.Active, _vvm.State);

            PointerEvent[] moves = new PointerEvent[]{
                MoveTouchEvent(12.5, 12.5),
                MoveTouchEvent(-12.5, 12.5),
                MoveTouchEvent(12.5, -12.5),
                MoveTouchEvent(-12.5, -12.5)
            };
            ConsumeEvents(moves);
            
            Assert.IsFalse(notificationTransform);

            ConsumeEvent(ReleaseTouchEvent());
            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);
        }

        [TestMethod]
        public void PanKnobViewModel_PanningEmitsPanUpdate()
        {
            bool notificationPanUpdate = false;
            int cnt = 0;

            _vvm.PanChange += ((sender, e) =>
            {
                notificationPanUpdate = true;
                cnt++;
            });

            ConsumeEvent(SingleTouchEvent());
            Assert.AreEqual(PanKnobState.Active, _vvm.State);

            PointerEvent[] moves = new PointerEvent[]{
                MoveTouchEvent(12.5, 12.5),
                MoveTouchEvent(-12.5, 12.5),
                MoveTouchEvent(12.5, -12.5),
                MoveTouchEvent(-12.5, -12.5)
            };
            ConsumeEvents(moves);

            Assert.IsTrue(notificationPanUpdate);
            Assert.AreEqual(moves.Length, cnt);

            ConsumeEvent(ReleaseTouchEvent());
            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);
        }

        [TestMethod]
        public void PanKnobViewModel_MovingUpdatesTransformProperty()
        {
            bool notificationTransform = false;
            int cnt = 0;

            _vvm.PropertyChanged += ((sender, e) =>
            {
                if ("PanKnobTransform".Equals(e.PropertyName))
                {
                    notificationTransform = true;
                    cnt++;
                }
            });

            ConsumeEvents(DoubleTouchEvents());
            Assert.AreEqual(PanKnobState.Moving, _vvm.State);

            PointerEvent[] moves = new PointerEvent[]{
                MoveTouchEvent(12.5, 12.5),
                MoveTouchEvent(-12.5, 12.5),
                MoveTouchEvent(12.5, -12.5),
                MoveTouchEvent(-12.5, -12.5)
            };
            ConsumeEvents(moves);
            
            Assert.IsTrue(notificationTransform);
            Assert.AreEqual(moves.Length, cnt);

            ConsumeEvent(ReleaseTouchEvent());
            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);
        }

        [TestMethod]
        public void PanKnobViewModel_MovingDoesNotEmitPanUpdate()
        {
            bool notificationPanUpdate = false;

            _vvm.PanChange += ((sender, e) =>
            {
                notificationPanUpdate = true;
            });

            ConsumeEvents(DoubleTouchEvents());
            Assert.AreEqual(PanKnobState.Moving, _vvm.State);

            PointerEvent[] moves = new PointerEvent[]{
                MoveTouchEvent(12.5, 12.5),
                MoveTouchEvent(-12.5, 12.5),
                MoveTouchEvent(12.5, -12.5),
                MoveTouchEvent(-12.5, -12.5)
            };
            ConsumeEvents(moves);

            Assert.IsFalse(notificationPanUpdate);

            ConsumeEvent(ReleaseTouchEvent());
            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);
        }

        [TestMethod]
        public void PanKnobViewModel_PanningWithInertia()
        {
            Assert.IsTrue(false, "Not implemented");
        }

        [TestMethod]
        public void PanKnobViewModel_MovingWithInertia()
        {
            Assert.IsTrue(false, "Not implemented");
        }

        [TestMethod]
        public void PanKnobViewModel_CannotMoveOutsideBorder()
        {
            Assert.IsTrue(false, "Not implemented");
        }


        /* ***********************************************************************
         *  Helper methods
         * */

        /// <summary>
        /// Helper method to send multiple pointer events
        /// </summary>
        /// <param name="events">list of events</param>
        protected void ConsumeEvents(PointerEvent[] events)
        {
            foreach (PointerEvent e in events)
            {
                _vvm.PointerEventConsumer.ConsumeEvent(e);
            }
        }

        /// <summary>
        /// Helper method to send single pointer event
        /// </summary>
        /// <param name="e">event</param>
        protected void ConsumeEvent(PointerEvent e)
        {
            _vvm.PointerEventConsumer.ConsumeEvent(e);
        }

        PointerEvent SingleTouchEvent()
        {
            return 
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, _pointerType, _pointerId, 100, TouchEventType.Down);
        }

        PointerEvent[] DoubleTouchEvents()
        {
            return new PointerEvent[] {
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, _pointerType, _pointerId, 100, TouchEventType.Down),
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, _pointerType, _pointerId, 200, TouchEventType.Down)
            };
        }

        PointerEvent MoveTouchEvent(double deltaX, double deltaY)
        {
            // delta matters, position does not matter
            return
                new PointerEvent(new Point(10.0, 20.0), false, new Point(deltaX, deltaY), false, false, _pointerType, _pointerId, 0, TouchEventType.Unknown);
        }

        PointerEvent ReleaseTouchEvent()
        {
            return
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), false, false, _pointerType, _pointerId, 1000, TouchEventType.Up);
        }

        PointerEvent BeginInertiaTouchEvent()
        {
            return
                new PointerEvent(new Point(0.0, 0.0), true, new Point(0.0, 0.0), true, false, _pointerType, _pointerId, 250, TouchEventType.Up);
        }

        PointerEvent CompleteInertiaTouchEvent()
        {
            return
                new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, _pointerType, _pointerId, 250, TouchEventType.Up);
        }
    }
}
