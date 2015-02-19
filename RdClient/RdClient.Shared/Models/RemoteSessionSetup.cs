namespace RdClient.Shared.Models
{
    using System.Diagnostics.Contracts;

    public sealed class RemoteSessionSetup
    {
        private readonly RemoteConnectionModel _connection;
        private readonly CredentialsModel _originalCredentials;
        private CredentialsModel _credentials;
        private bool _savedCredentials;

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

        public RemoteSessionSetup(RemoteConnectionModel connection, CredentialsModel credentials, bool savedCredentials)
        {
            Contract.Requires(null != connection);
            Contract.Requires(null != credentials);

            _connection = connection;
            _originalCredentials = credentials;

            if (null != _originalCredentials)
                _credentials = new CredentialsModel(_originalCredentials);
        }
    }
}
