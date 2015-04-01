namespace RdClient.Shared.Test.Converters
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Converters;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Collections.Generic;

    [TestClass]
    public sealed class BarItemsAlignmentFilteringConverterTests : DisposableObject
    {
        private BarItemModel[] _models;
        private BarItemsAlignmentFilteringConverter _converter;

        [TestInitialize]
        public void SetUpTest()
        {
            _models = new BarItemModel[]
            {
                new SeparatorBarItemModel(BarItemModel.ItemAlignment.Left),
                new SeparatorBarItemModel(BarItemModel.ItemAlignment.Right),
                new SeparatorBarItemModel(BarItemModel.ItemAlignment.Left),
                new SeparatorBarItemModel(BarItemModel.ItemAlignment.Right)
            };

            _converter = new BarItemsAlignmentFilteringConverter();
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _models = null;
        }

        [TestMethod]
        public void NewConverter_Convert_SameCollectionReturned()
        {
            Assert.AreSame(_models, _converter.Convert(_models, typeof(object), null, null));
        }

        [TestMethod]
        public void LeftAlignedConverter_Convert_LeftAlignedItemsReturned()
        {
            int count = 0;

            _converter.Alignment = BarItemModel.ItemAlignment.Left;
            Assert.AreEqual(BarItemModel.ItemAlignment.Left, _converter.Alignment);
            IEnumerable<BarItemModel> converted = _converter.Convert(_models, typeof(object), null, null) as IEnumerable<BarItemModel>;
            Assert.IsNotNull(converted);
            foreach(BarItemModel model in converted)
            {
                Assert.AreEqual(BarItemModel.ItemAlignment.Left, model.Alignment);
                ++count;
            }
            Assert.AreNotEqual(0, count);
        }

        [TestMethod]
        public void RightAlignedConverter_Convert_LeftAlignedItemsReturned()
        {
            int count = 0;

            _converter.Alignment = BarItemModel.ItemAlignment.Right;
            Assert.AreEqual(BarItemModel.ItemAlignment.Right, _converter.Alignment);
            IEnumerable<BarItemModel> converted = _converter.Convert(_models, typeof(object), null, null) as IEnumerable<BarItemModel>;
            Assert.IsNotNull(converted);
            foreach (BarItemModel model in converted)
            {
                Assert.AreEqual(BarItemModel.ItemAlignment.Right, model.Alignment);
                ++count;
            }
            Assert.AreNotEqual(0, count);
        }

        [TestMethod]
        public void NewConverter_ConvertNull_NullReturned()
        {
            Assert.IsNull(_converter.Convert(null, typeof(object), null, null));
        }

        [TestMethod]
        public void LeftAlignedConverter_ConvertNull_NullReturned()
        {
            _converter.Alignment = BarItemModel.ItemAlignment.Left;
            Assert.IsNull(_converter.Convert(null, typeof(object), null, null));
        }

        [TestMethod]
        public void RightAlignedConverter_ConvertNull_NullReturned()
        {
            _converter.Alignment = BarItemModel.ItemAlignment.Right;
            Assert.IsNull(_converter.Convert(null, typeof(object), null, null));
        }

        [TestMethod]
        public void NewConverter_BadTargetType_ThrowsArgumentException()
        {
            try
            {
                _converter.Convert(_models, typeof(string), null, null);
                Assert.Fail("Unexpected success");
            }
            catch(ArgumentException)
            {
                // Success
            }
            catch(Exception ex)
            {
                Assert.Fail(string.Format("Unexpected exception {0}", ex));
            }
        }

        [TestMethod]
        public void NewConverter_BadValueType_ThrowsArgumentException()
        {
            try
            {
                _converter.Convert("input string", typeof(object), null, null);
                Assert.Fail("Unexpected success");
            }
            catch (ArgumentException)
            {
                // Success
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Unexpected exception {0}", ex));
            }
        }

        [TestMethod]
        public void LeftAlignedConverter_ConvertOnlyRightAligned_EmptyCollectionReturned()
        {
            int count = 0;
            IEnumerable<BarItemModel> input = new BarItemModel[] { new SeparatorBarItemModel(BarItemModel.ItemAlignment.Right) };
            _converter.Alignment = BarItemModel.ItemAlignment.Left;
            IEnumerable<BarItemModel> converted = _converter.Convert(input, typeof(object), null, null) as IEnumerable<BarItemModel>;
            Assert.IsNotNull(converted);
            foreach (BarItemModel model in converted)
                ++count;
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void RightAlignedConverter_ConvertOnlyLeftAligned_EmptyCollectionReturned()
        {
            int count = 0;
            IEnumerable<BarItemModel> input = new BarItemModel[] { new SeparatorBarItemModel(BarItemModel.ItemAlignment.Left) };
            _converter.Alignment = BarItemModel.ItemAlignment.Right;
            IEnumerable<BarItemModel> converted = _converter.Convert(input, typeof(object), null, null) as IEnumerable<BarItemModel>;
            Assert.IsNotNull(converted);
            foreach (BarItemModel model in converted)
                ++count;
            Assert.AreEqual(0, count);
        }
    }
}
