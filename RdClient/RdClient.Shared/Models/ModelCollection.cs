using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Models
{
    public class ModelCollection<T> : ObservableCollection<T> where T : ModelBase
    {     
        private IDictionary<Guid, T> _dictionary;

        public ModelCollection()
        {
            _dictionary = new Dictionary<Guid, T>();
        }

        public bool ContainsItemWithId(Guid id)
        {
            return _dictionary.ContainsKey(id);
        }

        public T GetItemWithId(Guid id)
        {
            T value;
            if (_dictionary.TryGetValue(id, out value))
            {
                return value;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Can't get item. Collection does not contain an item with that Id: " + id);
            }
        }

        protected override void InsertItem(int index, T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Can't add a null object");
            }
            else if (ContainsItemWithId(item.Id))
            {
                throw new ArgumentException("Cannot add item. Collection already contains an element with the same Id: " + item.Id);
            }
            else
            {
                base.InsertItem(index, item);
                _dictionary.Add(item.Id, item);
            }
        }

        protected override void RemoveItem(int index)
        {
            _dictionary.Remove(this[index].Id);
            base.RemoveItem(index);            
        }

        protected override void ClearItems()
        {
            throw new InvalidOperationException("Clearing collection is not supported");
        }

        protected override void SetItem(int index, T item)
        {
            throw new InvalidOperationException("Replacing an item is not supported");       
        }
    }
}
