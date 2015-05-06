namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using RdClient.Shared.ViewModels;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Windows.Input;

    [DataContract(IsReference = true)]
    public sealed class GeneralSettings : SerializableModel, IPersistentObject
    {
        PersistentStatus _persistentStatus;
        private RelayCommand _save;
        private IStorageFolder _storageFolder;
        private string _fileName;
        private IModelSerializer _modelSerializer;

        private bool _useThumbnails;
        private bool _sendFeedback;

        public static GeneralSettings Load(IStorageFolder storageFolder, string fileName, IModelSerializer modelSerializer)
        {
            Contract.Assert(null != storageFolder);
            Contract.Assert(!string.IsNullOrEmpty(fileName));
            Contract.Assert(null != modelSerializer);
            Contract.Ensures(null != Contract.Result<CertificateTrust>());

            GeneralSettings settings = null;

            using (Stream stream = storageFolder.OpenFile(fileName))
            {
                if (null != stream)
                {
                    settings = modelSerializer.ReadModel<GeneralSettings>(stream);
                }
            }

            if (null == settings)
                settings = new GeneralSettings();

            settings._storageFolder = storageFolder;
            settings._fileName = fileName;
            settings._modelSerializer = modelSerializer;
            settings._persistentStatus = PersistentStatus.Clean;

            return settings;
        }

        public GeneralSettings()
        {
            this.SetDefaults();
        }

        [DataMember]
        public bool UseThumbnails
        {
            get
            {
                return _useThumbnails;
            }
            set
            {
                if (SetProperty(ref _useThumbnails, value))
                    this.PersistentStatus = Data.PersistentStatus.Modified;
            }
        }

        [DataMember]
        public bool SendFeeback
        {
            get
            {
                return _sendFeedback;
            }
            set
            {
                if (SetProperty(ref _sendFeedback, value))
                    this.PersistentStatus = Data.PersistentStatus.Modified;
            }
        }

        private void SetDefaults()
        {
            this.UseThumbnails = true;
            this.SendFeeback = false;
        }

        ICommand IPersistentObject.Save
        {
            get { return GetOrCreateSaveCommand(); }
        }

        private PersistentStatus PersistentStatus
        {
            get { return _persistentStatus; }
            set
            {
                if (value != _persistentStatus)
                {
                    _persistentStatus = value;
                    GetOrCreateSaveCommand().EmitCanExecuteChanged();
                }
            }
        }

        private RelayCommand GetOrCreateSaveCommand()
        {
            if (null == _save)
            {
                _save = new RelayCommand(this.Save, this.CanSave);
            }

            return _save;
        }

        private void Save(object parameter)
        {
            using (Stream stream = _storageFolder.CreateFile(_fileName))
            {
                if (null != stream)
                {
                    _modelSerializer.WriteModel(this, stream);
                    this.PersistentStatus = PersistentStatus.Clean;
                }
            }
        }

        private bool CanSave(object parameter)
        {
            return PersistentStatus.Clean != this.PersistentStatus;
        }
    }
}
