using System.Collections.Generic;
using System.Threading.Tasks;

using RdMock;
using RdClient.Shared.Models;

namespace RdClient.Shared.Test.Mock
{
    class DataStorage : MockBase, IDataStorage
    {

        public Task<IList<Desktop>> LoadDesktops()
        {
            return Task.FromResult((IList <Desktop>)Invoke(new object[] { }));
        }

        public Task SaveDesktop(Desktop desktop)
        {
            return Task.FromResult(Invoke(new object[] { desktop }));
        }

        public Task DeleteDesktop(Desktop desktop)
        {
            return Task.FromResult(Invoke(new object[] { desktop }));
        }

        public Task<IList<Credentials>> LoadCredentials()
        {
            return Task.FromResult((IList<Credentials>)Invoke(new object[] { }));
        }

        public Task SaveCredential(Credentials credential)
        {
            return Task.FromResult(Invoke(new object[] { credential }));
        }

        public Task DeleteCredential(Credentials credential)
        {
            return Task.FromResult(Invoke(new object[] { credential }));
        }
    }
}
