namespace RdClient.Shared.Data
{
    using System;
    using System.ComponentModel;

    public enum ModelStatus
    {
        Clean,
        New,
        Modified
    }

    public interface IModelContainer<TModel> where TModel : INotifyPropertyChanged
    {
        Guid Id { get; }
        TModel Model { get; }
        ModelStatus Status { get; set; }
    }
}
