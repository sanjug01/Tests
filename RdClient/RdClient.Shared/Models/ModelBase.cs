using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace RdClient.Shared.Models
{
    [DataContract(IsReference=true)]
    [KnownType(typeof(Desktop))]
    [KnownType(typeof(Credentials))]
    [KnownType(typeof(Thumbnail))]
    public class ModelBase : INotifyPropertyChanged, IEquatable<ModelBase>
    {
        [DataMember]
        private Guid _id;
        
        public Guid Id {
            get { return _id; }
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

        public override bool Equals(object obj)
        {
            return (obj != null && (obj == this || this.Equals(obj as ModelBase)));
        }                  

        public bool Equals(ModelBase modelBaseObj)
        {
            return ((modelBaseObj != null) && (this.Id.Equals(modelBaseObj.Id)));
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
