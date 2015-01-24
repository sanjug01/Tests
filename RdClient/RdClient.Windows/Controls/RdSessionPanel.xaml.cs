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
        private const double MIN_ZOOM_FACTOR = 1.0;

        private Point m_maxTranslationOffset;
        private Point m_minTranslationOffset;

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
            m_maxTranslationOffset.X = 0.0;
            m_maxTranslationOffset.Y = 0.0;
            m_minTranslationOffset.X = 0.0;
            m_minTranslationOffset.Y = 0.0;

            SwapChainPanelStoryboard.Completed += SwapChainPanelStoryboard_Completed;
        }

        public void ZoomIn()
        {
            double targetScaleFactor = MAX_ZOOM_FACTOR;

            // reset the pan transformation
            SwapChainPanelTranslateAnimationX.From = SwapChainPanelTranslateAnimationX.To;
            SwapChainPanelTranslateAnimationY.From = SwapChainPanelTranslateAnimationY.To;   

            // trick to do partial zooms
            targetScaleFactor = ScpScaleTransform.ScaleX + 0.5;
            if (targetScaleFactor > MAX_ZOOM_FACTOR)
            {
                targetScaleFactor = MAX_ZOOM_FACTOR;
            }

            SwapChainPanelScaleAnimationX.From = ScpScaleTransform.ScaleX;
            SwapChainPanelScaleAnimationX.To = targetScaleFactor;

            SwapChainPanelScaleAnimationY.From = ScpScaleTransform.ScaleY;
            SwapChainPanelScaleAnimationY.To = targetScaleFactor;

            Rect rectCoreWindow = CoreWindow.GetForCurrentThread().Bounds;
            ScpScaleTransform.CenterX = (rectCoreWindow.Right - rectCoreWindow.Left) / 2;
            ScpScaleTransform.CenterY = (rectCoreWindow.Bottom - rectCoreWindow.Top) / 2;

            SwapChainPanelStoryboard.Begin();
        }

        public void ZoomOut()
        {
            double targetScaleFactor = MIN_ZOOM_FACTOR;

            // reset the pan transformation
            SwapChainPanelTranslateAnimationX.From = SwapChainPanelTranslateAnimationX.To;
            SwapChainPanelTranslateAnimationY.From = SwapChainPanelTranslateAnimationY.To;            

            // trick to do partial zooms
            targetScaleFactor = ScpScaleTransform.ScaleX - 0.5;
            if (targetScaleFactor < MIN_ZOOM_FACTOR)
            {
                targetScaleFactor = MIN_ZOOM_FACTOR;
            }

            SwapChainPanelScaleAnimationX.From = ScpScaleTransform.ScaleX;
            SwapChainPanelScaleAnimationX.To = targetScaleFactor;

            SwapChainPanelScaleAnimationY.From = ScpScaleTransform.ScaleY;
            SwapChainPanelScaleAnimationY.To = targetScaleFactor;

            //
            // Translate (through animation) the swap chain panel to fit the screen
            //
            Rect rectCoreWindow = CoreWindow.GetForCurrentThread().Bounds;
            SwapChainPanelTranslateAnimationX.To = rectCoreWindow.X;
            SwapChainPanelTranslateAnimationY.To = rectCoreWindow.Y;

            SwapChainPanelStoryboard.Begin();
        }

        public void ZoomTransform(double centerX, double centerY, double scaleX, double scaleY)
        {
            // reset the pan transformation
            SwapChainPanelTranslateAnimationX.From = SwapChainPanelTranslateAnimationX.To;
            SwapChainPanelTranslateAnimationY.From = SwapChainPanelTranslateAnimationY.To;   

            SwapChainPanelScaleAnimationX.From = ScpScaleTransform.ScaleX;
            SwapChainPanelScaleAnimationX.To = scaleX;

            SwapChainPanelScaleAnimationY.From = ScpScaleTransform.ScaleY;
            SwapChainPanelScaleAnimationY.To = scaleY;

            // manage the center
            ScpScaleTransform.CenterX = centerX;
            ScpScaleTransform.CenterY = centerY;
        }

        public void PanTransform(double x, double y)
        {
            double panXTo = ScpTranslateTransform.X + x;
            double panYTo = ScpTranslateTransform.Y + y;

            // reset the zoom transformation
            SwapChainPanelScaleAnimationX.From = SwapChainPanelScaleAnimationX.To;
            SwapChainPanelScaleAnimationY.From = SwapChainPanelScaleAnimationY.To;            

            if (panXTo < m_minTranslationOffset.X)
            {
                panXTo = m_minTranslationOffset.X;
            }
            else if (panXTo > m_maxTranslationOffset.X)
            {
                panXTo = m_maxTranslationOffset.X;
            }

            if (panYTo < m_minTranslationOffset.Y)
            {
                panYTo = m_minTranslationOffset.Y;
            }
            else if (panYTo > m_maxTranslationOffset.Y)
            {
                panYTo = m_maxTranslationOffset.Y;
            }

            SwapChainPanelTranslateAnimationX.From = ScpTranslateTransform.X;
            SwapChainPanelTranslateAnimationX.To = panXTo;
            ScpTranslateTransform.X = panXTo;

            SwapChainPanelTranslateAnimationY.From = ScpTranslateTransform.Y;
            SwapChainPanelTranslateAnimationY.To = panYTo;
            ScpTranslateTransform.Y = panYTo;

            SwapChainPanelStoryboard.Begin();
        }

        void SwapChainPanelStoryboard_Completed(object sender, object e)
        {
            ScaleFactor = ScpScaleTransform.ScaleX;

            // TODO : there are more calculations to be done here
            UpdateTranslationBounds();
        }

        private void UpdateTranslationBounds()
        {
            Rect rectCoreWindow = CoreWindow.GetForCurrentThread().Bounds;
            Rect rectCoreWindowT = ScpScaleTransform.TransformBounds(rectCoreWindow);

            m_maxTranslationOffset.X = rectCoreWindow.Left - rectCoreWindowT.Left;
            m_maxTranslationOffset.Y = rectCoreWindow.Top - rectCoreWindowT.Top;
            m_minTranslationOffset.X = rectCoreWindow.Right - rectCoreWindowT.Right;
            m_minTranslationOffset.Y = rectCoreWindow.Bottom - rectCoreWindowT.Bottom;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Contract.Assert(null != this.ConnectCommand);

            RdpConnectionFactory factory = new RdpConnectionFactory() { SwapChainPanel = this.SwapChainPanel };

            this.ConnectCommand.Execute(new SessionModel(factory));
            UpdateTranslationBounds();
        }

        private static void OnConnectCommandPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
