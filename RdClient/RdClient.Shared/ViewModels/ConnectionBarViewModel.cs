using RdClient.Shared.Models.Viewport;
using System.Collections.ObjectModel;

namespace RdClient.Shared.ViewModels
{
    public class ConnectionBarViewModel : ViewModelBase
    {

        private IViewport _viewport;
        private ReadOnlyObservableCollection<object> _connectionBarItems;
        private double _connectionBarPosition;
        public double ConnectionBarPosition{
            get { return _connectionBarPosition;  }
            set { this.SetProperty(ref _connectionBarPosition, value); }
        }
        
        public void MoveConnectionBar(double dx, double connectionBarWidth)
        {

            double viewPortWidth = _viewport.Size.Width;

            double maxLeft = -((viewPortWidth / 2) - (connectionBarWidth / 2));
            double maxRight = ((viewPortWidth / 2) - (connectionBarWidth / 2));
            
            if (ConnectionBarPosition + dx < maxLeft)
            {
                ConnectionBarPosition = maxLeft;
            }
            else if (ConnectionBarPosition + dx > maxRight)
            {
                ConnectionBarPosition = maxRight;
            }
            else
            {
                ConnectionBarPosition += dx;
            }
        }

        public ConnectionBarViewModel()
        {
        }

        public IViewport Viewport {
            set {
                _viewport = value;
            }
        }

        public ReadOnlyObservableCollection<object> ConnectionBarItems
        {
            get { return _connectionBarItems; }
            set { this.SetProperty(ref _connectionBarItems, value); }
        }

    }
}
