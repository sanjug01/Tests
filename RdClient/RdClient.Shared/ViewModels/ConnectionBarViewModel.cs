using RdClient.Shared.Models.Viewport;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Input;

namespace RdClient.Shared.ViewModels
{
    public class ConnectionBarViewModel : ViewModelBase
    {

        private IViewport _viewport;
        private ReadOnlyObservableCollection<object> _connectionBarItems;
        private bool _isConnectionBarVisible;

        private double _connectionBarPosition;
        public double ConnectionBarPosition{
            get { return _connectionBarPosition;  }
            set { this.SetProperty(ref _connectionBarPosition, value); }
        }
        
        public void MoveConnectionBar(ManipulationDeltaRoutedEventArgs e, double connectionBarWidth)
        {

            double dx = e.Delta.Translation.X;
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
            IsConnectionBarVisible = true;
        }

        public IViewport Viewport { set { _viewport = value; } }
        
        public bool IsConnectionBarVisible
        {
            get { return _isConnectionBarVisible; }
            set { this.SetProperty(ref _isConnectionBarVisible, value); }
        }

        public ReadOnlyObservableCollection<object> ConnectionBarItems
        {
            get { return _connectionBarItems; }
            set { this.SetProperty(ref _connectionBarItems, value); }
        }

    }
}
