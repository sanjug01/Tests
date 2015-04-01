namespace RdClient.Shared.Test.Data
{
    using RdClient.Shared.Data;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;

    sealed class MemoryStorageFolder : IStorageFolder
    {
        private readonly MemoryStorageFolder _parentFolder;
        private readonly string _name;
        private readonly IDictionary<string, IStorageFolder> _subfolders;
        private readonly IDictionary<string, Stream> _files;

        public MemoryStorageFolder()
        {
            _parentFolder = null;
            _name = null;
            _subfolders = new Dictionary<string, IStorageFolder>();
            _files = new Dictionary<string, Stream>();
        }

        private MemoryStorageFolder(MemoryStorageFolder parentFolder, string name)
        {
            Contract.Assert(null != parentFolder);
            Contract.Assert(!string.IsNullOrEmpty(name));

            _parentFolder = parentFolder;
            _name = name;
            _subfolders = new Dictionary<string, IStorageFolder>();
            _files = new Dictionary<string, Stream>();
        }

        IEnumerable<string> IStorageFolder.GetFolders()
        {
            return _subfolders.Keys;
        }

        IEnumerable<string> IStorageFolder.GetFiles()
        {
            return _files.Keys;
        }

        IStorageFolder IStorageFolder.OpenFolder(string folderName)
        {
            IStorageFolder folder;

            if (!_subfolders.TryGetValue(folderName, out folder))
                folder = null;

            return folder;
        }

        IStorageFolder IStorageFolder.CreateFolder(string folderName)
        {
            IStorageFolder folder = null;

            if(!_subfolders.TryGetValue(folderName, out folder))
            {
                folder = new MemoryStorageFolder(this, folderName);
                _subfolders.Add(folderName, folder);
            }

            return folder;
        }

        Stream IStorageFolder.OpenFile(string name)
        {
            Stream stream = null;

            if (_files.TryGetValue(name, out stream))
                stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        Stream IStorageFolder.CreateFile(string name)
        {
            Stream newStream = new MemoryStream();

            _files[name] = newStream;

            return newStream;
        }

        void IStorageFolder.DeleteFile(string name)
        {
            Stream stream;

            if (_files.TryGetValue(name, out stream))
            {
                _files.Remove(name);
                stream.Dispose();
            }
        }

        void IStorageFolder.Delete()
        {
            if (null == _parentFolder)
                throw new InvalidOperationException("Cannot delete the root folder");

            _files.Clear();
            _subfolders.Clear();
            _parentFolder._subfolders.Remove(_name);
        }
    }
}
