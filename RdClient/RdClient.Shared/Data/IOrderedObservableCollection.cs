namespace RdClient.Shared.Data
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public interface IOrderedObservableCollection<TModel> : INotifyPropertyChanged where TModel : class, INotifyPropertyChanged
    {
        ReadOnlyObservableCollection<TModel> Models { get; }

        IComparer<TModel> Order { get; set; }
    }
}
