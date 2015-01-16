//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using RdClient.Shared.Helpers;
//using RdClient.Shared.Input.Mouse;
//using System.Collections.Generic;
//using Windows.Foundation;
//using MousePointer = System.Tuple<int, float, float>;

//namespace RdClient.Shared.Test.Model
//{
//    [TestClass]
//    public class PointerModeTests
//    {
//        public class TestTimer : ITimer
//        {
//            System.Action _callback;

//            public void TriggerCallback()
//            {
//                if(_callback != null)
//                {
//                    _callback();
//                }
//            }

//            public void Start(System.Action callback, System.TimeSpan period, bool recurring)
//            {
//                _callback = callback;
//            }

//            public void Stop()
//            {
                
//            }
//        }

//        [TestMethod]
//        public void PointerModel_ShouldMove()
//        {
//            MousePointer mousePointer = new MousePointer(0, (float) 0.0, (float) 0.0);
//            TestTimer timer = new TestTimer();
//            PointerEventConsumer pointerModel = new PointerEventConsumer(timer);
//            pointerModel.WindowSize = new Size(250.0, 250.0);
//            pointerModel.MousePointerChanged += (s, o) => { mousePointer = (o as MousePointer); };

//            PointerEvent pe1 = new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3);
//            PointerEvent pe2 = new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3);

//            pointerModel.ConsumeEvent(pe1);
//            pointerModel.ConsumeEvent(pe2);

//            Assert.AreEqual(4, mousePointer.Item1);
//            Assert.AreEqual(10.0, mousePointer.Item2);
//            Assert.AreEqual(10.0, mousePointer.Item3);
//        }

//        [TestMethod]
//        public void PointerModel_ShouldMove_Clamp()
//        {
//            List<MousePointer> mousePointers = new List<MousePointer>();
//            TestTimer timer = new TestTimer();
//            PointerEventConsumer pointerModel = new PointerEventConsumer(timer);
//            pointerModel.WindowSize = new Size(250.0, 250.0);
//            pointerModel.MousePointerChanged += (s, o) => { mousePointers.Add(o as MousePointer); };

//            List<PointerEvent> pointerEvents = new List<PointerEvent>();

//            pointerEvents.Add(new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3));
//            pointerEvents.Add(new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3));
//            pointerEvents.Add(new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3));
//            pointerEvents.Add(new PointerEvent(new Point(1.0, 1.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3));
//            pointerEvents.Add(new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3));
//            pointerEvents.Add(new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3));

//            foreach (PointerEvent pe in pointerEvents)
//            {
//                pointerModel.ConsumeEvent(pe);
//            }

//            Assert.AreEqual(new MousePointer(0, (float)1.0, (float)1.0), mousePointers[0]);
//            Assert.AreEqual(new MousePointer(4, (float)10.0, (float)10.0), mousePointers[1]);
//            Assert.AreEqual(new MousePointer(1, (float)10.0, (float)10.0), mousePointers[2]);
//            Assert.AreEqual(3, mousePointers.Count);
//        }

//        [TestMethod]
//        public void PointerModel_ShouldMoveOnce()
//        {
//            MousePointer mousePointer = new MousePointer(0, (float)0.0, (float)0.0);
//            int mousePointerChanges = 0;           
//            TestTimer timer = new TestTimer();
//            PointerEventConsumer pointerModel = new PointerEventConsumer(timer);
//            pointerModel.WindowSize = new Size(250.0, 250.0);
//            pointerModel.MousePointerChanged += (s, o) => { mousePointer = (o as MousePointer); mousePointerChanges++;  };

//            // finger down
//            PointerEvent pe1 = new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3);

//            // move finger
//            PointerEvent pe2 = new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3);

//            // lift finger
//            PointerEvent pe3 = new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3);

//            pointerModel.ConsumeEvent(pe1);
//            pointerModel.ConsumeEvent(pe2);
//            pointerModel.ConsumeEvent(pe3);

