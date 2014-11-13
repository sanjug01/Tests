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
        private IDataStorage _storage;
        private ObservableCollection<Desktop> _desktops;
        private ObservableCollection<Credentials> _creds;
        private IDictionary<Guid, ModelBase> _byId;

        private DataModel (IDataStorage storage)
        {
            _storage = storage;
            _byId = new Dictionary<Guid, ModelBase>();
        }

        public static async Task<IDataModel> NewDataModel(IDataStorage storage)
        {
            DataModel dataModel = new DataModel(storage);
            await dataModel.LoadFromStorage();
            return dataModel;
        }

        private async Task LoadFromStorage()
        {
            _desktops = new ObservableCollection<Desktop>(await _storage.LoadDesktops());
            foreach (Desktop desktop in _desktops)
            {
                _byId.Add(desktop.Id, desktop);
                desktop.PropertyChanged += desktop_PropertyChanged;
            }
            _desktops.CollectionChanged += desktopsChanged;
            _creds = new ObservableCollection<Credentials>(await _storage.LoadCredentials());
            foreach (Credentials cred in _creds)
            {
                _byId.Add(cred.Id, cred);
                cred.PropertyChanged += cred_PropertyChanged;
            }
            _creds.CollectionChanged += credentialsChanged;
        }
        

        public ObservableCollection<Desktop> Desktops
        {
            get { return _desktops; }
        }

        public ObservableCollection<Credentials> Credentials
        {
            get { return _creds; }
        }

        public bool ContainsObjectWithId(Guid id)
        {
            return _byId.ContainsKey(id);
        }        

        public ModelBase GetObjectWithId(Guid id)
        {
            ModelBase value;
            if (_byId.TryGetValue(id, out value))
            {
                return value;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Could not find an object with that id: " + id);
            }
        }

        async void cred_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await _storage.SaveCredential(sender as Credentials);
        }

        async void desktop_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await _storage.SaveDesktop(sender as Desktop);
        }

        async void desktopsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Desktop desktop in e.NewItems)
                {
                    if (ContainsObjectWithId(desktop.Id))
                    {
                        throw new InvalidOperationException("Cannot add desktop. An object with that Id is already present: " + desktop.Id);
                    }
                    if (desktop.CredentialId != Guid.Empty && !ContainsObjectWithId(desktop.CredentialId))
                    {
                        throw new InvalidOperationException("Cannot add desktop. It references a credential that has not been added to the model: " + desktop.CredentialId);
                    }
                    await _storage.SaveDesktop(desktop);
                    _byId.Add(desktop.Id, desktop);
                    desktop.PropertyChanged += desktop_PropertyChanged;                    
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Desktop desktop in e.OldItems)
                {
                    if (!ContainsObjectWithId(desktop.Id))
                    {
                        throw new InvalidOperationException("Cannot remove desktop. An object with that Id was not found: " + desktop.Id);
                    }
                    desktop.PropertyChanged -= desktop_PropertyChanged;
                    _byId.Remove(desktop.Id);
                    await _storage.DeleteDesktop(desktop);
                    
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
                    if (ContainsObjectWithId(cred.Id))
                    {
                        throw new InvalidOperationException("Cannot add credential. An object with that Id is already present: " + cred.Id);
                    }
                    await _storage.SaveCredential(cred);
                    _byId.Add(cred.Id, cred);
                    cred.PropertyChanged += cred_PropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Credentials cred in e.OldItems)
                {
                    if (!ContainsObjectWithId(cred.Id))
                    {
                        throw new InvalidOperationException("Cannot remove credential. An object with that Id was not found: " + cred.Id);
                    }
                    foreach (Desktop desktop in _desktops.Where(d => d.CredentialId == cred.Id))
                    {
                        desktop.CredentialId = Guid.Empty;
                    }
                    cred.PropertyChanged -= cred_PropertyChanged;
                    _byId.Remove(cred.Id);
                    await _storage.DeleteCredential(cred);
                    
                }
            }
            else
            {
                throw new InvalidOperationException("Only add or remove operations are supported");
            }
        }
    }
}
