namespace RdClient.Shared.Data
{
    using RdClient.Shared.Models;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    /// <summary>
    /// Base class for all classes saved by the application's data model.
    /// </summary>
    [DataContract(IsReference=true)]
    [KnownType(typeof(CredentialsModel))]
    [KnownType(typeof(LocalWorkspaceModel))]
    [KnownType(typeof(OnPremiseWorkspaceModel))]
    [KnownType(typeof(CloudWorkspaceModel))]
    [KnownType(typeof(RemoteConnectionModel))]
    [KnownType(typeof(DesktopModel))]
    [KnownType(typeof(TrustedCertificate))]
    [KnownType(typeof(CertificateTrust))]
    [KnownType(typeof(GeneralSettings))]
    public abstract class SerializableModel : IPersistentStatus
    {
        private PropertyChangedEventHandler _propertyChanged;
        private PersistentStatus _persistentStatus;

        protected SerializableModel()
        {
        }

        public override bool Equals(object obj)
        {
            bool equals = false;

            SerializableModel otherModel = obj as SerializableModel;

            if(null != otherModel)
            {
                equals = otherModel.GetType().Equals(this.GetType());
            }

            return equals;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected bool SetProperty<TProperty>(ref TProperty property, TProperty newValue,
            [CallerMemberName] string propertyName = null)
        {
            bool valueChanged = !object.Equals(property, newValue);

            if(valueChanged)
            {
                property = newValue;
                EmitPropertyChanged(propertyName);
                SetModified();
            }

            return valueChanged;
        }

        protected void EmitPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Contract.Assert(null != propertyName);

            PropertyChangedEventHandler handler = _propertyChanged;

            if (null != handler)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetModified()
        {
            if( _persistentStatus == PersistentStatus.Clean )
            {
                _persistentStatus = PersistentStatus.Modified;
                EmitPropertyChanged("Status");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }

        public PersistentStatus Status
        {
            get { return _persistentStatus; }
        }

        void IPersistentStatus.SetClean()
        {
            _persistentStatus = PersistentStatus.Clean;
            Cleaned();
        }

        protected virtual void Cleaned()
        {
        }
    }
}
