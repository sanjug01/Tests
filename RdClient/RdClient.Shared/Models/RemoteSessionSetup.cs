namespace RdClient.Shared.Models
{
    using System.Diagnostics.Contracts;

    public sealed class RemoteSessionSetup
    {
        private readonly ApplicationDataModel _dataModel;
        private readonly RemoteConnectionModel _connection;
        private CredentialsModel _credentials;
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

        public void SetCredentials(CredentialsModel credentials)
        {
            if (null == _credentials)
            {
                _credentials = credentials;
            }
            else
            {
                _credentials.Username = credentials.Username;
                _credentials.Password = credentials.Password;
            }
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
