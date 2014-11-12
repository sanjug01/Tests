using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Models
{
    public interface IDataStorageFactory
    {
        IDataStorage GetDataStorage();
    }

    public interface IDataStorage
    {
        Task<List<Desktop>> LoadDesktops();

        Task SaveDesktop(Desktop desktop);

        Task DeleteDesktop(Desktop desktop);

        Task<List<Credentials>> LoadCredentials();

        Task SaveCredential(Credentials credential);

        Task DeleteCredential(Credentials credential);
    }
}
