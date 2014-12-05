
namespace RdClient.Shared.Models
{
    public interface IDataModel
    {
        IDataStorage Storage
        {
            set;
        }

        bool Loaded
        {
            get;
        }

        void LoadFromStorage();

        ModelCollection<Desktop> Desktops
        {
            get;
        }

        ModelCollection<Credentials> Credentials
        {
            get;
        }

        ModelCollection<Thumbnail> Thumbnails
        {
            get;
        }
    }
}
