using RdClient.Navigation;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    
    /// <summary>
    /// view model to support integrated tests.
    ///      * these tests should be removed from shiped product.
    /// </summary>
    public sealed class TestsViewModel : ViewModelBase, IApplicationBarItemsSource
    {
        private readonly IEnumerable<BarItemModel> _barItems;

        public ICommand StressTestCommand { get; private set; }
        public ICommand GoHomeCommand { get; private set; }

        public IRdpConnectionFactory RdpConnectionFactory { private get; set; }

        private ConnectionInformation _connectionInformation;

        public TestsViewModel()
        {
            StressTestCommand = new RelayCommand(new Action<object>(StressTest));
            GoHomeCommand = new RelayCommand(new Action<object>(GoHome));

            _barItems = new List<BarItemModel>()
            {
                new SegoeGlyphBarButtonModel(SegoeGlyph.Back, new RelayCommand(o => Debug.WriteLine("Plop")), "Plop"),
                new SegoeGlyphBarButtonModel(SegoeGlyph.Forward, new RelayCommand(o => Debug.WriteLine("Flop")), "Flop"),
                new SeparatorBarItemModel(),
                new SegoeGlyphBarButtonModel(SegoeGlyph.Trash, new RelayCommand(o => Debug.WriteLine("Delete")), "Delete")
            };
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
            AutoResetEvent are = new AutoResetEvent(false);
            SessionModel sm = new SessionModel(RdpConnectionFactory);
            IRdpConnection rdpConnection = null;

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

            sm.ConnectionCreated += (sender, args) =>
            {
                rdpConnection = args.RdpConnection;
                rdpConnection.Events.FirstGraphicsUpdate += connectHandler;
                rdpConnection.Events.ClientDisconnected += disconnectHandler;
            };

            for(i = 0; i < iterations; i++)
            {
                sm.Connect(_connectionInformation);
                
                are.WaitOne();

                sm.Disconnect();
                
                are.WaitOne();
            }
        }

        void GoHome(object o)
        {
            NavigationService.NavigateToView("view1", null);
        }

        IEnumerable<BarItemModel> IApplicationBarItemsSource.GetItems(IApplicationBarSite applicationBarSite)
        {
            return _barItems;
        }

        private class BarItem : BarButtonModel
        {
            public BarItem(ICommand command, string label) : base(command, label)
            {
            }
        }
    }
}
