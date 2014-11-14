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

        private IDataStorage _storage;
        private bool _loaded;
        private ModelCollection<Desktop> _desktops;
        private ModelCollection<Credentials> _creds;

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

        public async Task LoadFromStorage()
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
                foreach (Desktop desktop in await _storage.LoadCollection(DESKTOP_COLLECTION_NAME))
                {
                    _desktops.Add(desktop);
                    desktop.PropertyChanged += desktop_PropertyChanged;
                }
                _desktops.CollectionChanged += desktopsChanged;

                _creds = new ModelCollection<Credentials>();
                foreach (Credentials cred in await _storage.LoadCollection(CREDENTIAL_COLLECTION_NAME))
                {
                    _creds.Add(cred);
                    cred.PropertyChanged += cred_PropertyChanged;
                }
                _creds.CollectionChanged += credentialsChanged;
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


        async void cred_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await _storage.SaveItem(CREDENTIAL_COLLECTION_NAME, sender as Credentials);
        }

        async void desktop_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await _storage.SaveItem(DESKTOP_COLLECTION_NAME, sender as Desktop);
        }

        async void desktopsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Desktop desktop in e.NewItems)
                {
                    await _storage.SaveItem(DESKTOP_COLLECTION_NAME, desktop);                    
                    desktop.PropertyChanged += desktop_PropertyChanged;                    
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Desktop desktop in e.OldItems)
                {
                    desktop.PropertyChanged -= desktop_PropertyChanged;
                    await _storage.DeleteItem(DESKTOP_COLLECTION_NAME, desktop);
                    
                }
            }
            else
            {
                throw new InvalidOperationException("Only add or remove operations are supported");
            }
        }

        async void credentialsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Credentials cred in e.NewItems)
                {
                    await _storage.SaveItem(CREDENTIAL_COLLECTION_NAME, cred);                    
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
                    await _storage.DeleteItem(CREDENTIAL_COLLECTION_NAME, cred);
                    
                }
            }
            else
            {
                throw new InvalidOperationException("Only add or remove operations are supported");
            }
        }
    }
}
