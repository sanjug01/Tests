namespace RdClient.Models
{
    using RdClient.Shared.Models;

    public sealed class FileSystemDataStorage : IDataStorage
    {
        private string _rootFolder;

        public FileSystemDataStorage()
        {
        }

        public string RootFolder
        {
            get { return _rootFolder; }
            set
            {
                //
                // TODO: add validation if necessary; doesn't look necessary at the moment.
                //
                _rootFolder = value;
            }
        }

        void IDataStorage.Load(PersistentData persistentData)
        {
            throw new System.NotImplementedException();
        }

        void IDataStorage.Save(PersistentData persistentData)
        {
            throw new System.NotImplementedException();
        }
    }
}
