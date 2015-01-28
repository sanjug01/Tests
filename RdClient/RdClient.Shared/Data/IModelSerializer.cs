namespace RdClient.Shared.Data
{
    using System.ComponentModel;
    using System.IO;

    public interface IModelSerializer
    {
        TModel ReadModel<TModel>(Stream stream) where TModel : class, INotifyPropertyChanged;

        void WriteModel<TModel>(TModel model, Stream stream) where TModel : class, INotifyPropertyChanged;
    }
}
