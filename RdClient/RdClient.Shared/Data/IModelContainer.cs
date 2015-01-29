namespace RdClient.Shared.Data
{
    using System;
    using System.ComponentModel;

    public enum PersistentStatus
    {
        Clean,
        New,
        Modified
    }

    public interface IModelContainer<TModel> : INotifyPropertyChanged where TModel : class, INotifyPropertyChanged
    {
        Guid Id { get; }
        TModel Model { get; }
        PersistentStatus Status { get; set; }
    }
}
