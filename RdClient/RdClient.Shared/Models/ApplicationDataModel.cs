namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.CxWrappers;

    public sealed class ApplicationDataModel : MutableObject, IPersistentObject
    {
        private readonly GroupCommand _save;

        private IStorageFolder _rootFolder;
        private IModelSerializer _modelSerializer;
        private IDataScrambler _dataScrambler;
        private CertificateTrust _certificateTrust;
        private GeneralSettings _settings;
        private WorkspaceModel<LocalWorkspaceModel> _localWorkspace;
        private IModelCollection<OnPremiseWorkspaceModel> _onPremWorkspaces;
        private IModelCollection<CredentialsModel> _credentials;
        private IModelCollection<GatewayModel> _gateways;

        public ICommand Save
        {
            get { return _save.Command; }
        }

        public IStorageFolder RootFolder
        {
            get { return _rootFolder; }

            set
            {
                Contract.Assert(null == _rootFolder, "RootFolder may be set only once");
                Contract.Assert(null != value, "RootFolder cannot be removed");

                this.SetProperty(ref _rootFolder, value);
            }
        }

        public IModelSerializer ModelSerializer
        {
            get { return _modelSerializer; }

            set
            {
                Contract.Assert(null == _modelSerializer, "ModelSerializer may be set only once");
                Contract.Assert(null != value, "ModelSerializer cannot be removed");

                this.SetProperty(ref _modelSerializer, value);
            }
        }

        public IDataScrambler DataScrambler
        {
            get { return _dataScrambler; }
            set { this.SetProperty(ref _dataScrambler, value); }
        }

        public IModelCollection<OnPremiseWorkspaceModel> OnPremWorkspaces 
        { 
            get { return _onPremWorkspaces; }
            private set { this.SetProperty(ref _onPremWorkspaces, value); }
        }

        public IModelCollection<GatewayModel> Gateways
        {
            get { return _gateways; }
            private set { this.SetProperty(ref _gateways, value); }
        }

        public WorkspaceModel<LocalWorkspaceModel> LocalWorkspace
        {
            get { return _localWorkspace; }
            private set { this.SetProperty(ref _localWorkspace, value); }
        }

        public ICertificateTrust CertificateTrust
        {
            get { return _certificateTrust; }
        }

        public GeneralSettings Settings
        {
            get { return _settings; }
        }

        ICommand IPersistentObject.Save
        {
            get { return _save.Command; }
        }

        public ApplicationDataModel()
        {
            _save = new GroupCommand();            
        }

        public IModelCollection<CredentialsModel> Credentials
        {
            get { return _credentials; }
            private set { this.SetProperty(ref _credentials, value); }
        }

        public void Compose()
        {
            Contract.Assert(null != _rootFolder);
            Contract.Assert(null != _modelSerializer);
            Contract.Assert(null != _dataScrambler);
            Contract.Assert(null == _localWorkspace);
            Contract.Assert(null == _certificateTrust);
            Contract.Assert(null == _settings);
            //
            // Create all subcomponents of the data model. This is done only once, when both the root folder and model serializer
            // have been set (in XAML).
            //
            this.Credentials = PrimaryModelCollection<CredentialsModel>.Load(_rootFolder.CreateFolder("credentials"), _modelSerializer);
            foreach(IModelContainer<CredentialsModel> container in this.Credentials.Models)
            {
                container.Model.SetScrambler(_dataScrambler);
            }
            SubscribeForPersistentStateUpdates(this.Credentials);

            this.LocalWorkspace = new WorkspaceModel<LocalWorkspaceModel>(_rootFolder.CreateFolder("LocalWorkspace"), _modelSerializer);
            SubscribeForPersistentStateUpdates(this.LocalWorkspace);
            //
            // Load the certificate trust (the full name of the class is used so the compiler does not confuse the class
            // and property of the same name.
            //
            _certificateTrust = RdClient.Shared.Data.CertificateTrust.Load(_rootFolder, "CertificateTrust.model", _modelSerializer);
            SubscribeForPersistentStateUpdates(_certificateTrust);

            _settings = GeneralSettings.Load(_rootFolder, "GeneralSettings.model", _modelSerializer);
            SubscribeForPersistentStateUpdates(_settings);

            //load workspaces from disk
            this.OnPremWorkspaces = PrimaryModelCollection<OnPremiseWorkspaceModel>.Load(_rootFolder.CreateFolder("OnPremWorkspaces"), _modelSerializer);
            SubscribeForPersistentStateUpdates(this.OnPremWorkspaces);

            //load gateways from disk
            this.Gateways = PrimaryModelCollection<GatewayModel>.Load(_rootFolder.CreateFolder("Gateways"), _modelSerializer);
            SubscribeForPersistentStateUpdates(this.Gateways);

            // manage changes for Credentials collection
            INotifyCollectionChanged ncc = this.Credentials.Models;
            ncc.CollectionChanged += this.OnCredentialsCollectionChanged;

            // manage changes for Gateways collection
            ncc = this.Gateways.Models;
            ncc.CollectionChanged += this.OnGatewaysCollectionChanged;
        }

        private void SubscribeForPersistentStateUpdates(IPersistentObject persistentObject)
        {
            _save.Add(persistentObject.Save);
        }

        private void OnCredentialsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (IModelContainer<CredentialsModel> credentialsContainer in e.NewItems)
                    {
                        //
                        // Attach the scrambler to the new credentials model.
                        //
                        credentialsContainer.Model.SetScrambler(_dataScrambler);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach(IModelContainer<CredentialsModel> credentialsContainer in e.OldItems)
                    {
                        //
                        // Remove references to deleted credentials from all local desktops
                        //
                        foreach(IModelContainer<RemoteConnectionModel> connectionContainer in _localWorkspace.Connections.Models)
                        {
                            connectionContainer.Model.CastAndCall<DesktopModel>(desktop=>
                            {
                                if (credentialsContainer.Id.Equals(desktop.CredentialsId))
                                    desktop.CredentialsId = Guid.Empty;
                            });
                        }
                        //
                        // Remove references to deleted credentials from all workspaces
                        //
                        foreach (IModelContainer<OnPremiseWorkspaceModel> workspace in _onPremWorkspaces.Models)
                        {
                            if (credentialsContainer.Id.Equals(workspace.Model.CredentialsId))
                            {
                                workspace.Model.CredentialsId = Guid.Empty;
                            }
                        }
                        //
                        // Remove references to deleted credentials from all gateways
                        //
                        foreach (IModelContainer<GatewayModel> gateway in _gateways.Models)
                        {
                            if (credentialsContainer.Id.Equals(gateway.Model.CredentialsId))
                            {
                                gateway.Model.CredentialsId = Guid.Empty;
                            }
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    //
                    // For each desktop check if the credential reference is valid, and if it is not, remove it.
                    //
                    foreach(IModelContainer<RemoteConnectionModel> connectionContainer in _localWorkspace.Connections.Models)
                    {
                        connectionContainer.Model.CastAndCall<DesktopModel>(desktop =>
                        {
                            if (!Guid.Empty.Equals(desktop.CredentialsId))
                            {
                                try
                                {
                                    this.Credentials.GetModel(desktop.CredentialsId);
                                }
                                catch (KeyNotFoundException)
                                {
                                    desktop.CredentialsId = Guid.Empty;
                                }
                            }
                        });
                    }
                    //
                    // For each workspace check if the credential reference is valid, and if it is not, remove it.
                    //
                    foreach (IModelContainer<OnPremiseWorkspaceModel> workspace in _onPremWorkspaces.Models)
                    {
                        if (!Guid.Empty.Equals(workspace.Model.CredentialsId))
                        {
                            try
                            {
                                this.Credentials.GetModel(workspace.Model.CredentialsId);
                            }
                            catch (KeyNotFoundException)
                            {
                                workspace.Model.CredentialsId = Guid.Empty;
                            }
                        }
                    }
                    //
                    // For each gateway check if the credential reference is valid, and if it is not, remove it.
                    //
                    foreach (IModelContainer<GatewayModel> gateway in _gateways.Models)
                    {
                        if (!Guid.Empty.Equals(gateway.Model.CredentialsId))
                        {
                            try
                            {
                                this.Credentials.GetModel(gateway.Model.CredentialsId);
                            }
                            catch (KeyNotFoundException)
                            {
                                gateway.Model.CredentialsId = Guid.Empty;
                            }
                        }
                    }
                    break;
            }
        }

        private void OnGatewaysCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    foreach (IModelContainer<GatewayModel> gatewayContainer in e.OldItems)
                    {
                        //
                        // Remove references to deleted gateways from all local desktops
                        //
                        foreach (IModelContainer<RemoteConnectionModel> connectionContainer in _localWorkspace.Connections.Models)
                        {
                            connectionContainer.Model.CastAndCall<DesktopModel>(desktop =>
                            {
                                if (gatewayContainer.Id.Equals(desktop.GatewayId))
                                    desktop.GatewayId = Guid.Empty;
                            });
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    //
                    // For each desktop check if the gateway reference is valid, and if it is not, remove it.
                    //
                    foreach (IModelContainer<RemoteConnectionModel> connectionContainer in _localWorkspace.Connections.Models)
                    {
                        connectionContainer.Model.CastAndCall<DesktopModel>(desktop =>
                        {
                            if (!Guid.Empty.Equals(desktop.GatewayId))
                            {
                                try
                                {
                                    this.Credentials.GetModel(desktop.GatewayId);
                                }
                                catch (KeyNotFoundException)
                                {
                                    desktop.GatewayId = Guid.Empty;
                                }
                            }
                        });
                    }
                    break;
            }
        }

    }
}
