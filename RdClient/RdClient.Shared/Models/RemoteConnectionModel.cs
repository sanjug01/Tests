namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

    [DataContract(IsReference=true)]
    public abstract class RemoteConnectionModel : SerializableModel
    {
        private static readonly uint ThumbnailHeight = 276;

        [DataMember(Name = "Thumbnail", IsRequired = false, EmitDefaultValue = false)]
        private ThumbnailModel _thumbnail;

        private IThumbnailEncoder _thumbnailEncoder;

        protected RemoteConnectionModel()
        {
            _thumbnailEncoder = ThumbnailEncoder.Create(ThumbnailHeight);
            _thumbnailEncoder.ThumbnailUpdated += this.OnThumbnailEncoded;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            _thumbnailEncoder = ThumbnailEncoder.Create(ThumbnailHeight);
            _thumbnailEncoder.ThumbnailUpdated += this.OnThumbnailEncoded;
            if (null != _thumbnail)
                _thumbnail.PropertyChanged += this.OnThumbnailPropertyChanged;

        }

        public ThumbnailModel Thumbnail
        {
            get
            {
                if (null == _thumbnail)
                {
                    _thumbnail = new ThumbnailModel();
                    _thumbnail.PropertyChanged += this.OnThumbnailPropertyChanged;
                }
                return _thumbnail;
            }
        }

        public IThumbnailEncoder Encoder
        {
            get { return _thumbnailEncoder; }
        }

        protected override void Cleaned()
        {
            ((IPersistentStatus)_thumbnail).SetClean();
        }

        private void OnThumbnailPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetModified();
        }

        private void OnThumbnailEncoded(object sender, ThumbnailUpdatedEventArgs e)
        {
            Contract.Assert(null != _thumbnail);
            _thumbnail.UpdateEncodedBytes(e.EncodedImageBytes);
        }
    }
}
