using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdClient.Shared.Test.Helpers;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace RdClient.Shared.Test.Model
{
    public class ThumbnailTests
    {
        Thumbnail _thumb;
        TestData _testData;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _thumb = new Thumbnail();            
        }

        [TestMethod]
        public void TestHasImageInitiallyFalse()
        {
            Assert.IsFalse(_thumb.HasImage);
        }

        [TestMethod]
        public async Task TestHasImageTrueAfterUpdate()
        {
            await _thumb.Update(_testData.NewValidScreenSnapshot(1, 1));
            Assert.IsTrue(_thumb.HasImage);
        }

        [TestMethod]
        public async Task TestImageScaledCorrectly()
        {
            RdpScreenSnapshot snapshot = _testData.NewValidScreenSnapshot(16, 9);
            await _thumb.Update(snapshot);
            Assert.IsTrue(_thumb.Image.PixelHeight == Thumbnail.THUMBNAIL_HEIGHT);
            Assert.AreEqual(snapshot.Width / (double)snapshot.Height, _thumb.Image.PixelWidth / (double)_thumb.Image.PixelHeight, 0.05d);     
        }

        [TestMethod]
        public async Task TestSerializesCorrectly()
        {
            await _thumb.Update(_testData.NewValidScreenSnapshot(4,3));
            Thumbnail deserializedThumbnail;            
            using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(ModelBase));
                serializer.WriteObject(stream.AsStream(), _thumb);                
                stream.Seek(0);
                deserializedThumbnail = serializer.ReadObject(stream.AsStream()) as Thumbnail;
            }
            Assert.IsNotNull(deserializedThumbnail);
            Assert.AreEqual(_thumb.Id, deserializedThumbnail.Id);
            Assert.AreEqual(_thumb.HasImage, deserializedThumbnail.HasImage);
            Assert.AreEqual(_thumb.Image.PixelHeight, deserializedThumbnail.Image.PixelHeight);
            Assert.AreEqual(_thumb.Image.PixelWidth, deserializedThumbnail.Image.PixelWidth);
        }

    }
}
