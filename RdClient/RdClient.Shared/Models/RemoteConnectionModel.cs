namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
using System.ComponentModel;
using System.Runtime.Serialization;

    [DataContract(IsReference=true)]
    public abstract class RemoteConnectionModel : SerializableModel
    {
        [DataMember(Name = "Thumbnail", IsRequired = false, EmitDefaultValue = false)]
        private ThumbnailModel _thumbnail;

        protected RemoteConnectionModel()
        {
        }

        public ThumbnailModel Thumbnail
        {
            get
            {
                if (null == _thumbnail)
                {
                    _thumbnail = new ThumbnailModel();
                    _thumbnail.PropertyChanged += this.OnThumbnailChanged;
                }
                return _thumbnail;
            }
        }

        protected virtual void OnThumbnailChanged(IThumbnailEncoder sender, PropertyChangedEventArgs e)
        {
        }

        private void OnThumbnailChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnThumbnailChanged((IThumbnailEncoder)sender, e);
        }
    }
}
