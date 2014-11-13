using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Models
{
    public interface IDataModelFactory
    {
        IDataModel GetDataModel();
    }

    public interface IDataModel
    {
        IDataStorage Storage
        {
            set;
        }

        Task LoadFromStorage();

        ObservableCollection<Desktop> Desktops
        {
            get;
        }

        ObservableCollection<Credentials> Credentials
        {
            get;
        }

        Desktop GetDesktopWithId(Guid id);

        Credentials GetCredentialWithId(Guid id);
    }
}
