using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Input.Pointer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace RdClient.Shared.Test.Input.Pointer
{
    [TestClass]
    public class PointerVisibilityConsumerTests
    {
        
        
        [TestMethod]
        public void PointerVisibilityConsumer_PointerExitedMouseInvisible()
        {
            using (Mock.RenderingPanel renderingPanel = new Mock.RenderingPanel())
            {
                PointerVisibilityConsumer pvc = new PointerVisibilityConsumer(renderingPanel);
                Mock.PointerRoutedEventArgsWrapper preaw = new Mock.PointerRoutedEventArgsWrapper()
                {
                    Action = PointerEventAction.PointerExited,
                    DeviceType = PointerDeviceType.Mouse
                };

                renderingPanel.Expect("ChangeMouseVisibility", new List<object> { Visibility.Collapsed }, null);
                pvc.Consume(preaw);
            }
        }

        [TestMethod]
        public void PointerVisibilityConsumer_PointerEnteredVisible()
        {
            using (Mock.RenderingPanel renderingPanel = new Mock.RenderingPanel())
            {
                PointerVisibilityConsumer pvc = new PointerVisibilityConsumer(renderingPanel);
                Mock.PointerRoutedEventArgsWrapper preaw = new Mock.PointerRoutedEventArgsWrapper()
                {
                    Action = PointerEventAction.PointerEntered,
                    DeviceType = PointerDeviceType.Mouse
                };

                renderingPanel.Expect("ChangeMouseVisibility", new List<object> { Visibility.Visible }, null);
                pvc.Consume(preaw);
            }
        }

        [TestMethod]
        public void PointerVisibilityConsumer_TouchModeInvisible()
        {
            using (Mock.RenderingPanel renderingPanel = new Mock.RenderingPanel())
            {
                PointerVisibilityConsumer pvc = new PointerVisibilityConsumer(renderingPanel);
                pvc.SetConsumptionMode(ConsumptionModeType.MultiTouch);
                Mock.PointerRoutedEventArgsWrapper preaw = new Mock.PointerRoutedEventArgsWrapper()
                {
                    Action = PointerEventAction.PointerEntered,
                    DeviceType = PointerDeviceType.Touch,
                };

                renderingPanel.Expect("ChangeMouseVisibility", new List<object> { Visibility.Collapsed }, null);
                pvc.Consume(preaw);
            }
        }

        [TestMethod]
        public void PointerVisibilityConsumer_IgnoreTouchExit()
        {
            using (Mock.RenderingPanel renderingPanel = new Mock.RenderingPanel())
            {
                PointerVisibilityConsumer pvc = new PointerVisibilityConsumer(renderingPanel);
                Mock.PointerRoutedEventArgsWrapper preaw = new Mock.PointerRoutedEventArgsWrapper()
                {
                    Action = PointerEventAction.PointerExited,
                    DeviceType = PointerDeviceType.Touch,
                };

                pvc.Consume(preaw);
            }
        }
    }
}
