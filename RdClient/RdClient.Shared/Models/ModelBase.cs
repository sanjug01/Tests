using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

using RdClient.Shared.Helpers;

namespace RdClient.Shared.Models
{
    [DataContract]
    public class ModelBase : INotifyPropertyChanged
    {
        
        private Guid _id;

        [DataMember]
        public Guid Id {
            get { return _id; }
            set { SetProperty(ref _id, value, "Id");  }
        }

        public ModelBase()
        {
            _id = Guid.NewGuid();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            Debug.WriteLine("propertyName: {0}, storage: {1}, value: {2}", propertyName, storage, value);

            if (object.Equals(storage, value))
            {
                return false;
            }
            else
            {
                storage = value;
                this.OnPropertyChanged(propertyName);
                return true;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
