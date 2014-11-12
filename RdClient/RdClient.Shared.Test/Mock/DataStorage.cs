using System.Collections.Generic;
using System.Threading.Tasks;

using RdMock;
using RdClient.Shared.Models;

namespace RdClient.Shared.Test.Mock
{
    class DataStorage : MockBase, IDataStorage
    {

        public async Task<List<Desktop>> LoadDesktops()
        {
            return (List <Desktop>)Invoke(new object[] { });
        }

        public async Task SaveDesktop(Desktop desktop)
        {
            Invoke(new object[] { desktop });
        }

        public async Task DeleteDesktop(Desktop desktop)
        {
            Invoke(new object[] { desktop });
        }

        public async Task<List<Credentials>> LoadCredentials()
        {
            return (List<Credentials>)Invoke(new object[] { });
        }

        public async Task SaveCredential(Credentials credential)
        {
            Invoke(new object[] { credential });
        }

        public async Task DeleteCredential(Credentials credential)
        {
            Invoke(new object[] { credential });
        }
    }
}
