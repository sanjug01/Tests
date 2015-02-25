namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using System;
    using System.Diagnostics.Contracts;

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
                    foreach(IModelContainer<CredentialsModel> c in _dataModel.LocalWorkspace.Credentials.Models)
                    {
                        if (dtm.CredentialsId.Equals(c.Id))
                        {
                            _sessionCredentials = new SessionCredentials(c);
                            break;
                        }
                    }
                    Contract.Assert(null != _sessionCredentials);
                }
                else
                {
                    _sessionCredentials = new SessionCredentials();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
