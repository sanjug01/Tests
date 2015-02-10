namespace RdClient.Shared.Data
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Persistent status of the model in a model container
    /// </summary>
    [DefaultValue(PersistentStatus.Clean)]
    public enum PersistentStatus
    {
        /// <summary>
        /// Model in the container is clean and does not need to be saved in persistent storage
        /// </summary>
        Clean,
        /// <summary>
        /// Model in the container is new and hasn't yet been saved in persistent storage
        /// </summary>
        New,
        /// <summary>
        /// Model in the container has been modified since it was loaded from persistent storage and needs to be saved
        /// </summary>
        Modified
    }

    public interface IModelContainer<TModel> : IPersistentStatus where TModel : class, IPersistentStatus
    {
        Guid Id { get; }
        TModel Model { get; }
    }
}
