namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.ViewModels;

    [TestClass]
    public class BarItemModelTests
    {
        private class TestBarItemModel : BarItemModel
        {
            public TestBarItemModel(ItemAlignment alignment, bool isVisible) : base(alignment, isVisible) { }
            protected override void OnPresenting(object activationParameter)
            {
                //throw new System.NotImplementedException();
            }
        }

        [TestMethod]
        public void ConstructBarItemModel_PropertiesSetCorrectly()
        {
            BarItemModel model;

            model = new TestBarItemModel(BarItemModel.ItemAlignment.Left, true);
            Assert.AreEqual(model.Alignment, BarItemModel.ItemAlignment.Left);
            Assert.AreEqual(model.IsVisible, true);

            model = new TestBarItemModel(BarItemModel.ItemAlignment.Right, false);
            Assert.AreEqual(model.Alignment, BarItemModel.ItemAlignment.Right);
            Assert.AreEqual(model.IsVisible, false);
        }

        [TestMethod]
        public void ChangeIsVisibleProperty_ChangeReported()
        {
            bool newValue = false, changeReported = false;
            BarItemModel model = new TestBarItemModel(BarItemModel.ItemAlignment.Left, !newValue);

            model.PropertyChanged += (s, e) => { newValue = ((BarItemModel)s).IsVisible; changeReported = true; };
            Assert.AreNotEqual(false, model.IsVisible);
            model.IsVisible = false;
            Assert.IsFalse(newValue);
            Assert.IsTrue(changeReported);
            Assert.AreEqual(newValue, model.IsVisible);
        }

        [TestMethod]
        public void ChangeIsVisiblePropertyToSameValue_ChangeNotReported()
        {
            bool newValue = false, changeReported = false;
            BarItemModel model = new TestBarItemModel(BarItemModel.ItemAlignment.Left, newValue);

            model.PropertyChanged += (s, e) => { newValue = ((BarItemModel)s).IsVisible; changeReported = true; };
            Assert.AreEqual(false, model.IsVisible);
            model.IsVisible = false;
            Assert.IsFalse(newValue);
            Assert.IsFalse(changeReported);
            Assert.AreEqual(newValue, model.IsVisible);
        }
    }
}
