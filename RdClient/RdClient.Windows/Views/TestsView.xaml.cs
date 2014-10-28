using RdClient.CxWrappers.Utils;
using RdClient.Helpers;
using RdClient.Navigation;
using RdClient.Shared;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Utils;
using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;

using System;
using System.Diagnostics.Contracts;
using System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    /// <summary>
    /// A view to group several integrated tests, such as:
    ///     * stress connect/disconne
    ///     
    /// Note that this view will be removed from the shipped product
    /// </summary>
    public sealed partial class TestsView : Page, IPresentableView
    {
        private TestsViewModel _testsViewModel;
        private SessionViewModel _sessionViewModel;

        private INavigationService _navigationService;
        private IRdpConnection _connection;
        private IPhysicalScreenSize _screenSize;
        private object _activationParameter;

        private readonly int _iterations = 100;

        public TestsView()
        {
            this.InitializeComponent();

            _testsViewModel = new TestsViewModel();
            _sessionViewModel = new SessionViewModel();

            _screenSize = new PhysicalScreenSize();
            this.SizeChanged += TestsView_SizeChanged;

            _connection = null;
        }


        void TestsView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        public void Activating(object activationParameter)
        {
        }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            Contract.Requires(navigationService != null);
            Contract.Requires(activationParameter != null);

            _navigationService = navigationService;
            _activationParameter = activationParameter;
        }

        public void Dismissing()
        {
        }


        /// <summary>
        ///     Tapping this button runs Connect/Disconnect stress test (100 times)
        ///         by re-using the same connection object
        /// </summary>
        /// <param name="sender"> sender </param>
        /// <param name="e"> event args</param>
        private void StressConnectButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Contract.Assert(_activationParameter != null);

            Tuple<Desktop, Credentials> connectionInformation = _activationParameter as Tuple<Desktop, Credentials>;
            Desktop desktop = connectionInformation.Item1;
            Credentials credentials = connectionInformation.Item2;

            _connection = RdpConnectionFactory.CreateInstance(CoreWindow.GetForCurrentThread(), this.TestSwapChainPanel, _sessionViewModel);
            RdpPropertyApplier.ApplyDesktop(_connection as IRdpProperties, desktop);
            RdpPropertyApplier.ApplyScreenSize(_connection as IRdpProperties, _screenSize);

            for (int i = 0; i < _iterations; i++ )
            {
                // Connect
                _connection.Connect(credentials, false);
               
                // and Disconnect
                _connection.Disconnect();
            }

            _connection = null;

        }

        private void FinishTestsButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            // return to main view
            _navigationService.NavigateToView("view1", null);
        }

        /// <summary>
        ///     Tapping this button runs Connect/Disconnect stress test (100 times)
        ///         by building a new connection object for each connection
        /// </summary>
        /// <param name="sender"> sender </param>
        /// <param name="e"> event args</param>
        private void StressConnect2Button_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Contract.Assert(_activationParameter != null);

            Tuple<Desktop, Credentials> connectionInformation = _activationParameter as Tuple<Desktop, Credentials>;
            Desktop desktop = connectionInformation.Item1;
            Credentials credentials = connectionInformation.Item2;

            for (int i = 0; i < _iterations; i++)
            {
                _connection = RdpConnectionFactory.CreateInstance(CoreWindow.GetForCurrentThread(), this.TestSwapChainPanel, _sessionViewModel);
                RdpPropertyApplier.ApplyDesktop(_connection as IRdpProperties, desktop);
                RdpPropertyApplier.ApplyScreenSize(_connection as IRdpProperties, _screenSize);
                // Connect
                _connection.Connect(credentials, false);

                // and Disconnect
                _connection.Disconnect();

                _connection = null;
            }


        }

        /// <summary>
        ///     Tapping this button runs single Disconnect test 
        ///         by reusing connection created in Connect test
        /// </summary>
        /// <param name="sender"> sender </param>
        /// <param name="e"> event args</param>
        private void DisconnectButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if(null!= _connection)
            {
                _connection.Disconnect();
                _connection = null;
            }
        }

        /// <summary>
        ///     Tapping this button runs single Connect test 
        ///         it will create and save a new connection object
        /// </summary>
        /// <param name="sender"> sender </param>
        /// <param name="e"> event args</param>
        private void ConnectButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Contract.Assert(_activationParameter != null);

            Tuple<Desktop, Credentials> connectionInformation = _activationParameter as Tuple<Desktop, Credentials>;
            Desktop desktop = connectionInformation.Item1;
            Credentials credentials = connectionInformation.Item2;

            _connection = RdpConnectionFactory.CreateInstance(CoreWindow.GetForCurrentThread(), this.TestSwapChainPanel, _sessionViewModel);
            RdpPropertyApplier.ApplyDesktop(_connection as IRdpProperties, desktop);
            RdpPropertyApplier.ApplyScreenSize(_connection as IRdpProperties, _screenSize);

            _connection.Connect(credentials, false);
        }

    }
}
