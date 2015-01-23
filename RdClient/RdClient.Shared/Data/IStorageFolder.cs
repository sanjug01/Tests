namespace RdClient.Shared.Data
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Simple representation of a persistent storage folder.
    /// </summary>
    public interface IStorageFolder
    {
        IEnumerable<string> GetFolders();

        IEnumerable<string> GetFiles();

        IStorageFolder OpenFolder(string folderName);

        IStorageFolder CreateFolder(string folderName);

        /// <summary>
        /// Open an existing file for reading and return a readable stream.
        /// </summary>
        /// <param name="name">Name of the file.</param>
        /// <returns>Stream object from that contents of the file can be read.</returns>
        Stream OpenFile(string name);

        /// <summary>
        /// Create a new file or overwrite an existing one and return a writable stream.
        /// </summary>
        /// <param name="name">Name of the file.</param>
        /// <returns>Writable stream object.</returns>
        Stream CreateFile(string name);

        /// <summary>
        /// Delete an existing file in the folder.
        /// </summary>
        /// <param name="name">Name of the file to delete.</param>
        /// <remarks>If the file does not exist the method does not fail.</remarks>
        void DeleteFile(string name);

        /// <summary>
        /// Delete the folder represented by the object.
        /// </summary>
        void Delete();
    }
}
