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
            using (Mock.RdpConnection connection = new Mock.RdpConnection(eventSource))
            {
                MouseViewModel mvm = new MouseViewModel();

                mvm.RdpConnection = connection;

                connection.Expect("SendMouseEvent", new List<object> { MouseEventType.MouseWheel, (float)10.0, (float)10.0 }, null);

                mvm.MousePosition = new Point(10.0, 10.0);
                mvm.SendMouseAction(MouseEventType.MouseWheel);

                Assert.AreEqual(new Point(10.0, 10.0), mvm.MousePosition);
            }
        }
    }
}
