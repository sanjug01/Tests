namespace RdClient.Data
{
    using RdClient.Shared.Data;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Implementation of the IStorageFolder interface that uses ApplicationData
    /// </summary>
    public sealed class ApplicationDataLocalStorageFolder : IStorageFolder
    {
        private readonly Windows.Storage.StorageFolder _parentFolder;
        private string _folderName;
        private Windows.Storage.StorageFolder _folder;

        public string FolderName
        {
            get { return _folderName; }
            set
            {
                if (!string.Equals(value, _folderName, StringComparison.OrdinalIgnoreCase))
                {
                    _folderName = value;
                    _folder = null;
                }
            }
        }

        public ApplicationDataLocalStorageFolder()
        {
            _parentFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        }

        private ApplicationDataLocalStorageFolder(Windows.Storage.StorageFolder parentFolder, string folderName)
        {
            Contract.Assert(null != parentFolder);
            Contract.Assert(!string.IsNullOrEmpty(folderName));
            _parentFolder = parentFolder;
            _folderName = folderName;
        }

        IEnumerable<string> IStorageFolder.GetFolders()
        {
            IList<string> folderNames = new List<string>();

            OpenFolderAndCall(folder =>
            {
                Task<IReadOnlyList<Windows.Storage.StorageFolder>> task = folder.CreateFolderQuery()
                    .GetFoldersAsync()
                    .AsTask<IReadOnlyList<Windows.Storage.StorageFolder>>();

                foreach (Windows.Storage.StorageFolder subfolder in task.Result)
                {
                    folderNames.Add(subfolder.Name);
                }
            });

            return folderNames;
        }

        IEnumerable<string> IStorageFolder.GetFiles()
        {
            IList<string> fileNames = new List<string>();

            OpenFolderAndCall(folder =>
            {
                Task<IReadOnlyList<Windows.Storage.StorageFile>> task = folder.CreateFileQuery()
                    .GetFilesAsync()
                    .AsTask<IReadOnlyList<Windows.Storage.StorageFile>>();

                foreach (Windows.Storage.StorageFile file in task.Result)
                {
                    fileNames.Add(file.Name);
                }
            });

            return fileNames;
        }

        IStorageFolder IStorageFolder.OpenFolder(string folderName)
        {
            IStorageFolder subfolder = null;

            OpenFolderAndCall(folder =>
            {
                subfolder = new ApplicationDataLocalStorageFolder(folder, folderName);
            });

            return subfolder;
        }

        IStorageFolder IStorageFolder.CreateFolder(string folderName)
        {
            IStorageFolder subfolder = null;

            CreateFolderAndCall(folder =>
            {
                subfolder = new ApplicationDataLocalStorageFolder(folder, folderName);
            });

            return subfolder;
        }

        Stream IStorageFolder.OpenFile(string name)
        {
            Stream stream = null;

            OpenFolderAndCall(folder =>
            {
                try
                {
                    Task<Windows.Storage.StorageFile> task = folder.GetFileAsync(name).AsTask<Windows.Storage.StorageFile>();
                    Task<Stream> openTask = task.Result.OpenStreamForReadAsync();
                    stream = openTask.Result;
                }
                catch (FileNotFoundException)
                {
                    // Success
                }
                catch (AggregateException ex)
                {
                    bool rightException = ex.InnerException is FileNotFoundException;

                    if (!rightException)
                    {
                        foreach (Exception innerException in ex.InnerExceptions)
                        {
                            if (innerException is FileNotFoundException)
                            {
                                rightException = true;
                                break;
                            }
                        }

                        if (!rightException)
                            throw new AggregateException("Unexpected exception from data storage", ex);
                    }
                }
            });

            return stream;
        }

        Stream IStorageFolder.CreateFile(string name)
        {
            Stream stream = null;

            CreateFolderAndCall(folder =>
            {
                stream = folder
                    .CreateFileAsync(name, Windows.Storage.CreationCollisionOption.ReplaceExisting)
                    .AsTask<Windows.Storage.StorageFile>()
                    .Result
                        .OpenStreamForWriteAsync().Result;
            });

            return stream;
        }

        void IStorageFolder.DeleteFile(string name)
        {
            OpenFolderAndCall(folder =>
            {
                try
                {
                    Task<Windows.Storage.StorageFile> getTask = folder.GetFileAsync(name).AsTask<Windows.Storage.StorageFile>();
                    getTask.Result.DeleteAsync(Windows.Storage.StorageDeleteOption.PermanentDelete).AsTask().Wait();
                }
                catch (FileNotFoundException)
                {
                    // Success
                }
                catch (AggregateException ex)
                {
                    bool rightException = ex.InnerException is FileNotFoundException;

                    if (!rightException)
                    {
                        foreach (Exception innerException in ex.InnerExceptions)
                        {
                            if (innerException is FileNotFoundException)
                            {
                                rightException = true;
                                break;
                            }
                        }

                        if (!rightException)
                            throw new AggregateException("Unexpected exception from data storage", ex);
                    }
                }
            });
        }

        void IStorageFolder.Delete()
        {
            OpenFolderAndCall(folder =>
            {
                folder.DeleteAsync().AsTask().Wait();
            });
        }

        private void OpenFolderAndCall(Action<Windows.Storage.StorageFolder> action)
        {
            if (null == _folder)
                _folder = OpenFolder(_parentFolder, _folderName);

            if(null != _folder)
            {
                action(_folder);
            }
        }

        private void CreateFolderAndCall(Action<Windows.Storage.StorageFolder> action)
        {
            if (null == _folder)
                _folder = CreateFolder(_parentFolder, _folderName);

            if (null != _folder)
            {
                action(_folder);
            }
        }

        private static Windows.Storage.StorageFolder OpenFolder(Windows.Storage.StorageFolder parentFolder, string folderName)
        {
            Windows.Storage.StorageFolder openedFolder = null;

            try
            {
                openedFolder = parentFolder
                    .GetFolderAsync(folderName)
                    .AsTask<Windows.Storage.StorageFolder>()
                    .Result;
            }
            catch (FileNotFoundException)
            {
                // Success
            }
            catch (AggregateException ex)
            {
                bool rightException = ex.InnerException is FileNotFoundException;

                if (!rightException)
                {
                    foreach (Exception innerException in ex.InnerExceptions)
                    {
                        if (innerException is FileNotFoundException)
                        {
                            rightException = true;
                            break;
                        }
                    }

                    if (!rightException)
                        throw new AggregateException("Unexpected exception from data storage", ex);
                }
            }

            return openedFolder;
        }

        private static Windows.Storage.StorageFolder CreateFolder(Windows.Storage.StorageFolder parentFolder, string folderName)
        {
            return parentFolder
                .CreateFolderAsync(folderName, Windows.Storage.CreationCollisionOption.OpenIfExists)
                .AsTask<Windows.Storage.StorageFolder>()
                .Result;
        }
    }
}