//            Assert.AreEqual(4, mousePointer.Item1);
//            Assert.AreEqual(10.0, mousePointer.Item2);
//            Assert.AreEqual(10.0, mousePointer.Item3);
//            Assert.AreEqual(1, mousePointerChanges);
//        }

//        [TestMethod]
//        public void PointerModel_ShouldMoveWithInertia()
//        {
//            MousePointer mousePointer = new MousePointer(0, (float)0.0, (float)0.0);
//            int mousePointerChanges = 0;
//            TestTimer timer = new TestTimer();
//            PointerEventConsumer pointerModel = new PointerEventConsumer(timer);
//            pointerModel.WindowSize = new Size(250.0, 250.0);
//            pointerModel.MousePointerChanged += (s, o) => { mousePointer = (o as MousePointer); mousePointerChanges++; };

//            // finger down
//            PointerEvent pe1 = new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3);

//            // move finger
//            PointerEvent pe2 = new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3);

//            // lift finger
//            PointerEvent pe3 = new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3);

//            // inertia
//            PointerEvent pe4 = new PointerEvent(new Point(0.0, 0.0), true, new Point(10.0, 10.0), false, false, PointerType.Touch, 3);

//            // inertia
//            PointerEvent pe5 = new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3);

//            pointerModel.ConsumeEvent(pe1);
//            pointerModel.ConsumeEvent(pe2);
//            pointerModel.ConsumeEvent(pe3);
//            pointerModel.ConsumeEvent(pe4);
//            pointerModel.ConsumeEvent(pe5);

//            Assert.AreEqual(4, mousePointer.Item1);
//            Assert.AreEqual(20.0, mousePointer.Item2);
//            Assert.AreEqual(20.0, mousePointer.Item3);
//            Assert.AreEqual(2, mousePointerChanges);
//        }

//        [TestMethod]
//        public void PointerModel_ShouldLeftDrag()
//        {
//            List<MousePointer> mousePointers = new List<MousePointer>();
//            TestTimer timer = new TestTimer();
//            PointerEventConsumer pointerModel = new PointerEventConsumer(timer);
//            pointerModel.WindowSize = new Size(250.0, 250.0);
//            pointerModel.MousePointerChanged += (s, o) => { mousePointers.Add(o as MousePointer); };

//            List<PointerEvent> pointerEvents = new List<PointerEvent>();

//            pointerEvents.Add(new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3));
//            pointerEvents.Add(new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3));
//            pointerEvents.Add(new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3));
//            pointerEvents.Add(new PointerEvent(new Point(1.0, 1.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3));
//            pointerEvents.Add(new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3));
//            pointerEvents.Add(new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3));

//            foreach(PointerEvent pe in pointerEvents)
//            {
//                pointerModel.ConsumeEvent(pe);
//            }

//            Assert.AreEqual(new MousePointer(0, (float)1.0, (float)1.0), mousePointers[0]);
//            Assert.AreEqual(new MousePointer(4, (float)10.0, (float)10.0), mousePointers[1]);
//            Assert.AreEqual(new MousePointer(1, (float)10.0, (float)10.0), mousePointers[2]);
//            Assert.AreEqual(3, mousePointers.Count);
//        }

//        [TestMethod]
//        public void PointerModel_ShouldRightDrag()
//        {
//            List<MousePointer> mousePointers = new List<MousePointer>();
//            TestTimer timer = new TestTimer();
//            PointerEventConsumer pointerModel = new PointerEventConsumer(timer);
//            pointerModel.WindowSize = new Size(250.0, 250.0);
//            pointerModel.MousePointerChanged += (s, o) => { mousePointers.Add(o as MousePointer); };

//            List<PointerEvent> pointerEvents = new List<PointerEvent>();

//            // right tap 1
//            pointerEvents.Add(new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3));
//            pointerEvents.Add(new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4));

