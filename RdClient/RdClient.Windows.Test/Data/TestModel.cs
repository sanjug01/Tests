namespace RdClient.Windows.Test.Data
{
    using RdClient.Shared.Helpers;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    [DataContract]
    sealed class TestModel : INotifyPropertyChanged
    {
        [DataMember(Name="property")]
        private int _property;

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
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
