namespace RdClient.Shared.Test.Data
{
    using RdClient.Shared.Helpers;

    sealed class TestModel : MutableObject
    {
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
