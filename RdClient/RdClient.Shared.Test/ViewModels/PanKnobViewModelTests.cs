//using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
//using RdClient.Shared.CxWrappers;
//using RdClient.Shared.Input.Pointer;
//using RdClient.Shared.Input.ZoomPan;
//using RdClient.Shared.Helpers;
//using RdClient.Shared.ViewModels;
//using System.Collections.Generic;
//using Windows.Foundation;
//using Windows.UI.Xaml.Media;

//namespace RdClient.Shared.Test.ViewModels
//{
//    [TestClass]
//    public class PanKnobViewModelTests
//    {

//        Rect _windowRect = new Rect(0, 0, 1280, 800);
//        PanKnobViewModel _vvm;
//        uint _pointerId;
//        PointerTypeOld _pointerType;

//        [TestInitialize]
//        public void SetUpTest()
//        {
//            _vvm = new PanKnobViewModel();
//            _vvm.ViewSize = new Size(_windowRect.Width, _windowRect.Height);
//            _pointerId = 3;
//            _pointerType = PointerTypeOld.Touch;
//        }

//        [TestCleanup]
//        public void TearDownTest()
//        {
//            _vvm = null;
//        }

//        [TestMethod]
//        public void PanKnobViewModel_DefaultStateIsInactive()
//        {
//            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);               
//        }

//        [TestMethod]
//        public void PanKnobViewModel_DefaultTransformParamsNotInitialized()
//        {
//            Assert.AreEqual(0, _vvm.TranslateXFrom);
//            Assert.AreEqual(0, _vvm.TranslateYFrom);

//            Assert.AreEqual(0, _vvm.TranslateXTo);
//            Assert.AreEqual(0, _vvm.TranslateYTo);

//            Assert.IsTrue(_vvm.PanControlOpacity > 0);
//            Assert.IsTrue(_vvm.PanOrbOpacity > 0);
//        }

//        [TestMethod]
//        public void PanKnobViewModel_ShowEmitsPropertyChange()
//        {
//            bool notificationTransform = false;

//            _vvm.PropertyChanged += ((sender, e) =>
//            {
//                if ("PanKnobTransform".Equals(e.PropertyName))
//                {
//                    notificationTransform = true;
//                }
//            });

//            _vvm.ShowKnobCommand.Execute(null);

//            Assert.IsTrue(notificationTransform);
//            Assert.AreEqual(PanKnobTransformType.ShowKnob, _vvm.PanKnobTransform.TransformType);
//        }

//        [TestMethod]
//        public void PanKnobViewModel_HideEmitsPropertyChange()
//        {
//            bool notificationTransform = false;

//            _vvm.PropertyChanged += ((sender, e) =>
//            {
//                if ("PanKnobTransform".Equals(e.PropertyName))
//                {
//                    notificationTransform = true;
//                }
//            });

//            _vvm.HideKnobCommand.Execute(null);

//            Assert.IsTrue(notificationTransform);
//            Assert.AreEqual(PanKnobTransformType.HideKnob, _vvm.PanKnobTransform.TransformType);
//        }


//        [TestMethod]
//        public void PanKnobViewModel_SinglePressChangeState()
//        {
//            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);

//            ConsumeEvent(SingleTouchEvent());
//            Assert.AreEqual(PanKnobState.Active, _vvm.State);

//            ConsumeEvent(ReleaseTouchEvent());
//            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);
//        }

//        [TestMethod]
//        public void PanKnobViewModel_SinglePressEnablesPanning()
//        {
//            // by default the panning layer is not enabled
//            Assert.IsFalse(_vvm.IsPanning);

//            ConsumeEvent(SingleTouchEvent());
//            Assert.IsTrue(_vvm.IsPanning);

//            ConsumeEvent(ReleaseTouchEvent());
//            Assert.IsFalse(_vvm.IsPanning);
//        }

//        [TestMethod]
//        public void PanKnobViewModel_DoublePressChangeState()
//        {
//            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);

//            ConsumeEvents(DoubleTouchEvents());
//            Assert.AreEqual(PanKnobState.Moving, _vvm.State);

//            ConsumeEvent(ReleaseTouchEvent());
//            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);
//        }

//        public void PanKnobViewModel_DoublePressEnablesPanning()
//        {
//            // by default the panning layer is not enabled
//            Assert.IsFalse(_vvm.IsPanning);

