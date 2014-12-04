using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Models
{
    public interface IDataStorage
    {
        IEnumerable<string> GetCollectionNames();

        IEnumerable<ModelBase> LoadCollection(string collectionName);

        void DeleteCollection(string collectionName);

        void SaveItem(string collectionName, ModelBase item);

        void DeleteItem(string collectionName, ModelBase item);
    }
}
