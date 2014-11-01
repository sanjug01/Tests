using RdClient.Navigation;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    
    /// <summary>
    /// view model to support integrated tests.
    ///      * these tests should be removed from shiped product.
    /// </summary>
    public class TestsViewModel : ViewModelBase
    {
        public ICommand StressTestCommand { get; private set; }
        public ICommand GoHomeCommand { get; private set; }
        public Tuple<Desktop, Credentials> ConnectionInformation { private get; set; }

        public IRdpConnectionFactory RdpConnectionFactory { private get; set; }
        public INavigationService NavigationService { private get; set; }

        public TestsViewModel()
        {
            StressTestCommand = new RelayCommand(new Action<object>(StressTest));
            GoHomeCommand = new RelayCommand(new Action<object>(GoHome));
        }

        public void StressTest(object o)
        {
            int iterations = 3;
            int i;
            SessionViewModel svm = new SessionViewModel();
            AutoResetEvent are = new AutoResetEvent(false);

            EventHandler<ClientDisconnectedArgs> disconnectHandler = (s, a) => {
                if (a.DisconnectReason.code != RdpDisconnectCode.UserInitiated)
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

                svm.ConnectCommand.Execute(ConnectionInformation);
                
                are.WaitOne();

                svm.DisconnectCommand.Execute(null);
                
                are.WaitOne();
            }
        }

        public void GoHome(object o)
        {
            NavigationService.NavigateToView("view1", null);
        }
    }
}
