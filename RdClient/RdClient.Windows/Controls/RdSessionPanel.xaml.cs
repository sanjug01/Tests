// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Controls
{
    using RdClient.CxWrappers.Utils;
    using RdClient.Shared.Models;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class RdSessionPanel : UserControl
    {

        private const double MAX_ZOOM_FACTOR = 2.5;

        public DependencyProperty ConnectCommandProperty = DependencyProperty.Register("ConnectCommand",
            typeof(ICommand), typeof(RdSessionPanel),
            new PropertyMetadata(null, OnConnectCommandPropertyChanged));

        public ICommand ConnectCommand
        {
            get { return (ICommand)GetValue(ConnectCommandProperty); }
            set { SetValue(ConnectCommandProperty, value); }
        }

        public static readonly DependencyProperty ViewSizeProperty = DependencyProperty.Register(
            "ViewSize", typeof(Size),
            typeof(RdSessionPanel), new PropertyMetadata(true));
        public Size ViewSize
        {
            private get { return (Size)GetValue(ViewSizeProperty); }
            set { SetValue(ViewSizeProperty, value); }
        }


        public RdSessionPanel()
        {
            this.InitializeComponent();
            this.SizeChanged += (sender, args) => { this.ViewSize = (args as SizeChangedEventArgs).NewSize; };
        }

        public void ZoomIn()
        {
            double ScaleFactor = MAX_ZOOM_FACTOR;
            SwapChainPanelScaleAnimationX.From = ScpScaleTransform.ScaleX;
            SwapChainPanelScaleAnimationX.To = ScaleFactor;

            SwapChainPanelScaleAnimationY.From = ScpScaleTransform.ScaleY;
            SwapChainPanelScaleAnimationY.To = ScaleFactor;

            SwapChainPanelStoryboard.Begin();
        }

        public void ZoomOut()
        {
            double targetScaleFactor = 1.0;


            SwapChainPanelScaleAnimationX.From = ScpScaleTransform.ScaleX;
            SwapChainPanelScaleAnimationX.To = targetScaleFactor;

            SwapChainPanelScaleAnimationY.From = ScpScaleTransform.ScaleY;
            SwapChainPanelScaleAnimationY.To = targetScaleFactor;


            //
            // Translate (through animation) the swap chain panel to fit the screen
            //
            //SwapChainPanelTranslateAnimationX.To = CoreWindow.GetForCurrentThread().Bounds.X;
            //SwapChainPanelTranslateAnimationY.To = CoreWindow.GetForCurrentThread().Bounds.Y;

            SwapChainPanelStoryboard.Begin();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Contract.Assert(null != this.ConnectCommand);

            RdpConnectionFactory factory = new RdpConnectionFactory() { SwapChainPanel = this.SwapChainPanel };

            this.ConnectCommand.Execute(new SessionModel(factory));
        }

        private static void OnConnectCommandPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
