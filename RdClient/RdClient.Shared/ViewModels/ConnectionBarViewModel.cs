using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.ViewModels
{
    public class ConnectionBarViewModel : ViewModelBase
    {

        public ConnectionBarViewModel()
        {
            IsConnectionBarVisible = true;
        }

        private bool _isConnectionBarVisible;
        public bool IsConnectionBarVisible
        {
            get { return _isConnectionBarVisible; }
            set { this.SetProperty(ref _isConnectionBarVisible, value); }
        }

        private ReadOnlyObservableCollection<object> _connectionBarItems;
        public ReadOnlyObservableCollection<object> ConnectionBarItems
        {
            get { return _connectionBarItems; }
            set { this.SetProperty(ref _connectionBarItems, value); }
        }

    }
}
