namespace RdClient.Shared.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public interface IModelCollection<TModel> where TModel : INotifyPropertyChanged
    {
        ReadOnlyObservableCollection<IModelContainer<TModel>> Models { get; }
        Guid AddNewModel(TModel newModel);
        TModel RemoveModel(Guid id);
    }
}
