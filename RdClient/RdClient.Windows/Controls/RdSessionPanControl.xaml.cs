
namespace RdClient.Controls
{
    using RdClient.CxWrappers.Utils;
    using RdClient.Shared.Models;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Core;
    using Windows.UI.Xaml.Input;

    public sealed partial class RdSessionPanControl : UserControl
    {
        public static readonly DependencyProperty ViewSizeProperty = DependencyProperty.Register(
            "ViewSize", typeof(Size),
            typeof(RdSessionPanControl), 
            new PropertyMetadata(true));
        public Size ViewSize
        {
            private get { return (Size)GetValue(ViewSizeProperty); }
            set { SetValue(ViewSizeProperty, value); }
        }

        public static DependencyProperty PanControlForegroundBrushProperty = DependencyProperty.Register(
            "PanControlForegroundBrush", typeof(SolidColorBrush),
            typeof(RdSessionPanControl), 
            new PropertyMetadata((SolidColorBrush)Application.Current.Resources["rdBlackBrush"])
            );

        public SolidColorBrush PanControlForegroundBrush
        {
            get { return (SolidColorBrush)GetValue(PanControlForegroundBrushProperty); }
            set { SetValue(PanControlForegroundBrushProperty, value); }
        }

        public static DependencyProperty PanControlBackgroundBrushProperty = DependencyProperty.Register(
            "PanControlBackgroundBrush", typeof(SolidColorBrush),
            typeof(RdSessionPanControl), 
            new PropertyMetadata((SolidColorBrush)Application.Current.Resources["rdWhiteBrush"])
            );

        public SolidColorBrush PanControlBackgroundBrush
        {
            get { return (SolidColorBrush)GetValue(PanControlBackgroundBrushProperty); }
            set { SetValue(PanControlBackgroundBrushProperty, value); }
        }

        public static DependencyProperty PanControlOrbOpacityProperty = DependencyProperty.Register(
            "PanControlOrbOpacity", typeof(double),
            typeof(RdSessionPanControl), 
            new PropertyMetadata(0.35)
            );

        public double PanControlOrbOpacity
        {
            get { return (double)GetValue(PanControlOrbOpacityProperty); }
            set { SetValue(PanControlOrbOpacityProperty, value); }
        }

        public RdSessionPanControl()
        {
            this.InitializeComponent();
            this.SizeChanged += (sender, args) => { this.ViewSize = (args as SizeChangedEventArgs).NewSize; };

            PanControlForegroundBrush = (SolidColorBrush)Application.Current.Resources["rdWhiteBrush"];
            PanControlBackgroundBrush = (SolidColorBrush)Application.Current.Resources["rdBlackBrush"];
            PanControlOrbOpacity = 1.0;

            PanControl.PointerPressed += PanControl_PointerPressed;
        }

        public void ShowPanControl()
        {
            PanControl.Visibility = Visibility.Visible;
            ShowPanControlAnimation.From = PanControl.Opacity;
            ShowPanControlStoryboard.Begin();
        }

        public void HidePanControl()
        {
            HidePanControlAnimation.From = PanControl.Opacity;
            HidePanControlStoryboard.Begin();
        }

        private void PanControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            //// reverse colors 
            PanControlForegroundBrush = (SolidColorBrush)Application.Current.Resources["rdBlackBrush"];
            PanControlBackgroundBrush = (SolidColorBrush)Application.Current.Resources["rdWhiteBrush"];            
            PanControlOrb.Opacity = 1.0;
            
            // TODO : manage PanControl
        }

        private void PanControl_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //// reset colors
            PanControlForegroundBrush = (SolidColorBrush)Application.Current.Resources["rdWhiteBrush"];
            PanControlBackgroundBrush = (SolidColorBrush)Application.Current.Resources["rdBlackBrush"];            

            // PanControlOrb.Opacity = 1.0;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
