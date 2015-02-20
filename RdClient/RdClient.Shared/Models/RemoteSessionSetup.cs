namespace RdClient.Shared.Models
{
    using System.Diagnostics.Contracts;

    public sealed class RemoteSessionSetup
    {
        private readonly ApplicationDataModel _dataModel;
        private readonly RemoteConnectionModel _connection;
        private readonly CredentialsModel _credentials;
        private bool _savedCredentials;

        public ApplicationDataModel DataModel
        {
            get { return _dataModel; }
        }

        public RemoteConnectionModel Connection
        {
            get { return _connection; }
        }

        public CredentialsModel Credentials
        {
            get { return _credentials; }
        }

        public bool SavedCredentials
        {
            get { return _savedCredentials; }
            set { _savedCredentials = value; }
        }

        public RemoteSessionSetup(ApplicationDataModel dataModel, RemoteConnectionModel connection, CredentialsModel credentials, bool savedCredentials)
        {
            Contract.Requires(null != dataModel);
            Contract.Requires(null != connection);

            _dataModel = dataModel;
            _connection = connection;
            _credentials = credentials;
        }
    }
}
