namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.ViewModels;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Windows.Input;

    public sealed class WorkspaceModel<TWorkspaceData> : SerializableCompositionModel where TWorkspaceData : class, INotifyPropertyChanged, new()
    {
        private readonly IStorageFolder _folder;
        private readonly IModelSerializer _modelSerializer;
        private readonly RelayCommand _saveWorkspaceData;
        private readonly TWorkspaceData _workspaceData;
        private readonly IModelCollection<RemoteConnectionModel> _connections;
        private readonly GroupCommand _save;
        private PersistentStatus _modelDataStatus;

        public WorkspaceModel(IStorageFolder folder, IModelSerializer modelSerializer)
        {
            _folder = folder;
            _modelSerializer = modelSerializer;

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

            _connections = PrimaryModelCollection<RemoteConnectionModel>.Load(folder.CreateFolder("connections"), modelSerializer);
            _saveWorkspaceData = new RelayCommand(this.SaveWorkspaceData, this.CanSaveWorkspaceData);
            _modelDataStatus = PersistentStatus.Clean;

            _save = new GroupCommand();
            _save.Add(_connections.Save);
            _save.Add(_saveWorkspaceData);
        }

        public TWorkspaceData WorkspaceData
        {
            get
            {
                Contract.Ensures(null != Contract.Result<TWorkspaceData>());
                return _workspaceData;
            }
        }

        public IModelCollection<RemoteConnectionModel> Connections { get { return _connections; } }

        protected override ICommand CreateSaveCommand()
        {
            return _save.Command;
        }

        private void SaveWorkspaceData(object parameter)
        {
            if (PersistentStatus.Clean != _modelDataStatus)
            {
                using (Stream stream = _folder.CreateFile(".workspace"))
                {
                    if (null != stream)
                    {
                        _modelSerializer.WriteModel<TWorkspaceData>(_workspaceData, stream);
                        _modelDataStatus = PersistentStatus.Clean;
                        _saveWorkspaceData.EmitCanExecuteChanged();
                    }
                }
            }
        }

        private bool CanSaveWorkspaceData(object parameter)
        {
            return PersistentStatus.Clean != _modelDataStatus;
        }

        private void OnWorkspaceDataChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PersistentStatus.Clean == _modelDataStatus)
            {
                _modelDataStatus = PersistentStatus.Modified;
                _saveWorkspaceData.EmitCanExecuteChanged();
            }
        }
    }
}