//            // right tap 2
//            pointerEvents.Add(new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 4));
//            pointerEvents.Add(new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4));

//            // drag
//            pointerEvents.Add(new PointerEvent(new Point(1.0, 1.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3));
//            pointerEvents.Add(new PointerEvent(new Point(1.0, 1.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4));

//            // drag
//            pointerEvents.Add(new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 3));
//            pointerEvents.Add(new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Touch, 4));

//            // release
//            pointerEvents.Add(new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 3));
//            pointerEvents.Add(new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Touch, 4));

//            foreach (PointerEvent pe in pointerEvents)
//            {
//                pointerModel.ConsumeEvent(pe);
//            }

//            Assert.AreEqual(new MousePointer(5, (float)1.0, (float)1.0), mousePointers[0]);
//            Assert.AreEqual(new MousePointer(4, (float)10.0, (float)10.0), mousePointers[1]);
//            Assert.AreEqual(new MousePointer(6, (float)10.0, (float)10.0), mousePointers[2]);
//            Assert.AreEqual(3, mousePointers.Count);
//        }

//        [TestMethod]
//        public void PointerModel_Mouse_ShouldLeftDrag()
//        {
//            List<MousePointer> mousePointers = new List<MousePointer>();
//            TestTimer timer = new TestTimer();
//            PointerEventConsumer pointerModel = new PointerEventConsumer(timer);
//            pointerModel.WindowSize = new Size(250.0, 250.0);
//            pointerModel.MousePointerChanged += (s, o) => { mousePointers.Add(o as MousePointer); };

//            List<PointerEvent> pointerEvents = new List<PointerEvent>();

//            pointerEvents.Add(new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), true, false, PointerType.Mouse, 3));
//            pointerEvents.Add(new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), true, false, PointerType.Mouse, 3));
//            pointerEvents.Add(new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Mouse, 3));

//            foreach (PointerEvent pe in pointerEvents)
//            {
//                pointerModel.ConsumeEvent(pe);
//            }

//            Assert.AreEqual(new MousePointer(0, (float)0.0, (float)0.0), mousePointers[0]);
//            Assert.AreEqual(new MousePointer(4, (float)10.0, (float)10.0), mousePointers[1]);
//            Assert.AreEqual(new MousePointer(1, (float)10.0, (float)10.0), mousePointers[2]);
//            Assert.AreEqual(3, mousePointers.Count);
//        }

//        [TestMethod]
//        public void PointerModel_Mouse_ShouldRightDrag()
//        {
//            List<MousePointer> mousePointers = new List<MousePointer>();
//            TestTimer timer = new TestTimer();
//            PointerEventConsumer pointerModel = new PointerEventConsumer(timer);
//            pointerModel.WindowSize = new Size(250.0, 250.0);
//            pointerModel.MousePointerChanged += (s, o) => { mousePointers.Add(o as MousePointer); };

//            List<PointerEvent> pointerEvents = new List<PointerEvent>();

//            pointerEvents.Add(new PointerEvent(new Point(0.0, 0.0), false, new Point(0.0, 0.0), false, true, PointerType.Mouse, 3));
//            pointerEvents.Add(new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, true, PointerType.Mouse, 3));
//            pointerEvents.Add(new PointerEvent(new Point(10.0, 10.0), false, new Point(0.0, 0.0), false, false, PointerType.Mouse, 3));

//            foreach (PointerEvent pe in pointerEvents)
//            {
//                pointerModel.ConsumeEvent(pe);
//            }

//            Assert.AreEqual(new MousePointer(5, (float)0.0, (float)0.0), mousePointers[0]);
//            Assert.AreEqual(new MousePointer(4, (float)10.0, (float)10.0), mousePointers[1]);
//            Assert.AreEqual(new MousePointer(6, (float)10.0, (float)10.0), mousePointers[2]);
//            Assert.AreEqual(3, mousePointers.Count);
//        }
//    }
//}
