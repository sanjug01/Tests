// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Controls
{
    using RdClient.CxWrappers.Utils;
    using RdClient.Helpers;
    using RdClient.Shared.Input.ZoomPan;
    using RdClient.Shared.Models;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using Windows.Foundation;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class RdSessionPanel : UserControl
    {
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

        static readonly DependencyProperty WindowRectProperty = DependencyProperty.Register(
            "WindowRect",
             typeof(Rect),
             typeof(RdSessionPanel),
             new PropertyMetadata(true)
             );
        public Rect WindowRect
        {
            get { return (Rect)GetValue(WindowRectProperty); }
            set { SetValue(WindowRectProperty, value); }
        }

        static readonly DependencyProperty TransformRectProperty = DependencyProperty.Register(
            "TransformRect",
            typeof(Rect),
            typeof(RdSessionPanel),
            new PropertyMetadata(true)
            );

        public Rect TransformRect
        {
            get { return (Rect)GetValue(TransformRectProperty); }
            set { SetValue(TransformRectProperty, value); }
        }

        static readonly DependencyProperty ZoomPanTransformProperty = DependencyProperty.Register(
           "ZoomPanTransform",
            typeof(object),
            typeof(RdSessionPanel),
            new PropertyMetadata(null, OnZoomPanTransformChanged)
            );

        public IZoomPanTransform ZoomPanTransform
        {
            get { return (IZoomPanTransform)GetValue(ZoomPanTransformProperty); }
            set { SetValue(ZoomPanTransformProperty, value); }
        }

        private static void OnZoomPanTransformChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Contract.Assert(sender != null);
            Contract.Assert(e != null);
            RdSessionPanel panel = sender as RdSessionPanel;
            
            panel.SwapChainPanelStoryboard.Begin();
        }

        public RdSessionPanel()
        {
            this.InitializeComponent();
            this.SizeChanged += (sender, args) => { this.ViewSize = (args as SizeChangedEventArgs).NewSize; };
            SwapChainPanelStoryboard.Completed += SwapChainPanelStoryboard_Completed;
        }

        void SwapChainPanelStoryboard_Completed(object sender, object e)
        {
            this.WindowRect = CoreWindow.GetForCurrentThread().Bounds;
            this.TransformRect = ScpScaleTransform.TransformBounds(this.WindowRect);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Contract.Assert(null != this.ConnectCommand);

            RdpConnectionFactory factory = new RdpConnectionFactory() { SwapChainPanel = this.SwapChainPanel };

            // bind the window rect and transform rect
            this.WindowRect = CoreWindow.GetForCurrentThread().Bounds;
            this.TransformRect = ScpScaleTransform.TransformBounds(this.WindowRect);
            //
            // TODO: properly inject the deferred execution object.
            //
            this.ConnectCommand.Execute(new SessionModel(factory, new CoreDispatcherDeferredExecution()));
        }

        private static void OnConnectCommandPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
