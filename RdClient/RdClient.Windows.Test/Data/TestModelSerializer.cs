namespace RdClient.Windows.Test.Data
{
    using RdClient.Shared.Data;
    using System;
    using System.IO;
    using System.Runtime.Serialization;

    sealed class TestModelSerializer : IModelSerializer
    {
        private readonly DataContractSerializer _contractSerializer;

        public TestModelSerializer()
        {
            _contractSerializer = new DataContractSerializer(typeof(TestModel));
        }

        TModel IModelSerializer.ReadModel<TModel>(Stream stream)
        {
            return (TModel)_contractSerializer.ReadObject(stream);
        }

        void IModelSerializer.WriteModel<TModel>(TModel model, Stream stream)
        {
            _contractSerializer.WriteObject(stream, model);
        }
    }
}
