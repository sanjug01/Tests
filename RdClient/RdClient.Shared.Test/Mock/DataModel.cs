using RdMock;
using RdClient.Shared.Models;
using System;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.Mock
{
    class DataModel : MockBase, IDataModel
    {
        public IDataStorage Storage { get; set; }

        public bool Loaded { get; set; }

        public Task LoadFromStorage()
        {
            return Task.FromResult(Invoke(new object[] { }));
        }

        public ModelCollection<Desktop> Desktops { get; set; }

        public ModelCollection<Credentials> Credentials { get; set; }
    }
}
