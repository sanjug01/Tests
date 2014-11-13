using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Models
{
    public interface IDataModel
    {
        ObservableCollection<Desktop> Desktops
        {
            get;
        }

        ObservableCollection<Credentials> Credentials
        {
            get;
        }

        ModelBase GetObjectWithId(Guid id);

        bool ContainsObjectWithId(Guid id);
    }
}
