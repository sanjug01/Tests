namespace RdClient.Shared.Test.Navigation
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Navigation;
    using System.ComponentModel;

    [TestClass]
    public sealed class ViewVisibilityTests
    {
        [TestMethod]
        public void NewViewVisibility_Visible_IsVisible()
        {
            IViewVisibility visibility = ViewVisibility.Create(true);
            Assert.IsTrue(visibility.IsVisible);
        }

        [TestMethod]
        public void NewViewVisibility_NotVisible_NotIsVisible()
        {
            IViewVisibility visibility = ViewVisibility.Create(false);
            Assert.IsFalse(visibility.IsVisible);
        }

        [TestMethod]
        public void NewViewVisibility_Default_NotIsVisible()
        {
            IViewVisibility visibility = ViewVisibility.Create();
            Assert.IsFalse(visibility.IsVisible);
        }

        [TestMethod]
        public void ViewVisibilityNotVisible_Show_SetVisibleOnce()
        {
            PropertyChangedEventArgs args = null;
            IViewVisibility visibility = ViewVisibility.Create();

            visibility.PropertyChanged += (sender, e) =>
            {
                Assert.IsNull(args);
                args = e;
            };

            visibility.Show();
            Assert.IsTrue(visibility.IsVisible);
            Assert.IsNotNull(args);
            Assert.AreEqual("IsVisible", args.PropertyName);
        }

        [TestMethod]
        public void ViewVisibilityVisible_Show_NotChanged()
        {
            IViewVisibility visibility = ViewVisibility.Create(true);

            visibility.PropertyChanged += (sender, e) =>
            {
                Assert.Fail();
            };

            visibility.Show();
            Assert.IsTrue(visibility.IsVisible);
        }

        [TestMethod]
        public void ViewVisibilityVisible_Hide_SetNotVisibleOnce()
        {
            PropertyChangedEventArgs args = null;
            IViewVisibility visibility = ViewVisibility.Create(true);

            visibility.PropertyChanged += (sender, e) =>
            {
                Assert.IsNull(args);
                args = e;
            };

            visibility.Hide();
            Assert.IsFalse(visibility.IsVisible);
            Assert.IsNotNull(args);
            Assert.AreEqual("IsVisible", args.PropertyName);
        }

        [TestMethod]
        public void ViewVisibilityNotVisible_Hide_NotChanged()
        {
            IViewVisibility visibility = ViewVisibility.Create();

            visibility.PropertyChanged += (sender, e) =>
            {
                Assert.Fail();
            };

            visibility.Hide();
            Assert.IsFalse(visibility.IsVisible);
        }
    }
}
