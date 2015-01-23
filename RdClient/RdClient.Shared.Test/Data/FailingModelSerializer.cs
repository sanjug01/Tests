namespace RdClient.Shared.Test.Data
{
    using RdClient.Shared.Data;
    using System;
    using System.IO;

    sealed class FailingModelSerializer : IModelSerializer
    {
        TModel IModelSerializer.ReadModel<TModel>(Stream stream)
        {
            throw new NotImplementedException();
        }

        void IModelSerializer.WriteModel<TModel>(TModel model, Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
