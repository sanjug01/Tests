using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;
using Windows.Foundation;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class MouseViewModelTests
    {

        [TestMethod]
        public void MouseViewModel_MousePointerChangedShouldSendMouseEvent()
        {
            RdpEventSource eventSource = new RdpEventSource();
            using(Mock.RdpConnection connection = new Mock.RdpConnection(eventSource))
            using(Mock.ExecutionDeferrer deferrer = new Mock.ExecutionDeferrer())
            {
                MouseViewModel mvm = new MouseViewModel();
                mvm.ViewSize = new Size(20.0, 20.0);
                mvm.DeferredExecution = deferrer;
                mvm.RdpConnection = connection;

                deferrer.Expect("DeferToUI", new List<object> { null }, null);
                connection.Expect("SendMouseEvent", new List<object> { MouseEventType.Move, (float)10.0, (float)10.0 }, null);

                mvm.MousePosition = new Point(10.0, 10.0);
                mvm.SendMouseAction(MouseEventType.Move);

                Assert.AreEqual(new Point(10.0, 10.0), mvm.MousePosition);
            }
        }

        [TestMethod]
        public void MouseViewModel_MousePositionClamping()
        {
            using(Mock.ExecutionDeferrer deferrer = new Mock.ExecutionDeferrer())
            {
                MouseViewModel mvm = new MouseViewModel();
                mvm.DeferredExecution = deferrer;
                mvm.ViewSize = new Size(10.0, 10.0);

                deferrer.Expect("DeferToUI", new List<object> { null }, null);
                mvm.MousePosition = new Point(20.0, 20.0);
                Assert.AreEqual(new Point(10.0, 10.0), mvm.MousePosition);

                deferrer.Expect("DeferToUI", new List<object> { null }, null);
                mvm.MousePosition = new Point(-5.0, -5.0);
                Assert.AreEqual(new Point(0.0, 0.0), mvm.MousePosition);
            }
        }
    }
}
