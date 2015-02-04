using Windows.Storage;
using Windows.Storage.Search;

namespace RdClient.Shared.Test.Helpers
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Helpers;
    using System;
    using System.Threading.Tasks;

    [TestClass]
    public sealed class DataStorageExtensionsTests
    {
        private static StorageFolder _testRoot;

        [ClassInitialize]
        public static void SetUpClass(TestContext context)
        {
            _testRoot = ApplicationData.Current.LocalFolder.CreateFolderAsync("TestRootFolder").AsTask<StorageFolder>().Result;
            Assert.IsNotNull(_testRoot);
        }

        [ClassCleanup]
        public static void TearDownClass()
        {
            _testRoot.DeleteAsync(StorageDeleteOption.PermanentDelete).AsTask().Wait();
            _testRoot = null;
        }

        [TestMethod]
        public void CreateSubfolderAndCall_CalledWithValidSubfolder()
        {
            bool subfolderCreated = false;

            _testRoot.CreateFolderAndCall("subfolder", subfolder =>
            {
                if(null != subfolder)
                    subfolderCreated = true;
            });

            Assert.IsTrue(subfolderCreated);
            _testRoot.DeleteFolder("subfolder");
        }

        [TestMethod]
        public void NoSubfolders_GetSubfolderAndCall_NotCalled()
        {
            _testRoot.GetFolderAndCall("nonexistentSubfolder", subfolder =>
            {
                Assert.Fail("Unexpected subfolder found");
            });
        }

        [TestMethod]
        public void CreateSubfolder_GetSubfolderAndCall_CalledWithValidSubfolder()
        {
            StorageFolder f = null;

            _testRoot.CreateFolderAndCall("newSubfolder", subfolder =>
            {
                _testRoot.GetFolderAndCall("newSubfolder", gotSubfolder =>
                {
                    f = gotSubfolder;
                });
            });

            Assert.IsNotNull(f);
            _testRoot.DeleteFolder("newSubfolder");
        }

        [TestMethod]
        public void CreateWriteStream_CalledWithValidStream()
        {
            bool streamCreated = false;

            _testRoot.CreateWriteStreamAndCall("file.test", stream =>
            {
                Assert.IsNotNull(stream);
                streamCreated = true;
            });

            Assert.IsTrue(streamCreated);
            _testRoot.DeleteFile("file.test");
        }

        [TestMethod]
        public void WriteToStreamReadBack_ReadWrittenData()
        {
            byte[] written = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
            byte[] read = new byte[written.Length + 1];
            int readBytes = 0;

            _testRoot.CreateWriteStreamAndCall("write.test", stream =>
            {
                stream.Write(written, 0, written.Length);
            });

            _testRoot.OpenReadStreamAndCall("write.test", stream =>
            {
                readBytes = stream.Read(read, 0, read.Length);
            });

            Assert.AreEqual(written.Length, readBytes);
            _testRoot.DeleteFile("write.test");
        }

        [TestMethod]
        public void CreateStream_DeleteFile_FileDeleted()
        {
            bool created = false;

            _testRoot.CreateWriteStreamAndCall("writedelete.test", stream => { });
            _testRoot.OpenReadStreamAndCall("writedelete.test", stream => { created = true; });
            Assert.IsTrue(created);
            _testRoot.DeleteFile("writedelete.test");
            created = false;
            _testRoot.OpenReadStreamAndCall("writedelete.test", stream => { created = true; });
            Assert.IsFalse(created);
        }

        [TestMethod]
        public void CreateManyFiles_CallForAllFiles_Called()
        {
            const int FileCount = 25;
            int lastNumber = FileCount + 1;

            _testRoot.CreateFolderAndCall("files.folder", folder =>
            {
                //
                // Create files in the new folder
                //
                for (int i = 0; i < FileCount; ++i)
                {
                    string fileName = string.Format("{0}.file", (FileCount - i).ToString("D5"));
                    folder.CreateWriteStreamAndCall(fileName, stream => stream.WriteByte((byte)i));
                    folder.CreateWriteStreamAndCall(fileName + ".extra", stream => stream.WriteByte((byte)i));
                }
                //
                // Verify that all the files are retrieved in the correct order.
                //
                QueryOptions options = new QueryOptions(CommonFileQuery.OrderByName, new string[] { ".file" });
                bool first = true;

                folder.ForEachFileCall(options, file =>
                {
                    string[] parts = file.Name.Split('.');
                    Assert.AreEqual(2, parts.Length);
                    Assert.AreEqual("file", parts[1]);
                    int fileNumber = int.Parse(parts[0]);

                    if (first)
                        first = false;
                    else
                        Assert.IsTrue(lastNumber < fileNumber);
                    lastNumber = fileNumber;
                });
            });
            Assert.AreEqual(FileCount, lastNumber);

            _testRoot.DeleteFolder("files.folder");
        }
    }
}
