namespace RdClient.Shared.Helpers
{
    using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

    /// <summary>
    /// Collection of extension methods that remove asynchronous access to the data storage objects.
    /// </summary>
    public static class DataStorageExtensions
    {
        /// <summary>
        /// Get a subfolder from a folder and if it exists call an action delegate with the subfolder.
        /// </summary>
        /// <param name="folder">Parent folder in that the extension looks for the subfolder.</param>
        /// <param name="folderName">Name of the subfolder.</param>
        /// <param name="action">Action delegate that is called if the subfolder exists in the parent folder.</param>
        public static void GetFolderAndCall(this StorageFolder folder, string folderName, Action<StorageFolder> action)
        {
            StorageFolder subfolder = GetFolder(folderName, folder);

            if(null != subfolder)
            {
                action(subfolder);
            }
        }

        /// <summary>
        /// Create a subfolder in a folder and call an action delegate with the subfolder.
        /// </summary>
        /// <param name="folder">Parent folder in that the extension creates the subfolder.</param>
        /// <param name="folderName">Name of the subfolder.</param>
        /// <param name="action">Action delegate that is called.</param>
        public static void CreateFolderAndCall(this StorageFolder folder, string folderName, Action<StorageFolder> action)
        {
            Contract.Requires(null != folder);
            Contract.Requires(null != folderName);
            Contract.Requires(null != action);

            action(CreateFolder(folderName, folder));
        }

        /// <summary>
        /// Create a subfolder if it doesn't exist in a folder and call an action delegate with the subfolder.
        /// </summary>
        /// <param name="folder">Parent folder in that the extension creates the subfolder.</param>
        /// <param name="folderName">Name of the subfolder.</param>
        /// <param name="action">Action delegate that is called.</param>
        public static void GetOrCreateFolderAndCall(this StorageFolder folder, string folderName, Action<StorageFolder> action)
        {
            Contract.Requires(null != folder);
            Contract.Requires(null != folderName);
            Contract.Requires(null != action);

            action(GetOrCreateFolder(folderName, folder));
        }

        /// <summary>
        /// Permanently delete a subfolder with the specified name if a folder.
        /// </summary>
        /// <param name="folder">Parent folder in that a subfolder is deleted.</param>
        /// <param name="folderName">NAme of the subfolder to delete.</param>
        public static void DeleteFolder(this StorageFolder folder, string folderName)
        {
            Contract.Requires(null != folder);
            Contract.Requires(null != folderName);

            folder.GetFolderAndCall(folderName, subfolder =>
            {
                Task task = subfolder.DeleteAsync(StorageDeleteOption.PermanentDelete).AsTask();
                task.Wait();
            });
        }

        public static void OpenReadStreamAndCall(this StorageFolder folder, string fileName, Action<Stream> action)
        {
            Contract.Requires(null != folder);
            Contract.Requires(null != fileName);
            Contract.Requires(null != action);

            using (Stream stream = OpenReadStream(fileName, folder))
            {
                if(null != stream)
                {
                    action(stream);
                }
            }
        }

        public static void CreateWriteStreamAndCall(this StorageFolder folder, string fileName, Action<Stream> action)
        {
            Contract.Requires(null != folder);
            Contract.Requires(null != fileName);
            Contract.Requires(null != action);

            using(Stream stream = CreateWriteStream(fileName, folder))
            {
                action(stream);
            }
        }

        public static void DeleteFile(this StorageFolder folder, string fileName)
        {
            Contract.Requires(null != folder);
            Contract.Requires(null != fileName);

            StorageFile file = GetFile(fileName, folder);

            if(null != file)
            {
                Task task = file.DeleteAsync(StorageDeleteOption.PermanentDelete).AsTask();
                task.Wait();
            }
        }

        public static void ForEachFileCall(this StorageFolder folder, QueryOptions queryOptions, Action<StorageFile> action)
        {
            Contract.Requires(null != folder);
            Contract.Requires(null != queryOptions);
            Contract.Requires(null != action);

            StorageFileQueryResult query = folder.CreateFileQueryWithOptions(queryOptions);
            Task<IReadOnlyList<StorageFile>> task = query.GetFilesAsync().AsTask<IReadOnlyList<StorageFile>>();
            task.Wait();

            foreach(StorageFile file in task.Result)
            {
                action(file);
            }
        }

        public static void OpenReadStreamAndCall(this StorageFile file, Action<Stream> action)
        {
            Task<Stream> task = file.OpenStreamForReadAsync();
            task.Wait();

            using(Stream stream = task.Result)
            {
                action(task.Result);
            }
        }

        private static StorageFolder GetFolder(string folderName, StorageFolder parentFolder)
        {
            Contract.Requires(null != folderName);
            Contract.Requires(null != parentFolder);

            StorageFolder folder = null;

            try
            {
                Task<StorageFolder> task = parentFolder.GetFolderAsync(folderName).AsTask<StorageFolder>();
                task.Wait();
                folder = task.Result;
            }
            catch(FileNotFoundException)
            {
                // Success
            }
            catch(AggregateException ex)
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

            return folder;
        }

        private static StorageFolder CreateFolder(string folderName, StorageFolder parentFolder)
        {
            Contract.Requires(null != folderName);
            Contract.Requires(null != parentFolder);
            Contract.Ensures(null != Contract.Result<StorageFolder>());

            Task<StorageFolder> task = parentFolder.CreateFolderAsync(folderName, CreationCollisionOption.ReplaceExisting).AsTask<StorageFolder>();
            task.Wait();
            Contract.Assert(null != task.Result);
            return task.Result;
        }

        private static StorageFolder GetOrCreateFolder(string folderName, StorageFolder parentFolder)
        {
            Contract.Requires(null != folderName);
            Contract.Requires(null != parentFolder);
            Contract.Ensures(null != Contract.Result<StorageFolder>());

            Task<StorageFolder> task = parentFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists).AsTask<StorageFolder>();
            task.Wait();
            Contract.Assert(null != task.Result);
            return task.Result;
        }

        private static StorageFile GetFile(string fileName, StorageFolder folder)
        {
            Contract.Requires(null != folder);
            Contract.Requires(null != fileName);

            StorageFile file = null;

            try
            {
                Task<StorageFile> task = folder.GetFileAsync(fileName).AsTask<StorageFile>();
                task.Wait();
                file = task.Result;
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

            return file;
        }

        private static Stream OpenReadStream(string fileName, StorageFolder folder)
        {
            Contract.Requires(null != folder);
            Contract.Requires(null != fileName);

            Stream stream = null;

            try
            {
                Task<Stream> task = folder.OpenStreamForReadAsync(fileName);
                task.Wait();
                stream = task.Result;
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

            return stream;
        }

        private static Stream CreateWriteStream(string fileName, StorageFolder folder)
        {
            Contract.Requires(null != folder);
            Contract.Requires(null != fileName);

            Task<Stream> task = folder.OpenStreamForWriteAsync(fileName, CreationCollisionOption.ReplaceExisting);
            task.Wait();
            return task.Result;
        }
    }
}
