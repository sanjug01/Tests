using System.Collections.Generic;
using System.Threading.Tasks;

using RdMock;
using RdClient.Shared.Models;

namespace RdClient.Shared.Test.Mock
{
    class DataStorage : MockBase, IDataStorage
    {
        public Task<IEnumerable<ModelBase>> LoadCollection(string collectionName)
        {
            return Task.FromResult((IEnumerable<ModelBase>)Invoke(new object[] { collectionName }));
        }

        public Task SaveItem(string collectionName, ModelBase item)
        {
            return Task.FromResult(Invoke(new object[] { collectionName, item }));
        }

        public Task DeleteItem(string collectionName, ModelBase item)
        {
            return Task.FromResult(Invoke(new object[] { collectionName, item }));
        }
    }
}
