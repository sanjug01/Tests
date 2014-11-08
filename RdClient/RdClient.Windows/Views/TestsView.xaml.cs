﻿using RdClient.CxWrappers.Utils;
using RdClient.Helpers;
using RdClient.Navigation;
using RdClient.Shared;
using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System.Diagnostics.Contracts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    public sealed partial class TestsView : Page, IPresentableView
    {
        public IViewModel ViewModel { get { return this.TestViewModel; } }

        public TestsView()
        {
            this.InitializeComponent();
        }

        public void Activating(object activationParameter)
        {
        }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
        }

        public void Dismissing()
        {
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RdpConnectionFactory factory = new RdpConnectionFactory();
            factory.SwapChainPanel = this.TestSwapChainPanel;

            this.TestViewModel.RdpConnectionFactory = factory;
        }
    }
}
