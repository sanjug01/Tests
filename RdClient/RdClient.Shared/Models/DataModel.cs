using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace RdClient.Shared.Models
{
    public class DataModel : IDataModel
    {
        public readonly string DESKTOP_COLLECTION_NAME = "Desktops";
        public readonly string CREDENTIAL_COLLECTION_NAME = "Credentials";
        public readonly string THUMBNAIL_COLLECTION_NAME = "Thumbnails";

        private IDataStorage _storage;
        private bool _loaded;
        private ModelCollection<Desktop> _desktops;
        private ModelCollection<Credentials> _creds;
        private ModelCollection<Thumbnail> _thumbnails;

        public DataModel ()
        {
            _storage = null;
            _loaded = false;
            _desktops = null;
            _creds = null;
        }

        public IDataStorage Storage
        {
            set 
            { 
                if (Loaded)
                {
                    throw new InvalidOperationException("Can't set storage after data has been loaded");
                }
                else if (value == null)
                {
                    throw new ArgumentNullException("Storage cannot be null");
                }
                else
                {
                    _storage = value;
                }
            }
        }

        public bool Loaded
        {
            get { return _loaded; }
        }

        public void LoadFromStorage()
        {            
            if (Loaded)
            {
                throw new InvalidOperationException("DataModel already loaded. Cannot load again as that would overwrite existing data");
            }
            else if (_storage == null)
            {
                throw new InvalidOperationException("Must set Storage first");
            }
            else
            {
                _desktops = new ModelCollection<Desktop>();
                foreach (Desktop desktop in _storage.LoadCollection(DESKTOP_COLLECTION_NAME))
                {
                    _desktops.Add(desktop);
                    desktop.PropertyChanged += desktop_PropertyChanged;
                }
                _desktops.CollectionChanged += desktopsChanged;

                _creds = new ModelCollection<Credentials>();
                foreach (Credentials cred in _storage.LoadCollection(CREDENTIAL_COLLECTION_NAME))
                {
                    _creds.Add(cred);
                    cred.PropertyChanged += cred_PropertyChanged;
                }
                _creds.CollectionChanged += credentialsChanged;

                _thumbnails = new ModelCollection<Thumbnail>();
                foreach (Thumbnail thumbnail in await _storage.LoadCollection(THUMBNAIL_COLLECTION_NAME))
                {
                    _thumbnails.Add(thumbnail);
                    thumbnail.PropertyChanged += thumbnail_PropertyChanged;
                }
                _thumbnails.CollectionChanged += thumbnailsChanged;
                _loaded = true;

            }
        }

        public ModelCollection<Desktop> Desktops
        {
            get 
            {
                if (!Loaded)
                {
                    throw new InvalidOperationException("Must load from storage first");
                }
                else
                {
                    return _desktops;
                }
            }
        }

        public ModelCollection<Credentials> Credentials
        {
            get
            {
                if (!Loaded)
                {
                    throw new InvalidOperationException("Must load from storage first");
                }
                else
                {
                    return _creds;
                }
            }
        }

        public ModelCollection<Thumbnail> Thumbnails
        {
            get
            {
                if (!Loaded)
                {
                    throw new InvalidOperationException("Must load from storage first");
                }
                else
                {
                    return _thumbnails;
                }
            }
        }


        void cred_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _storage.SaveItem(CREDENTIAL_COLLECTION_NAME, sender as Credentials);
        }

        void desktop_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _storage.SaveItem(DESKTOP_COLLECTION_NAME, sender as Desktop);
        }

        void thumbnail_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _storage.SaveItem(THUMBNAIL_COLLECTION_NAME, sender as Thumbnail);
        }

        void desktopsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Desktop desktop in e.NewItems)
                {
                    _storage.SaveItem(DESKTOP_COLLECTION_NAME, desktop);                    
                    desktop.PropertyChanged += desktop_PropertyChanged;                    
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Desktop desktop in e.OldItems)
                {
                    desktop.PropertyChanged -= desktop_PropertyChanged;
                    _storage.DeleteItem(DESKTOP_COLLECTION_NAME, desktop);
                    if (desktop.HasThumbnail)
                    {
                        this.Thumbnails.Remove(this.Thumbnails.GetItemWithId(desktop.ThumbnailId));
                    }                    
                }
            }
            else
            {
                throw new InvalidOperationException("Only add or remove operations are supported");
            }
        }

        void credentialsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Credentials cred in e.NewItems)
                {
                    _storage.SaveItem(CREDENTIAL_COLLECTION_NAME, cred);                    
                    cred.PropertyChanged += cred_PropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Credentials cred in e.OldItems)
                {
                    foreach (Desktop desktop in _desktops.Where(d => d.CredentialId == cred.Id))
                    {
                        desktop.CredentialId = Guid.Empty;
                    }
                    cred.PropertyChanged -= cred_PropertyChanged;
                    _storage.DeleteItem(CREDENTIAL_COLLECTION_NAME, cred);                    
                }
            }
            else
            {
                throw new InvalidOperationException("Only add or remove operations are supported");
            }
        }

        async void thumbnailsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Thumbnail thumb in e.NewItems)
                {
                    await _storage.SaveItem(THUMBNAIL_COLLECTION_NAME, thumb);
                    thumb.PropertyChanged += thumbnail_PropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Thumbnail thumb in e.OldItems)
                {
                    foreach (Desktop desktop in _desktops.Where(d => d.ThumbnailId == thumb.Id))
                    {
                        desktop.ThumbnailId = Guid.Empty;
                    }
                    thumb.PropertyChanged -= cred_PropertyChanged;
                    await _storage.DeleteItem(THUMBNAIL_COLLECTION_NAME, thumb);

                }
            }
            else
            {
                throw new InvalidOperationException("Only add or remove operations are supported");
            }
        }
    }
}
