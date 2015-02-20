namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Information about credentials model used by a particular session.
    /// This object is edited by the in-session credentials editing task.
    /// </summary>
    public sealed class SessionCredentials
    {
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
        private bool _modified;

        public CredentialsModel Credentials
        {
            get { return _credentials; }
        }

        /// <summary>
        /// True if the object was modified by user.
        /// The property is sent to the RDC server so the server may reject the credentials
        /// and require user to type the password.
        /// </summary>
        public bool IsModified
        {
            get { return _modified; }
        }

        public void ApplySavedCredentials(IModelContainer<CredentialsModel> savedCredentials)
        {
            _credentials.PropertyChanged -= this.OnCredentialsPropertyChanged;
            savedCredentials.Model.CopyTo(_credentials);
            _modified = false;
            _credentials.PropertyChanged += this.OnCredentialsPropertyChanged;
        }

        public SessionCredentials()
        {
            _credentials = new CredentialsModel();
            //
            // There is no loaded model, so any password typed in will be modified.
            //
            _modified = true;
            _credentials.PropertyChanged += this.OnCredentialsPropertyChanged;
        }

        public SessionCredentials(IModelContainer<CredentialsModel> savedCredentials)
        {
            Contract.Assert(null != savedCredentials);

            _credentials = new CredentialsModel();
            savedCredentials.Model.CopyTo(_credentials);
            _modified = false;
            _credentials.PropertyChanged += this.OnCredentialsPropertyChanged;
        }

        private void OnCredentialsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _modified = true;
        }
    }
}