//            ConsumeEvents(DoubleTouchEvents());
//            Assert.IsTrue(_vvm.IsPanning);

//            ConsumeEvent(ReleaseTouchEvent());
//            Assert.IsFalse(_vvm.IsPanning);
//        }

//        [TestMethod]
//        public void PanKnobViewModel_PanningDoesNotUpdatePosition()
//        {
//            bool notificationTransform = false;
//            _vvm.PropertyChanged += ((sender, e) =>
//            {
//                if ("PanKnobTransform".Equals(e.PropertyName))
//                {
//                    notificationTransform = true;
//                }
//            });

//            ConsumeEvent(SingleTouchEvent());
//            Assert.AreEqual(PanKnobState.Active, _vvm.State);

//            PointerEventOld[] moves = new PointerEventOld[]{
//                MoveTouchEvent(12.5, 12.5),
//                MoveTouchEvent(-12.5, 12.5),
//                MoveTouchEvent(12.5, -12.5),
//                MoveTouchEvent(-12.5, -12.5)
//            };
//            ConsumeEvents(moves);
            
//            Assert.IsFalse(notificationTransform);

//            ConsumeEvent(ReleaseTouchEvent());
//            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);
//        }

//        [TestMethod]
//        public void PanKnobViewModel_PanningEmitsPanUpdate()
//        {
//            bool notificationPanUpdate = false;
//            int cntNotifications = 0;

//            _vvm.PanChange += ((sender, e) =>
//            {
//                notificationPanUpdate = true;
//                cntNotifications++;
//            });

//            ConsumeEvent(SingleTouchEvent());
//            Assert.AreEqual(PanKnobState.Active, _vvm.State);

//            PointerEventOld[] moves = new PointerEventOld[]{
//                MoveTouchEvent(12.5, 12.5),
//                MoveTouchEvent(-12.5, 12.5),
//                MoveTouchEvent(12.5, -12.5),
//                MoveTouchEvent(-12.5, -12.5)
//            };
//            ConsumeEvents(moves);

//            Assert.IsTrue(notificationPanUpdate);
//            Assert.AreEqual(moves.Length, cntNotifications);

//            ConsumeEvent(ReleaseTouchEvent());
//            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);
//        }

//        [TestMethod]
//        public void PanKnobViewModel_MovingUpdatesTransformProperty()
//        {
//            bool notificationTransform = false;
//            int cntNotifications = 0;

//            _vvm.PropertyChanged += ((sender, e) =>
//            {
//                if ("PanKnobTransform".Equals(e.PropertyName))
//                {
//                    notificationTransform = true;
//                    cntNotifications++;
//                }
//            });

//            ConsumeEvents(DoubleTouchEvents());
//            Assert.AreEqual(PanKnobState.Moving, _vvm.State);

//            PointerEventOld[] moves = new PointerEventOld[]{
//                MoveTouchEvent(12.5, 12.5),
//                MoveTouchEvent(-12.5, 12.5),
//                MoveTouchEvent(12.5, -12.5),
//                MoveTouchEvent(-12.5, -12.5)
//            };
//            ConsumeEvents(moves);
            
//            Assert.IsTrue(notificationTransform);
//            Assert.AreEqual(moves.Length, cntNotifications);
//            Assert.AreEqual(PanKnobTransformType.MoveKnob, _vvm.PanKnobTransform.TransformType);

//            ConsumeEvent(ReleaseTouchEvent());
//            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);
//        }

//        [TestMethod]
//        public void PanKnobViewModel_MovingDoesNotEmitPanUpdate()
//        {
//            bool notificationPanUpdate = false;

//            _vvm.PanChange += ((sender, e) =>
//            {
//                notificationPanUpdate = true;
//            });

//            ConsumeEvents(DoubleTouchEvents());
//            Assert.AreEqual(PanKnobState.Moving, _vvm.State);

//            PointerEventOld[] moves = new PointerEventOld[]{
//                MoveTouchEvent(12.5, 12.5),
//                MoveTouchEvent(-12.5, 12.5),
//                MoveTouchEvent(12.5, -12.5),
//                MoveTouchEvent(-12.5, -12.5)
//            };
//            ConsumeEvents(moves);

//            Assert.IsFalse(notificationPanUpdate);

