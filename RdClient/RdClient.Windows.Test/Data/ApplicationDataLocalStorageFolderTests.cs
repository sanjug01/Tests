using Windows.Storage;

namespace RdClient.Windows.Test.Data
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Data;
    using RdClient.Shared.Data;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    [TestClass]
    public sealed class ApplicationDataLocalStorageFolderTests
    {
        static readonly string RootFolderName = "TestRootFolder";

        private bool _deleteTestFolder;

        [TestInitialize]
        public void SetUpTest()
        {
            _deleteTestFolder = false;
        }

        [TestCleanup]
        public void TearDownTest()
        {
            //
            // Delete the test root folder;
            //
            try
            {
                Task<StorageFolder> getTask = ApplicationData.Current.LocalFolder.GetFolderAsync(RootFolderName).AsTask<StorageFolder>();
                getTask.Wait();
                getTask.Result.DeleteAsync(StorageDeleteOption.PermanentDelete).AsTask().Wait();
            }
            catch (Exception ex)
            {
                //
                // The folder is deleted after each test (just in case) but a failure is reported only
                // if the folder was expected to be there.
                //
                if (_deleteTestFolder)
                    Debug.WriteLine("TearDownTest|Failed to delete the test root folder|{0}", ex.Message);
            }
        }

        [TestMethod]
        public void NewLocalStorageFolder_ListFiles_EmptyList()
        {
            IStorageFolder root = new ApplicationDataLocalStorageFolder() { FolderName = RootFolderName };
            IEnumerable<string> files = root.GetFiles();

            Assert.IsNotNull(files);

            foreach(string s in files)
            {
                Assert.Fail("Unexpected file returned");
            }
        }

        [TestMethod]
        public void NewLocalStorageFolder_ListFolders_EmptyList()
        {
            IStorageFolder root = new ApplicationDataLocalStorageFolder() { FolderName = RootFolderName };
            IEnumerable<string> folders = root.GetFolders();

            Assert.IsNotNull(folders);

            foreach (string s in folders)
            {
                Assert.Fail("Unexpected folder returned");
            }
        }

        [TestMethod]
        public void NewLocalStorageFolder_GetSubfolder_NullReturned()
        {
            IStorageFolder root = new ApplicationDataLocalStorageFolder() { FolderName = RootFolderName };

            Assert.IsNull(root.OpenFolder("subfolder"));
        }

        [TestMethod]
        public void NewLocalStorageFolder_CreateSubfolder_Created()
        {
            _deleteTestFolder = true;
            IStorageFolder root = new ApplicationDataLocalStorageFolder() { FolderName = RootFolderName };

            Assert.IsNotNull(root.CreateFolder("subfolder"));
        }

        [TestMethod]
        public void LocalStorageFolder_CreateSubfolderGetSubolders_CreatedSubfolderReturned()
        {
            _deleteTestFolder = true;
            IStorageFolder root = new ApplicationDataLocalStorageFolder() { FolderName = RootFolderName };
            int count = 0;

            IStorageFolder subfolder = root.CreateFolder("subfolder");
            Assert.IsNotNull(subfolder);
            //
            // Create another subfolder to force the storage to create the first one
            //
            Assert.IsNotNull(subfolder.CreateFolder("another_subfolder"));

            foreach(string name in root.GetFolders())
            {
                Assert.AreEqual("subfolder", name);
                ++count;
            }
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void LocalStorageFolder_CreateFileGetFiles_CreatedFileReturned()
        {
            _deleteTestFolder = true;
            IStorageFolder root = new ApplicationDataLocalStorageFolder() { FolderName = RootFolderName };
            int count = 0;

            using(Stream s = root.CreateFile("file.txt"))
            {
                Assert.IsNotNull(s);
            }

            foreach (string name in root.GetFiles())
            {
                Assert.AreEqual("file.txt", name);
                ++count;
            }
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void LocalStorageFolder_CreateFileDeleteFile_FileDeleted()
        {
            _deleteTestFolder = true;
            IStorageFolder root = new ApplicationDataLocalStorageFolder() { FolderName = RootFolderName };

            using (Stream s = root.CreateFile("file.txt"))
            {
                Assert.IsNotNull(s);
            }

            root.DeleteFile("file.txt");

            foreach (string name in root.GetFiles())
            {
                Assert.Fail(string.Format("Unexpected file {0}", name));
            }
        }

        [TestMethod]
        public void LocalStorageFolder_CreateSubfolderDeleteSubfolder_SubfolderDeleted()
        {
            _deleteTestFolder = true;
            IStorageFolder root = new ApplicationDataLocalStorageFolder() { FolderName = RootFolderName };
            IStorageFolder subfolder = root.CreateFolder("subfolder");

            Assert.IsNotNull(subfolder);
            subfolder.Delete();

            foreach (string name in root.GetFolders())
            {
                Assert.Fail(string.Format("Unexpected subfolder {0}", name));
            }
        }

        [TestMethod]
        public void LocalStorageFolder_CreateSubfolderWithFileDeleteSubfolder_SubfolderDeleted()
        {
            _deleteTestFolder = true;
            IStorageFolder root = new ApplicationDataLocalStorageFolder() { FolderName = RootFolderName };
            IStorageFolder subfolder = root.CreateFolder("subfolder");

            Assert.IsNotNull(subfolder);
            using(Stream s = subfolder.CreateFile("file.txt"))
            {
                // Do nothing
            }
            subfolder.Delete();

            foreach (string name in root.GetFolders())
            {
                Assert.Fail(string.Format("Unexpected subfolder {0}", name));
            }
        }
    }
}
