namespace RdClient.Shared.Data
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;
    using RdClient.Shared.Helpers;
    using System.Windows.Input;
    using RdClient.Shared.ViewModels;
    using System.IO;

    [DataContract(IsReference=true)]
    public sealed class ServerIdentityTrust : SerializableModel, IServerIdentityTrust, IPersistentObject
    {
        PersistentStatus _persistentStatus;
        private RelayCommand _save;
        private IStorageFolder _storageFolder;
        private string _fileName;
        private IModelSerializer _modelSerializer;

        /////// <summary>
        /////// Sorted list of trusted servers.
        /////// </summary>
        ////[DataMember(Name = "TrustedServers", EmitDefaultValue = false)]
        ////private List<TrustedCertificate> _trustedCertificates;


        public static ServerIdentityTrust Load(IStorageFolder storageFolder, string fileName, IModelSerializer modelSerializer)
        {
            Contract.Assert(null != storageFolder);
            Contract.Assert(!string.IsNullOrEmpty(fileName));
            Contract.Assert(null != modelSerializer);
            Contract.Ensures(null != Contract.Result<CertificateTrust>());

            ServerIdentityTrust certificateTrust = null;

            using(Stream stream = storageFolder.OpenFile(fileName))
            {
                if (null != stream)
                {
                    certificateTrust = modelSerializer.ReadModel<ServerIdentityTrust>(stream);
                }
            }

            if (null == certificateTrust)
                certificateTrust = new ServerIdentityTrust();

            certificateTrust._storageFolder = storageFolder;
            certificateTrust._fileName = fileName;
            certificateTrust._modelSerializer = modelSerializer;
            certificateTrust._persistentStatus = PersistentStatus.Clean;

            return certificateTrust;
        }

        public ServerIdentityTrust()
        {
        }

        void IServerIdentityTrust.TrustServer(string hostName)
        {
            ////IList<TrustedCertificate> certificates = GetOrCreateCertificates();

            ////TrustedCertificate trustedCertificate = new TrustedCertificate(certificate);
            ////int insertionIndex = certificates.IndexOfFirstGreaterOrEqual(trustedCertificate, _comparer);

            ////if(insertionIndex < 0)
            ////{
            ////    _trustedCertificates.Add(trustedCertificate);
            ////}
            ////else if( _comparer.Compare(trustedCertificate, _trustedCertificates[insertionIndex]) < 0 )
            ////{
            ////    _trustedCertificates.Insert(insertionIndex, trustedCertificate);
            ////}

            ////this.PersistentStatus = PersistentStatus.Modified;
        }

        void IServerIdentityTrust.RemoveAllTrust()
        {
            ////if (null != _trustedCertificates)
            ////{
            ////    _trustedCertificates.Clear();
            ////    _trustedCertificates = null;
            ////    this.PersistentStatus = PersistentStatus.Modified;
            ////}
        }

        bool IServerIdentityTrust.IsServerTrusted(string hostName)
        {
            bool trusted = false;

            //if (null != _trustedCertificates)
            //{
            //    trusted = _trustedCertificates.BinarySearch(new TrustedCertificate(certificate), _comparer) >= 0;
            //}

            return trusted;
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
                if(value != _persistentStatus)
                {
                    _persistentStatus = value;
                    GetOrCreateSaveCommand().EmitCanExecuteChanged();
                }
            }
        }

        private RelayCommand GetOrCreateSaveCommand()
        {
            if(null == _save)
            {
                _save = new RelayCommand(this.Save, this.CanSave);
            }

            return _save;
        }

        private void Save(object parameter)
        {
            using(Stream stream = _storageFolder.CreateFile(_fileName))
            {
                if(null != stream)
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

        private List<TrustedCertificate> GetOrCreateCertificates()
        {
            ////if (null == _trustedCertificates)
            ////    _trustedCertificates = new List<TrustedCertificate>();
            ////return _trustedCertificates;
            return null;
        }
    }
}
