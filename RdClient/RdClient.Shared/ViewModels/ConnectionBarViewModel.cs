using RdClient.Shared.Models.Viewport;
using System.Collections.ObjectModel;

namespace RdClient.Shared.ViewModels
{
    public class ConnectionBarViewModel : ViewModelBase
    {

        private double _position;
        private ReadOnlyObservableCollection<object> _items;
        public double Position{
            get { return _position;  }
            set { this.SetProperty(ref _position, value); }
        }

        public ReadOnlyObservableCollection<object> Items
        {
            get { return _items; }
            set { this.SetProperty(ref _items, value); }
        }
        
        public void MoveConnectionBar(double dx, double connectionBarWidth, double containerWidth)
        {
            
            double maxLeft = -((containerWidth / 2) - (connectionBarWidth / 2));
            double maxRight = ((containerWidth / 2) - (connectionBarWidth / 2));
            
            if (Position + dx < maxLeft)
            {
                Position = maxLeft;
            }
            else if (Position + dx > maxRight)
            {
                Position = maxRight;
            }
            else
            {
                Position += dx;
            }
        }

    }
}
