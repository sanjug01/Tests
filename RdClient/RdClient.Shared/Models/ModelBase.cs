using System;

using RdClient.Shared.Helpers;

namespace RdClient.Shared.Models
{
    public class ModelBase : MutableObject
    {
        private Guid _id;

        public Guid Id {
            get { return _id; }
            set { SetProperty(ref _id, value, "Id");  }
        }

        public ModelBase()
        {
            _id = Guid.NewGuid();
        }
    }
}