//            ConsumeEvent(ReleaseTouchEvent());
//            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);
//        }

//        [TestMethod]
//        public void PanKnobViewModel_PanningWithInertia()
//        {
//            PointerEventOld[] movesBefore = new PointerEventOld[]{
//                InertiaMoveTouchEvent(12.5, 12.5),
//                InertiaMoveTouchEvent(-12.5, 12.5),
//            };

//            PointerEventOld[] movesAfter = new PointerEventOld[]{
//                InertiaMoveTouchEvent(-5.5, 5.5),
//                InertiaMoveTouchEvent(-11.0, 11.0),
//            };

//            bool notificationPanUpdate = false;
//            int cntNotifications = 0;
//            // one extra notification for begin inertia
//            int expNotifications = 1 + movesBefore.Length + movesAfter.Length;

//            _vvm.PanChange += ((sender, e) =>
//            {
//                notificationPanUpdate = true;
//                cntNotifications++;
//            });


//            ConsumeEvent(SingleTouchEvent());
//            Assert.AreEqual(PanKnobState.Active, _vvm.State);

//            ConsumeEvent(BeginInertiaTouchEvent());
//            Assert.AreEqual(PanKnobState.Active, _vvm.State);

//            ConsumeEvents(movesBefore);
//            Assert.AreEqual(PanKnobState.Active, _vvm.State);

//            // release with inertia will keep moving
//            ConsumeEvent(ReleaseTouchEvent());
//            Assert.AreEqual(PanKnobState.Active, _vvm.State);

//            ConsumeEvents(movesAfter);
//            Assert.AreEqual(PanKnobState.Active, _vvm.State);


//            ConsumeEvent(CompleteInertiaTouchEvent());
//            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);

//            Assert.IsTrue(notificationPanUpdate);
//            Assert.AreEqual(expNotifications, cntNotifications);
//        }

//        [TestMethod]
//        public void PanKnobViewModel_MovingWithInertia()
//        {
//            PointerEventOld[] movesBefore = new PointerEventOld[]{
//                InertiaMoveTouchEvent(12.5, 12.5),
//                InertiaMoveTouchEvent(-12.5, 12.5),
//            };

//            PointerEventOld[] movesAfter = new PointerEventOld[]{
//                InertiaMoveTouchEvent(-5.5, 5.5),
//                InertiaMoveTouchEvent(-11.0, 11.0),
//            };

//            bool notificationTransform = false;
//            int cntNotifications = 0;
//            // one extra notification for begin inertia
//            int expNotifications = 1 + movesBefore.Length + movesAfter.Length;

//            _vvm.PropertyChanged += ((sender, e) =>
//            {
//                if ("PanKnobTransform".Equals(e.PropertyName))
//                {
//                    notificationTransform = true;
//                    cntNotifications++;
//                }
//            });

//            ConsumeEvents(DoubleTouchEvents());
//            Assert.AreEqual(PanKnobState.Moving, _vvm.State);

//            ConsumeEvent(BeginInertiaTouchEvent());
//            Assert.AreEqual(PanKnobState.Moving, _vvm.State);

//            ConsumeEvents(movesBefore);
//            Assert.AreEqual(PanKnobState.Moving, _vvm.State);

//            // release with inertia will keep moving
//            ConsumeEvent(ReleaseTouchEvent());
//            Assert.AreEqual(PanKnobState.Moving, _vvm.State);

//            ConsumeEvents(movesAfter);
//            Assert.AreEqual(PanKnobState.Moving, _vvm.State);


//            ConsumeEvent(CompleteInertiaTouchEvent());
//            Assert.AreEqual(PanKnobState.Inactive, _vvm.State);

//            Assert.IsTrue(notificationTransform);
//            Assert.AreEqual(expNotifications, cntNotifications);
//        }

//        [TestMethod]
//        public void PanKnobViewModel_CannotMoveOutsideBorder()
//        {
//            double borderLeft = -(_windowRect.Width - GlobalConstants.PanKnobWidth) / 2.0;
//            double borderRight = -(_windowRect.Width - GlobalConstants.PanKnobWidth) / 2.0;
//            double borderTop = -(_windowRect.Height - GlobalConstants.PanKnobHeight) / 2.0;
//            double borderBottom = -(_windowRect.Height - GlobalConstants.PanKnobHeight) / 2.0;

