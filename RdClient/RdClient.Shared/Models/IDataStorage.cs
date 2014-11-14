using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Models
{
    public interface IDataStorage
    {
        Task<IEnumerable<ModelBase>> LoadCollection(string collectionName);

        Task SaveItem(string collectionName, ModelBase item);

        Task DeleteItem(string collectionName, ModelBase item);
    }
}
