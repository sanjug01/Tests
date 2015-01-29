namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public sealed class ApplicationDataModel : MutableObject, IPersistentObject
    {
        private readonly ISet<ICommand> _saveCommands;
        private readonly RelayCommand _save;

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
            get { return _save; }
        }

        public ApplicationDataModel()
        {
            _saveCommands = new HashSet<ICommand>();
            _save = new RelayCommand(this.Save, this.CanSave);
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
            persistentObject.Save.CanExecuteChanged += this.OnPersistentObjectCanSaveChanged;
        }

        private void OnPersistentObjectCanSaveChanged(object sender, EventArgs e)
        {
            ICommand saveCommand = sender as ICommand;
            Contract.Assert(null != saveCommand, string.Format("Unexpected object type {0} for Save command", sender.GetType()));

            bool setChanged = false;

            if(saveCommand.CanExecute(true))
            {
                //
                // Add the command to the set of saveable objects
                //
                setChanged = _saveCommands.Add(saveCommand);
            }
            else
            {
                //
                // Remove the command from the set of saveable objects
                //
                setChanged = _saveCommands.Remove(saveCommand);
            }

            if (setChanged)
                _save.EmitCanExecuteChanged();
        }

        private void Save(object parameter)
        {
            foreach (ICommand saveCommand in _saveCommands)
                saveCommand.Execute(parameter);
        }

        private bool CanSave(object parameter)
        {
            return 0 != _saveCommands.Count;
        }
    }
}
