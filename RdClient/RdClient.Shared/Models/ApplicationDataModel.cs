namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using RdClient.Shared.Navigation;

    public sealed class ApplicationDataModel : MutableObject, IPersistentObject
    {
        private readonly GroupCommand _save;

        private IStorageFolder _rootFolder;
        private IModelSerializer _modelSerializer;
        private CertificateTrust _certificateTrust;
        private GeneralSettings _settings;
        private WorkspaceModel<LocalWorkspaceModel> _localWorkspace;

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

                if(this.SetProperty(ref _rootFolder, value))
                {
                    ComposeDataModel();
                }
            }
        }

        public IModelSerializer ModelSerializer
        {
            get { return _modelSerializer; }

            set
            {
                Contract.Assert(null == _modelSerializer, "ModelSerializer may be set only once");
                Contract.Assert(null != value, "ModelSerializer cannot be removed");

                if (this.SetProperty(ref _modelSerializer, value))
                {
                    ComposeDataModel();
                }
            }
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

        private void ComposeDataModel()
        {
            if (null != _rootFolder && null != _modelSerializer)
            {
                Contract.Assert(null == _localWorkspace);
                Contract.Assert(null == _certificateTrust);
                Contract.Assert(null == _settings);
                //
                // Create all subcomponents of the data model. This is done only once, when both the root folder and model serializer
                // have been set (in XAML).
                //
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

                INotifyCollectionChanged ncc = this.LocalWorkspace.Credentials.Models;
                ncc.CollectionChanged += this.OnCredentialsCollectionChanged;
            }
        }

        private void SubscribeForPersistentStateUpdates(IPersistentObject persistentObject)
        {
            _save.Add(persistentObject.Save);
        }

        private void OnCredentialsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    //
                    // Remove references to deleted credentials from all local desktops
                    //
                    foreach(IModelContainer<CredentialsModel> credentialsContainer in e.OldItems)
                    {
                        foreach(IModelContainer<RemoteConnectionModel> connectionContainer in _localWorkspace.Connections.Models)
                        {
                            connectionContainer.Model.CastAndCall<DesktopModel>(desktop=>
                            {
                                if (credentialsContainer.Id.Equals(desktop.CredentialsId))
                                    desktop.CredentialsId = Guid.Empty;
                            });
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
                                    _localWorkspace.Credentials.GetModel(desktop.CredentialsId);
                                }
                                catch (KeyNotFoundException)
                                {
                                    desktop.CredentialsId = Guid.Empty;
                                }
                            }
                        });
                    }
                    break;
            }
        }
    }
}
