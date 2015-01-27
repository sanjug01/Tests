namespace RdClient.Shared.Data
{
    using RdClient.Shared.Models;
    using System;
    using System.IO;
    using System.Runtime.Serialization;

    public class SerializableModelSerializer : IModelSerializer
    {
        private readonly DataContractSerializer _contractSerializer;

        public SerializableModelSerializer()
        {
            DataContractSerializerSettings settings = new DataContractSerializerSettings()
            {
                PreserveObjectReferences = true,
            };
            _contractSerializer = new DataContractSerializer(typeof(SerializableModel), settings);
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
