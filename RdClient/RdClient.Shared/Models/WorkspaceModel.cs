namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using RdClient.Shared.ViewModels;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Windows.Input;

    public sealed class WorkspaceModel<TWorkspaceData> : SerializableCompositionModel where TWorkspaceData : INotifyPropertyChanged, new()
    {
        private readonly IStorageFolder _folder;
        private readonly IModelSerializer _modelSerializer;
        private readonly RelayCommand _save;
        private readonly TWorkspaceData _workspaceData;
        private readonly IModelCollection<RemoteConnectionModel> _connections;
        private readonly IModelCollection<CredentialsModel> _credentials;
        private bool _modelDataModified;

        public WorkspaceModel(IStorageFolder folder, IModelSerializer modelSerializer)
        {
            _folder = folder;
            _modelSerializer = modelSerializer;
            _save = new RelayCommand(this.ExecuteSave, this.CanSave);

            using (Stream stream = _folder.OpenFile(".workspace"))
            {
                if (null != stream)
                {
                    _workspaceData = _modelSerializer.ReadModel<TWorkspaceData>(stream);
                }
                else
                {
                    _workspaceData = new TWorkspaceData();
                }
            }

            _workspaceData.PropertyChanged += this.OnWorkspaceDataChanged;

            _credentials = PrimaryModelCollection<CredentialsModel>.Load(folder.CreateFolder("credentials"), modelSerializer);
            _credentials.Save.CanExecuteChanged += this.OnCanSaveChanged;
            _connections = PrimaryModelCollection<RemoteConnectionModel>.Load(folder.CreateFolder("connections"), modelSerializer);
            _connections.Save.CanExecuteChanged += this.OnCanSaveChanged;

            _modelDataModified = false;
        }

        public TWorkspaceData WorkspaceData
        {
            get
            {
                Contract.Ensures(null != Contract.Result<TWorkspaceData>());
                return _workspaceData;
            }
        }

        public IModelCollection<CredentialsModel> Credentials { get { return _credentials; } }

        public IModelCollection<RemoteConnectionModel> Connections { get { return _connections; } }

        protected override ICommand CreateSaveCommand()
        {
            return _save;
        }

        private void ExecuteSave(object parameter)
        {
            bool couldSave = this.CanSave(parameter);

            if (couldSave)
            {
                _credentials.Save.Execute(parameter);
                _connections.Save.Execute(parameter);

                using (Stream stream = _folder.CreateFile(".workspace"))
                {
                    if (null != stream)
                    {
                        _modelSerializer.WriteModel<TWorkspaceData>(_workspaceData, stream);
                        _modelDataModified = false;
                    }
                }

                if (!CanSave(parameter))
                    _save.EmitCanExecuteChanged();
            }
        }

        private bool CanSave(object parameter)
        {
            return _modelDataModified || _credentials.Save.CanExecute(parameter) || _connections.Save.CanExecute(parameter);
        }

        private void OnCanSaveChanged(object sender, EventArgs e)
        {
            if (!_modelDataModified)
                _save.EmitCanExecuteChanged();
        }

        private void OnWorkspaceDataChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_modelDataModified)
            {
                _modelDataModified = true;
                _save.EmitCanExecuteChanged();
            }
        }
    }
}
