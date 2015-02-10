namespace RdClient.Shared.Test.Data
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    [DataContract]
    sealed class TestModel : IPersistentStatus
    {
        [DataMember(Name="property")]
        private int _property;

        private PersistentStatus _status;

        public TestModel()
        {
            //
            // Default constructor for serialization
            //
            _property = 0;
        }

        public TestModel(int propertyValue)
        {
            _property = propertyValue;
        }

        public int Property
        {
            get { return _property; }
            set
            {
                if(value != _property)
                {
                    _property = value;
                    if (null != this.PropertyChanged)
                        this.PropertyChanged(this, new PropertyChangedEventArgs("Property"));

                    if(PersistentStatus.Clean == _status)
                    {
                        _status = PersistentStatus.Modified;
                        if (null != this.PropertyChanged)
                            this.PropertyChanged(this, new PropertyChangedEventArgs("Status"));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PersistentStatus Status
        {
            get { return _status; }
        }

        void IPersistentStatus.SetClean()
        {
            _status = PersistentStatus.Clean;
        }
    }
}
