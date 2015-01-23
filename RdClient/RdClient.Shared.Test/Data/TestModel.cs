namespace RdClient.Shared.Test.Data
{
    using RdClient.Shared.Helpers;
    using System.Runtime.Serialization;

    [DataContract]
    sealed class TestModel : MutableObject
    {
        [DataMember(Name="property")]
        private int _property;

        public TestModel(int propertyValue)
        {
            _property = propertyValue;
        }

        public int Property
        {
            get { return _property; }
            set { this.SetProperty(ref _property, value); }
        }
    }
}
