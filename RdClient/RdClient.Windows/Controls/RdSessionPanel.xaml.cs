// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Controls
{
    using RdClient.CxWrappers.Utils;
    using RdClient.Shared.Models;
    using RdClient.Shared.Input.ZoomPan;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Core;
    using Windows.UI.Xaml.Input;

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
            typeof(RdSessionPanel), 
            new PropertyMetadata(true));
        public Size ViewSize
        {
            private get { return (Size)GetValue(ViewSizeProperty); }
            set { SetValue(ViewSizeProperty, value); }
        }


        public static DependencyProperty ScaleFactorProperty = DependencyProperty.Register(
            "ScaleFactor", typeof(double), 
            typeof(RdSessionPanel), 
            new PropertyMetadata(1.0)
            );

        public double ScaleFactor
        {
            get { return (double)GetValue(ScaleFactorProperty); }
            set { SetValue(ScaleFactorProperty, value); }
        }

       static readonly DependencyProperty ZoomUpdateProperty = DependencyProperty.Register(
           "ZoomUpdate",
            typeof(object),
            typeof(RdSessionPanel), 
            new PropertyMetadata(null, OnZoomUpdateChanged)
            );

        public IZoomUpdate ZoomUpdate
        {
            get { return (IZoomUpdate)GetValue(ZoomUpdateProperty); }
            set { SetValue(ZoomUpdateProperty, value); }
        }

        private static void OnZoomUpdateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Contract.Assert(sender != null);
            Contract.Assert(e != null);
            RdSessionPanel panel = sender as RdSessionPanel;
            IZoomUpdate zoomUpdate = e.NewValue as IZoomUpdate;

            if (zoomUpdate.ZoomType == ZoomUpdateType.ZoomIn)
            {
                panel.ZoomIn();
            }
            else if (zoomUpdate.ZoomType == ZoomUpdateType.ZoomOut)
            {
                panel.ZoomOut();
            }
            else
            {
                // should be a custom zoom
                ICustomZoomUpdate customZoomUpdate = zoomUpdate as ICustomZoomUpdate;
            }

        }

        static readonly DependencyProperty PanUpdateProperty = DependencyProperty.Register(
            "PanUpdate",
             typeof(object),
             typeof(RdSessionPanel),
             new PropertyMetadata(null, OnPanUpdateChanged)
             );

        public IPanUpdate PanUpdate
        {
            get { return (IPanUpdate)GetValue(PanUpdateProperty); }
            set { SetValue(PanUpdateProperty, value); }
        }

        private static void OnPanUpdateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Contract.Assert(sender != null);
            Contract.Assert(e != null);
            RdSessionPanel panel = sender as RdSessionPanel;
            IPanUpdate panUpdate = e.NewValue as IPanUpdate;

            // TODO apply pan
            panel.PanTransform(panUpdate.X, panUpdate.Y);
        }

        public RdSessionPanel()
        {
            this.InitializeComponent();
            this.SizeChanged += (sender, args) => { this.ViewSize = (args as SizeChangedEventArgs).NewSize; };

            ScaleFactor = 1.0;

            SwapChainPanelStoryboard.Completed += SwapChainPanelStoryboard_Completed;
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

        public void ZoomTransform(double centerX, double centerY, double scaleX, double scaleY)
        {
            SwapChainPanelScaleAnimationX.From = ScpScaleTransform.ScaleX;
            SwapChainPanelScaleAnimationX.To = scaleX;

            SwapChainPanelScaleAnimationY.From = ScpScaleTransform.ScaleY;
            SwapChainPanelScaleAnimationY.To = scaleY;

            // TODO: manage the center

        }

        public void PanTransform(double x, double y)
        {
            SwapChainPanelTranslateAnimationX.From = ScpTranslateTransform.X;
            SwapChainPanelTranslateAnimationX.To = ScpTranslateTransform.X + x;

            SwapChainPanelTranslateAnimationY.From = ScpTranslateTransform.Y;
            SwapChainPanelTranslateAnimationY.To = ScpTranslateTransform.Y + y;
            
            // reset the zoom transformation
            SwapChainPanelScaleAnimationX.From = SwapChainPanelScaleAnimationX.To;
            SwapChainPanelScaleAnimationY.From = SwapChainPanelScaleAnimationY.To;

            SwapChainPanelStoryboard.Begin();
        }

        void SwapChainPanelStoryboard_Completed(object sender, object e)
        {
            ScaleFactor = ScpScaleTransform.ScaleX;

            // TODO : there are more caculation to be done here
            //Rect rectCoreWindow = CoreWindow.GetForCurrentThread().Bounds;
            //Rect rectCoreWindowT = ScpScaleTransform.TransformBounds(rectCoreWindow);

            ////m_maxTranslationOffset.X = rectCoreWindow.Left - rectCoreWindowT.Left;
            ////m_maxTranslationOffset.Y = rectCoreWindow.Top - rectCoreWindowT.Top;
            ////m_minTranslationOffset.X = rectCoreWindow.Right - rectCoreWindowT.Right;
            ////m_minTranslationOffset.Y = rectCoreWindow.Bottom - rectCoreWindowT.Bottom;
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
