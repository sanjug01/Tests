namespace RdClient.Shared.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Input;

    public interface IModelCollection<TModel> : IPersistentObject where TModel : class, IPersistentStatus
    {
        ReadOnlyObservableCollection<IModelContainer<TModel>> Models { get; }
        Guid AddNewModel(TModel newModel);
        TModel GetModel(Guid id);
        bool HasModel(Guid id);
        TModel RemoveModel(Guid id);
    }
}
