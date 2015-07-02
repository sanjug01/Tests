namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public sealed class RemoteSessionSetup
    {
        private ApplicationDataModel _dataModel;
        private RemoteConnectionModel _connection;
        private SessionCredentials _sessionCredentials;
        private SessionGateway _sessionGateway;

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

        public SessionGateway SessionGateway
        {
            get { return _sessionGateway; }
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

        public void SaveGatewayCredentials()
        {
            if (_connection is DesktopModel)
            {
                _sessionGateway.SaveCredentials(_dataModel);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void InitSessionCredentials(SessionCredentials credentials)
        {
            if(null != credentials)
            {
                _sessionCredentials = credentials;
            }
            else
            {
                ICredentialsIdModel cim = (ICredentialsIdModel)_connection;
                if(cim.HasCredentials)
                {
                    _sessionCredentials = new SessionCredentials(_dataModel.Credentials.Models.First(c => cim.CredentialsId == c.Id));
                }
                else
                {
                    _sessionCredentials = new SessionCredentials();
                }
            }
        }

        private void InitGateway(SessionGateway gateway)
        {
            Contract.Assert(_sessionCredentials != null);

            if(_connection is DesktopModel)
            {
                if(((DesktopModel)_connection).HasGateway)
                {
                    IModelContainer<GatewayModel> gatewayModel = null;
                    IModelContainer<CredentialsModel> gatewayCredentials = null;

                    if (gateway != null)
                    {
                        gatewayModel = ModelContainer<GatewayModel>.CreateForNewModel(gateway.Gateway);
                    }
                    else
                    {
                        DesktopModel dm = (DesktopModel)_connection;
                        gatewayModel = _dataModel.Gateways.Models.First(c => dm.GatewayId == c.Id);
                    }

                    if (gatewayModel.Model.HasCredentials)
                    {
                        gatewayCredentials = _dataModel.Credentials.Models.First(c => gatewayModel.Model.CredentialsId == c.Id);
                    }
                    else if (_sessionCredentials != null)
                    {
                        gatewayCredentials = ModelContainer<CredentialsModel>.CreateForNewModel(_sessionCredentials.Credentials);
                        if (String.IsNullOrEmpty(gatewayCredentials.Model.Password))
                        {
                            gatewayCredentials.Model.Password = String.Empty;
                        }
                    }

                    _sessionGateway = new SessionGateway(gatewayModel, gatewayCredentials);
                }
                else
                {
                    _sessionGateway = new SessionGateway();
                }
            }
        }

        public RemoteSessionSetup(ApplicationDataModel dataModel, RemoteConnectionModel connection)
        {
            _dataModel = dataModel;
            _connection = connection;
            InitSessionCredentials(null);
            InitGateway(null);
        }

        public RemoteSessionSetup(RemoteSessionSetup setup)
        {
            _dataModel = setup.DataModel;
            _connection = setup.Connection;
            InitSessionCredentials(setup.SessionCredentials);
            InitGateway(setup.SessionGateway);
        }
    }
}
