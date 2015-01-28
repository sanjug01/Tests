namespace RdClient.Shared.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Input;

    public interface IModelCollection<TModel> where TModel : class, INotifyPropertyChanged
    {
        ReadOnlyObservableCollection<IModelContainer<TModel>> Models { get; }
        Guid AddNewModel(TModel newModel);
        TModel GetModel(Guid id);
        TModel RemoveModel(Guid id);

        /// <summary>
        /// Save the collection to the same media from that it was loaded.
        /// </summary>
        ICommand Save { get; }
    }
}