//            double deltaX, deltaY;

//            ConsumeEvents(DoubleTouchEvents());
//            Assert.AreEqual(PanKnobState.Moving, _vvm.State);

//            // left overflow
//            deltaX = 2 * borderLeft;
//            deltaY = 0;
//            ConsumeEvent(MoveTouchEvent(deltaX, deltaY));
//            Assert.IsTrue(_vvm.TranslateXTo == borderLeft);

//            // right overflow
//            deltaX = -borderLeft + 2 * borderRight;
//            deltaY = 0;
//            ConsumeEvent(MoveTouchEvent(deltaX, deltaY));
//            Assert.IsTrue(_vvm.TranslateXTo == borderRight);

//            // revert
//            ConsumeEvent(MoveTouchEvent(-borderRight, deltaY));

//            // top overflow
//            deltaX = 0;
//            deltaY = 2 * borderTop;
//            ConsumeEvent(MoveTouchEvent(deltaX, deltaY));
//            Assert.IsTrue(_vvm.TranslateYTo == borderTop);

//            // bottom overflow
//            deltaX = 0;
//            deltaY = -borderTop + 2 * borderBottom;
//            ConsumeEvent(MoveTouchEvent(deltaX, deltaY));
//            Assert.IsTrue(_vvm.TranslateYTo == borderBottom);

//            // revert
//            ConsumeEvent(MoveTouchEvent(deltaX, -borderBottom));

//            // double overflow
//            deltaX = 2 * borderRight;
//            deltaY = 2 * borderBottom;
//            ConsumeEvent(MoveTouchEvent(deltaX, deltaY));
//            Assert.IsTrue(_vvm.TranslateXTo == borderRight);
//            Assert.IsTrue(_vvm.TranslateYTo == borderBottom);
//        }


//        /* ***********************************************************************
//         *  Helper methods
//         * */

//        /// <summary>
//        /// Helper method to send multiple pointer events
//        /// </summary>
//        /// <param name="events">list of events</param>
//        protected void ConsumeEvents(PointerEventOld[] events)
//        {
//            foreach (PointerEventOld e in events)
//            {
//                _vvm.PointerEventConsumer.ConsumeEvent(e);
//            }
//        }

//        /// <summary>
//        /// Helper method to send single pointer event
//        /// </summary>
//        /// <param name="e">event</param>
//        protected void ConsumeEvent(PointerEventOld e)
//        {
//            _vvm.PointerEventConsumer.ConsumeEvent(e);
//        }

//        PointerEventOld SingleTouchEvent()
//        {
//            return 
//                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, _pointerType, _pointerId, 100, TouchEventType.Down);
//        }

//        PointerEventOld[] DoubleTouchEvents()
//        {
//            return new PointerEventOld[] {
//                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, _pointerType, _pointerId, 100, TouchEventType.Down),
//                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, _pointerType, _pointerId, 200, TouchEventType.Down)
//            };
//        }

//        PointerEventOld MoveTouchEvent(double deltaX, double deltaY)
//        {
//            // delta matters, position does not matter
//            return
//                new PointerEventOld(new Point(10.0, 20.0), false, new Point(deltaX, deltaY), false, false, _pointerType, _pointerId, 0, TouchEventType.Unknown);
//        }

//        PointerEventOld ReleaseTouchEvent()
//        {
//            return
//                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0.0, 0.0), false, false, _pointerType, _pointerId, 1000, TouchEventType.Up);
//        }

//        PointerEventOld BeginInertiaTouchEvent()
//        {
//            return
//                new PointerEventOld(new Point(0.0, 0.0), true, new Point(0.0, 0.0), false, false, _pointerType, _pointerId, 0, TouchEventType.Unknown);
//        }

//        PointerEventOld InertiaMoveTouchEvent(double deltaX, double deltaY)
//        {
//            // delta matters, position does not matter
//            return
//                new PointerEventOld(new Point(10.0, 20.0), true, new Point(deltaX, deltaY), false, false, _pointerType, _pointerId, 0, TouchEventType.Unknown);
//        }

//        PointerEventOld CompleteInertiaTouchEvent()
//        {
//            return
//                new PointerEventOld(new Point(0.0, 0.0), false, new Point(0, 0), false, false, _pointerType, _pointerId, 0, TouchEventType.Unknown);
//        }
//    }
//}
