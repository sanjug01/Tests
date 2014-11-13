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

        public IDataStorage Storage
        {
            set 
            { 
                _storage = value;
            }
        }

        public ObservableCollection<Desktop> Desktops
        {
            get { return _desktops; }
        }

        public ObservableCollection<Credentials> Credentials
        {
            get { return _creds; }
        }

        public Desktop GetDesktopWithId(Guid id)
        {
            Desktop desktop = TryGetDesktopWithId(id);
            if (desktop == null)
            {
                throw new ArgumentOutOfRangeException("Desktop with that id was not found" + id);
            }
            return desktop;
        }

        private Desktop TryGetDesktopWithId(Guid id)
        {
            return _desktops.Where(d => d.Id == id).FirstOrDefault();
        }

        private Credentials TryGetCredentialWithId(Guid id)
        {
            return _creds.Where(c => c.Id == id).FirstOrDefault();
        }

        public Credentials GetCredentialWithId(Guid id)
        {
            Credentials cred = TryGetCredentialWithId(id);
            if (cred == null)
            {
                throw new ArgumentOutOfRangeException("Credential with that id was not found" + id);
            }
            return cred;
        }

        public async Task LoadFromStorage()
        {
            if (_desktops != null)
            {                
                _desktops.CollectionChanged -= _desktops_CollectionChanged;
            }
            _desktops = new ObservableCollection<Desktop>(await _storage.LoadDesktops());
            _desktops.CollectionChanged += _desktops_CollectionChanged;

            if (_creds != null)
            {
                _creds.CollectionChanged -= _creds_CollectionChanged;
            }
            _creds = new ObservableCollection<Credentials>(await _storage.LoadCredentials());
            _creds.CollectionChanged += _creds_CollectionChanged;
        }

        async void _creds_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IList removedItems = e.OldItems;
            IList addedItems = e.NewItems;
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                removedItems = _creds;
                addedItems = sender as IList;
            }
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (Credentials cred in addedItems)
                {
                    if (TryGetCredentialWithId(cred.Id) != null)
                    {
                        throw new ArgumentException("Cannot add duplicate credential. A credential with that Id is already present: " + cred.Id);
                    }
                    await _storage.SaveCredential(cred);
                    cred.PropertyChanged += cred_PropertyChanged;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (Credentials cred in removedItems)
                {
                    cred.PropertyChanged -= cred_PropertyChanged;
                    foreach (Desktop desktop in _desktops.Where(d => d.CredentialId == cred.Id))
                    {
                        desktop.CredentialId = Guid.Empty;
                    }
                    await _storage.DeleteCredential(cred);
                }
            }
        }

        async void cred_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await _storage.SaveCredential(sender as Credentials);
        }

        async void _desktops_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IList removedItems = e.OldItems;
            IList addedItems = e.NewItems;
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                removedItems = _desktops;
                addedItems = sender as IList;
            }
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (Desktop desktop in addedItems)
                {
                    if (TryGetDesktopWithId(desktop.Id) != null)
                    {
                        throw new ArgumentException("Cannot add duplicate desktop. A desktop with that Id is already present: " + desktop.Id);
                    }
                    await _storage.SaveDesktop(desktop);
                    desktop.PropertyChanged += desktop_PropertyChanged;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (Desktop desktop in removedItems)
                {
                    desktop.PropertyChanged -= desktop_PropertyChanged;
                    await _storage.DeleteDesktop(desktop);
                }
            }
        }

        async void desktop_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await _storage.SaveDesktop(sender as Desktop);
        }
    }
}
