using RdClient.Shared.Models.Viewport;
using System.Collections.ObjectModel;

namespace RdClient.Shared.ViewModels
{
    public class ConnectionBarViewModel : ViewModelBase
    {

        private double _connectionBarPosition;
        private ReadOnlyObservableCollection<object> _connectionBarItems;
        public double ConnectionBarPosition{
            get { return _connectionBarPosition;  }
            set { this.SetProperty(ref _connectionBarPosition, value); }
        }

        public ReadOnlyObservableCollection<object> ConnectionBarItems
        {
            get { return _connectionBarItems; }
            set { this.SetProperty(ref _connectionBarItems, value); }
        }
        
        public void MoveConnectionBar(double dx, double connectionBarWidth, double containerWidth)
        {
            
            double maxLeft = -((containerWidth / 2) - (connectionBarWidth / 2));
            double maxRight = ((containerWidth / 2) - (connectionBarWidth / 2));
            
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

    }
}
