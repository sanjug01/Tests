// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Views
{
    using RdClient.CxWrappers.Utils;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.ViewModels;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
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

        public RdSessionPanel()
        {
            this.InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Contract.Assert(null != this.ConnectCommand);

            RdpConnectionFactory factory = new RdpConnectionFactory() { SwapChainPanel = this.SwapChainPanel };

            this.ConnectCommand.Execute(new SessionModel(factory, new WinrtThreadPoolTimerFactory()));
        }

        private static void OnConnectCommandPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
