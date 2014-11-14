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
        private ModelCollection<Desktop> _desktops;
        private ModelCollection<Credentials> _creds;

        private DataModel (IDataStorage storage)
        {
            _storage = storage;
            _desktops = new ModelCollection<Desktop>();
            _creds = new ModelCollection<Credentials>();
        }

        public static async Task<IDataModel> NewDataModel(IDataStorage storage)
        {
            DataModel dataModel = new DataModel(storage);
            await dataModel.LoadFromStorage();
            return dataModel;
        }

        private async Task LoadFromStorage()
        {            
            foreach (Desktop desktop in await _storage.LoadDesktops())
            {
                _desktops.Add(desktop);
                desktop.PropertyChanged += desktop_PropertyChanged;
            }
            _desktops.CollectionChanged += desktopsChanged;            
            foreach (Credentials cred in await _storage.LoadCredentials())
            {
                _creds.Add(cred);
                cred.PropertyChanged += cred_PropertyChanged;
            }
            _creds.CollectionChanged += credentialsChanged;
        }


        public ModelCollection<Desktop> Desktops
        {
            get { return _desktops; }
        }

        public ModelCollection<Credentials> Credentials
        {
            get { return _creds; }
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
                    await _storage.SaveDesktop(desktop);                    
                    desktop.PropertyChanged += desktop_PropertyChanged;                    
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Desktop desktop in e.OldItems)
                {
                    desktop.PropertyChanged -= desktop_PropertyChanged;
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
                    await _storage.SaveCredential(cred);                    
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
