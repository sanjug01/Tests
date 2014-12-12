namespace RdClient.Shared.Models
{
    public interface IDataStorage
    {
        /// <summary>
        /// Populate the persistent data object with information loaded from storage.
        /// </summary>
        /// <param name="persistentData">Data object populated by the method.</param>
        /// <remarks>Contents of the persistent data object are reset before new data is loaded from storage.</remarks>
        void Load(RdDataModel dataModel);

        /// <summary>
        /// Write information from the persistent data object to storage.
        /// </summary>
        /// <param name="persistentData">Persistent data object whose inbformation the method writes to storage.</param>
        void Save(RdDataModel dataModel);
    }
}
