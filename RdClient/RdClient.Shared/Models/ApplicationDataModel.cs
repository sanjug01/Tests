namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public sealed class ApplicationDataModel : MutableObject, IPersistentObject
    {
        private readonly ISet<ICommand> _saveCommands;
        private readonly GroupCommand _save;

        private IStorageFolder _rootFolder;
        private IModelSerializer _modelSerializer;
        private CertificateTrust _certificateTrust;
        private WorkspaceModel<LocalWorkspaceModel> _localWorkspace;

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

        ICommand IPersistentObject.Save
        {
            get { return _save.Command; }
        }

        public ApplicationDataModel()
        {
            _saveCommands = new HashSet<ICommand>();
            _save = new GroupCommand();
        }

        private void ComposeDataModel()
        {
            if (null != _rootFolder && null != _modelSerializer)
            {
                Contract.Assert(null == _localWorkspace);
                Contract.Assert(null == _certificateTrust);
                //
                // Create all subcomponents of the data model. This is done only once, when both the root folder and model serializer
                // have been set (in XAML).
                //
                this.LocalWorkspace = new WorkspaceModel<LocalWorkspaceModel>(_rootFolder.CreateFolder("LocalWorkspace"), _modelSerializer);
                SubscribeForPersistentStateUpdates(this.LocalWorkspace);
                //
                // Load the certificate trust.
                //
                _certificateTrust = CertificateTrust.Load(_rootFolder, "CertificateTrust.model", _modelSerializer);
                SubscribeForPersistentStateUpdates(_certificateTrust);
            }
        }

        private void SubscribeForPersistentStateUpdates(IPersistentObject persistentObject)
        {
            _save.Add(persistentObject.Save);
        }
    }
}
