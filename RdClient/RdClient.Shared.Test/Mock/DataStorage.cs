using System.Collections.Generic;
using System.Threading.Tasks;

using RdMock;
using RdClient.Shared.Models;

namespace RdClient.Shared.Test.Mock
{
    class DataStorage : MockBase, IDataStorage
    {
        public IEnumerable<string> GetCollectionNames()
        {
            return (IEnumerable<string>) Invoke(new object[] { });
        }        

        public IEnumerable<ModelBase> LoadCollection(string collectionName)
        {
            return (IEnumerable<ModelBase>) Invoke(new object[] { collectionName });
        }

        public void DeleteCollection(string collectionName)
        {
            Invoke(new object[] { collectionName });
        }

        public void SaveItem(string collectionName, ModelBase item)
        {
            Invoke(new object[] { collectionName, item });
        }

        public void DeleteItem(string collectionName, ModelBase item)
        {
            Invoke(new object[] { collectionName, item });
        }
    }
}
