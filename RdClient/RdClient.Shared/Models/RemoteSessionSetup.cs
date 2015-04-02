namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public sealed class RemoteSessionSetup
    {
        private readonly ApplicationDataModel _dataModel;
        private readonly RemoteConnectionModel _connection;
        private readonly SessionCredentials _sessionCredentials;

        public ApplicationDataModel DataModel
        {
            get { return _dataModel; }
        }

        public RemoteConnectionModel Connection
        {
            get { return _connection; }
        }

        public SessionCredentials SessionCredentials
        {
            get { return _sessionCredentials; }
        }

        public string HostName
        {
            get
            {
                string hostName;

                if (_connection is DesktopModel)
                    hostName = ((DesktopModel)_connection).HostName;
                else
                    hostName = ((RemoteResourceModel)_connection).FriendlyName;

                return hostName;
            }
        }

        public void SaveCredentials()
        {
            if(_connection is DesktopModel)
            {
                DesktopModel dtm = (DesktopModel)_connection;
                dtm.CredentialsId = _sessionCredentials.Save(_dataModel);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public RemoteSessionSetup(ApplicationDataModel dataModel, RemoteConnectionModel connection)
        {
            Contract.Requires(null != dataModel);
            Contract.Requires(null != connection);
            Contract.Ensures(null != _sessionCredentials);

            _dataModel = dataModel;
            _connection = connection;

            if(_connection is DesktopModel)
            {
                DesktopModel dtm = (DesktopModel)_connection;
                if (dtm.HasCredentials)
                {
                    _sessionCredentials = new SessionCredentials(_dataModel.Credentials.Models.First(c => dtm.CredentialsId == c.Id));
                }
                else
                {
                    _sessionCredentials = new SessionCredentials();
                }
            }
            else if(_connection is RemoteResourceModel)
            {
                RemoteResourceModel remoteResource = _connection as RemoteResourceModel;
                _sessionCredentials = new SessionCredentials(_dataModel.Credentials.Models.First(c => remoteResource.CredentialId == c.Id));
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
