using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdClient.Shared.Test.Helpers;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System;

namespace RdClient.Shared.Test.Model
{
    [TestClass]
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
        public async Task TestImageScaledCorrectly()
        {
            int inputWidth = 16;
            int inputHeight = 9;
            IRdpScreenSnapshot snapshot = _testData.NewValidScreenSnapshot(inputWidth, inputHeight);
            await _thumb.Update(snapshot);
            BitmapDecoder decoder;
            using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(_thumb.ImageBytes.AsBuffer());
                stream.Seek(0);
                decoder = await BitmapDecoder.CreateAsync(stream);
                PixelDataProvider pixelData = await decoder.GetPixelDataAsync();
            }            
            Assert.IsTrue(decoder.PixelHeight == Thumbnail.THUMBNAIL_HEIGHT);
            Assert.AreEqual(decoder.PixelWidth / (double)decoder.PixelHeight, inputWidth / (double)inputHeight, 0.05d);     
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
            Assert.AreEqual(_thumb, deserializedThumbnail);
            CollectionAssert.AreEqual(_thumb.ImageBytes, deserializedThumbnail.ImageBytes);
        }

    }
}
