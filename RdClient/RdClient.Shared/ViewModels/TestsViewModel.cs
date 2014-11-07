using RdClient.Navigation;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    
    /// <summary>
    /// view model to support integrated tests.
    ///      * these tests should be removed from shiped product.
    /// </summary>
    public class TestsViewModel : ViewModelBase, ITestsViewModel
    {
        public ICommand StressTestCommand { get; private set; }
        public ICommand GoHomeCommand { get; private set; }

        public IRdpConnectionFactory RdpConnectionFactory { private get; set; }

        private ConnectionInformation _connectionInformation;

        public TestsViewModel()
        {
            StressTestCommand = new RelayCommand(new Action<object>(StressTest));
            GoHomeCommand = new RelayCommand(new Action<object>(GoHome));
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Requires(null != activationParameter as ConnectionInformation);
            _connectionInformation = activationParameter as ConnectionInformation;
        }

        void StressTest(object o)
        {
            int iterations = 3;
            int i;
            SessionViewModel svm = new SessionViewModel();
            AutoResetEvent are = new AutoResetEvent(false);

            EventHandler<ClientDisconnectedArgs> disconnectHandler = (s, a) => {
                if (a.DisconnectReason.Code != RdpDisconnectCode.UserInitiated)
                {
                    throw new Exception("unexpected disconnect");
                }
                else
                {
                    are.Set();
                }
            };
            EventHandler<FirstGraphicsUpdateArgs> connectHandler = (s, a) => { are.Set(); };

            svm.RdpConnectionFactory = RdpConnectionFactory;           

            for(i = 0; i < iterations; i++)
            {
                IRdpConnection rdpConnection = null;

                svm.ConnectionCreated += (sender, args) => {
                    rdpConnection = args.RdpConnection;
                    rdpConnection.Events.FirstGraphicsUpdate += connectHandler;
                    rdpConnection.Events.ClientDisconnected += disconnectHandler;
                };

                svm.ConnectCommand.Execute(_connectionInformation);
                
                are.WaitOne();

                svm.DisconnectCommand.Execute(null);
                
                are.WaitOne();
            }
        }

        void GoHome(object o)
        {
            NavigationService.NavigateToView("view1", null);
        }
    }
}
