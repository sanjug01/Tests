namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Core data model of the application - collection of all persistent objects.
    /// </summary>
    public sealed class RdDataModel
    {
        private readonly LocalWorkspace _localWorkspace;
        private readonly ModelCollection<OnPremiseWorkspace> _onPremiseWorkspaces;
        private readonly ModelCollection<CloudWorkspace> _cloudWorkspaces;
        private readonly ModelCollection<TrustedCertificate> _trustedCertificates;
        private readonly ModelCollection<Thumbnail> _thumbnails;
        private readonly GeneralSettings _settings;
        IDataStorage _storage;

        public LocalWorkspace LocalWorkspace
        {
            get { return _localWorkspace; }
        }

        public ModelCollection<OnPremiseWorkspace> OnPremiseWorkspaces
        {
            get { return _onPremiseWorkspaces; }
        }

        public ModelCollection<CloudWorkspace> CloudWorkspaces
        {
            get { return _cloudWorkspaces; }
        }

        public ModelCollection<TrustedCertificate> TrustedCertificates
        {
            get { return _trustedCertificates; }
        }

        public ModelCollection<Thumbnail> Thumbnails
        {
            get { return _thumbnails; }
        }

        public GeneralSettings Settings
        {
            get { return _settings; }
        }

        public IDataStorage Storage
        {
            get { return _storage; }
            set { _storage = value; }
        }

        public RdDataModel()
        {
            _localWorkspace = new LocalWorkspace(this);
            _onPremiseWorkspaces = new ModelCollection<OnPremiseWorkspace>();
            _cloudWorkspaces = new ModelCollection<CloudWorkspace>();
            _trustedCertificates = new ModelCollection<TrustedCertificate>();
            _thumbnails = new ModelCollection<Thumbnail>();
            _settings = new GeneralSettings();
        }

        public void Load()
        {
            Contract.Assert(null != _storage);
            _storage.Load(this);
        }

        public void Save()
        {
            Contract.Assert(null != _storage);
            _storage.Save(this);
        }

        public void TrustCertificate(IRdpCertificate certificate)
        {
            Contract.Requires(null != certificate);
            //
            // Add the certificate to the white list.
            //
            // TODO: optimize certificate matching
            //
            try
            {
                _trustedCertificates.First(cert => cert.IsMatchingCertificate(certificate));
            }
            catch(InvalidOperationException)
            {
                _trustedCertificates.Add(new TrustedCertificate(certificate));
            }
        }

        public bool IsCertificateTrusted(IRdpCertificate certificate)
        {
            //
            // Find the certificate in the white list
            //
            // TODO: Optimize certificate lookup
            //
            bool trusted = false;

            try
            {
                _trustedCertificates.First(c => c.IsMatchingCertificate(certificate));
                trusted = true;
            }
            catch(InvalidOperationException)
            {
                //
                // There is no trust; just assert that there isnt't
                //
                Contract.Assert(!trusted);
            }

            return trusted;
        }

        public void ClearAllTrustedCertificates()
        {
            //
            // Remove all trusted certificates
            //
            _trustedCertificates.Clear();
        }
    }
}
