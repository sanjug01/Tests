namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Information about gateway and assigned credentials model used by a particular session.
    /// This object may be edited by the in-session credentials editing task.
    /// </summary>
    public sealed class SessionGateway : ISessionCredentials
    {
        private readonly GatewayModel _gateway;
        //
        // Credentials object used by the session; this is always a new object, not stored
        // in the data model.
        //
        private readonly CredentialsModel _credentials;
        //
        // Modification status of the password in _credentials - if the password has been
        // edited by user after the credentials object was loaded from the data model,
        // _modified is set to true. It remains true until a new credentials object
        // is loaded form the data model.
        //
        private bool _newPassword;
        private bool _hasGateway;

        public CredentialsModel Credentials
        {
            get { return _credentials; }
        }

        public GatewayModel Gateway
        {
            get { return _gateway; }
        }

        /// <summary>
        /// True if the password was modified by user.
        /// The property is sent to the RDC server so the server may reject the credentials
        /// and require user to type the password.
        /// </summary>
        public bool IsNewPassword
        {
            get { return _newPassword; }
        }

        public bool HasGateway
        {
            get { return _hasGateway; }
        }

        public void ApplySavedCredentials(IModelContainer<CredentialsModel> savedCredentials)
        {
            _credentials.PropertyChanged -= this.OnCredentialsPropertyChanged;
            savedCredentials.Model.CopyTo(_credentials);
            _newPassword = false;
            _credentials.PropertyChanged += this.OnCredentialsPropertyChanged;
        }

        public void SetChangedPassword(string changedPassword)
        {
            _credentials.PropertyChanged -= this.OnCredentialsPropertyChanged;
            _credentials.Password = changedPassword;
            _newPassword = true;
            _credentials.PropertyChanged += this.OnCredentialsPropertyChanged;
        }

        public Guid SaveCredentials(ApplicationDataModel dataModel)
        {
            Guid credentialsId = Guid.Empty;
            //
            //
            //
            foreach(IModelContainer<CredentialsModel> c in dataModel.Credentials.Models)
            {
                if(string.Equals(c.Model.Username, _credentials.Username, StringComparison.OrdinalIgnoreCase))
                {
                    credentialsId = c.Id;
                    _credentials.CopyTo(c.Model);
                    Contract.Assert(!Guid.Empty.Equals(credentialsId));
                    break;
                }
            }

            if (Guid.Empty.Equals(credentialsId))
            {
                CredentialsModel newCredentials = new CredentialsModel(_credentials);
                credentialsId = dataModel.Credentials.AddNewModel(newCredentials);
            }

            return credentialsId;
        }

        public SessionGateway()
        {
            _gateway = null;
            _credentials = null;
            _hasGateway = false;
            _newPassword = false;
        }

        public SessionGateway(
            IModelContainer<GatewayModel> savedGateway,
            IModelContainer<CredentialsModel> savedCredentials
            )
        {
            _gateway = new GatewayModel();

            savedGateway.Model.CopyTo(_gateway);
            if (_gateway.HasCredentials)
            {
                _credentials = new CredentialsModel();
                savedCredentials.Model.CopyTo(_credentials);
                _credentials.PropertyChanged += this.OnCredentialsPropertyChanged;
            }
            else
            {
                _credentials = null;
            }

            _newPassword = false;
            _hasGateway = true;
        }

        private void OnCredentialsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _newPassword = true;
        }
    }
}
