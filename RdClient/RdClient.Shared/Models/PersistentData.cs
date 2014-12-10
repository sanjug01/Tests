namespace RdClient.Shared.Models
{
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Core data model of the application - collection of all persistent objects.
    /// </summary>
    public sealed class PersistentData
    {
        private readonly LocalWorkspace _localWorkspace;
        private readonly ObservableCollection<OnPremiseWorkspace> _onPremiseWorkspaces;
        private readonly ObservableCollection<CloudWorkspace> _cloudWorkspaces;
        private readonly ModelCollection<Credentials> _credentials;
        IDataStorage _storage;

        public LocalWorkspace LocalWorkspace
        {
            get { return _localWorkspace; }
        }

        public ObservableCollection<OnPremiseWorkspace> OnPremiseWorkspaces
        {
            get { return _onPremiseWorkspaces; }
        }

        public ModelCollection<Credentials> Credentials
        {
            get { return _credentials; }
        }

        public IDataStorage Storage
        {
            get { return _storage; }
            set { _storage = value; }
        }

        public PersistentData()
        {
            _localWorkspace = new LocalWorkspace(this);
            _onPremiseWorkspaces = new ObservableCollection<OnPremiseWorkspace>();
            _cloudWorkspaces = new ObservableCollection<CloudWorkspace>();
            _credentials = new ModelCollection<Credentials>();
        }

        public void Load()
        {
            Contract.Assert(null != _storage);
            _storage.Load(this);
        }

        public void Save()
        {
            Contract.Assert(null != _storage);
            _storage.Save(this);
        }
    }
}
