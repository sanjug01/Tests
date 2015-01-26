namespace RdClient.Shared.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public interface IModelCollection<TModel> where TModel : INotifyPropertyChanged
    {
        ReadOnlyObservableCollection<IModelContainer<TModel>> Models { get; }
        Guid AddNewModel(TModel newModel);
        TModel GetModel(Guid id);
        TModel RemoveModel(Guid id);

        /// <summary>
        /// Save the collection to the same media from that it was loaded.
        /// </summary>
        void Save();
    }
}
