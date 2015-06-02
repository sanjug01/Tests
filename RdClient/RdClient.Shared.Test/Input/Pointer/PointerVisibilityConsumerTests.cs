using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Input.Pointer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace RdClient.Shared.Test.Input.Pointer
{
    [TestClass]
    public class PointerVisibilityConsumerTests
    {
        [TestMethod]
        public void PointerVisibilityConsume_SomeTest()
        {
            using (Mock.RenderingPanel renderingPanel = new Mock.RenderingPanel())
            {
                PointerVisibilityConsumer pvc = new PointerVisibilityConsumer(renderingPanel);
                Mock.PointerEventBase peb = new Mock.PointerEventBase(PointerEventAction.PointerEntered, new Point(0, 0));

                renderingPanel.Expect("ChangeMouseVisibility", new List<object> { Visibility.Visible }, null);
                pvc.Consume(peb);
                
            }
        }
    }
}
