//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using RdClient.Shared.CxWrappers;
//using RdClient.Shared.ViewModels;
//using System;
//using System.Collections.Generic;

//namespace RdClient.Shared.Test.ViewModels
//{
//    [TestClass]
//    public class MouseViewModelTests
//    {

//        [TestMethod]
//        public void MouseViewModel_MousePointerChangedShouldSendMouseEvent()
//        {
//            RdpEventSource eventSource = new RdpEventSource();
//            using (Mock.RdpConnection connection = new Mock.RdpConnection(eventSource))
//            {
//                MouseViewModel mvm = new MouseViewModel();

//                mvm.RdpConnection = connection;

//                connection.Expect("SendMouseEvent", new List<object> { MouseEventType.MouseWheel, (float)0.5, (float)0.5 }, null);
//                mvm.MousePointer = new Tuple<int, float, float>(3, (float) 0.5, (float) 0.5);

//                Assert.AreEqual(new Tuple<float, float>((float) 0.5, (float) 0.5), mvm.MousePointerShapePosition);
//            }
//        }
//    }
//}
