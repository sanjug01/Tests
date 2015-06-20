namespace RdClient.Shared.Test.Helpers
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.ViewModels;
    using RdMock;
    using System.Collections.Generic;

    [TestClass]
    public sealed class ViewItemsSourceTests
    {
        private sealed class TestItemsView : MockBase, IItemsView
        {
            void IItemsView.SelectItem(IViewItemsSource itemsSource, object item)
            {
                base.Invoke(new object[] { itemsSource, item });
            }
        }

        [TestMethod]
        public void ViewItemsSource_SelectItem_CallsView()
        {
            ViewItemsSource vis = new ViewItemsSource();
            IViewItemsSource ivis = vis;

            using (TestItemsView tiv = new TestItemsView())
            {
                object item = new object();
                tiv.Expect("SelectItem", new List<object>() { ivis, item }, null);

                ivis.SetItemsView(tiv);
                vis.SelectItem(item);
            }
        }
    }
}
