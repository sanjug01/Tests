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

        [TestInitialize]
        public void SetUpTest()
        {
            _vvm = new PanKnobViewModel();
            _vvm.ViewSize = new Size(_windowRect.Width, _windowRect.Height);
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
            bool notificationFired = false;

            _vvm.PropertyChanged += ((sender, e) =>
            {
                if ("PanKnobTransform".Equals(e.PropertyName))
                {
                    notificationFired = true;
                }
            });

            _vvm.ShowKnobCommand.Execute(null);

            Assert.IsTrue(notificationFired);
        }

        [TestMethod]
        public void PanKnobViewModel_HideEmitsPropertyChange()
        {
            bool notificationFired = false;

            _vvm.PropertyChanged += ((sender, e) =>
            {
                if ("PanKnobTransform".Equals(e.PropertyName))
                {
                    notificationFired = true;
                }
            });

            _vvm.HideKnobCommand.Execute(null);

            Assert.IsTrue(notificationFired);
        }


        // TODO: check state for various mouse events + inertia
        // TODO: check view size border


        /// <summary>
        /// Helper method to send multiple pointer eventes
        /// </summary>
        /// <param name="events">list of events</param>
        protected void ConsumeEvents(PointerEvent[] events)
        {
            foreach (PointerEvent e in events)
            {
                _vvm.PointerEventConsumer.ConsumeEvent(e);
            }
        }
    }
}
