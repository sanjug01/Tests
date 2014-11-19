﻿using RdClient.Shared.Navigation;
using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System.Diagnostics.Contracts;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    public sealed partial class View1 : Page, IPresentableView
    {
        private INavigationService _navigationService;

        public IViewModel ViewModel { get { return null; } }

        public View1()
        {
            this.InitializeComponent();
        }

        public void Activating(object activationParameter)
        {
            
        }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            Contract.Requires(navigationService != null);

            _navigationService = navigationService;
        }

        public void Dismissing()
        {

        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ConnectionInformation connectionInformation = new ConnectionInformation()
            {
                Desktop = new Desktop() { HostName = "a3-w81" },
                Credentials = new Credentials() { Username = "tslabadminx", Domain = "", Password = "1234AbCd", HaveBeenPersisted = false }
            };

            _navigationService.NavigateToView("SessionView", connectionInformation);
        }

        private void TestsButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ConnectionInformation connectionInformation = new ConnectionInformation()
            {
                Desktop = new Desktop() { HostName = "a3-w81" },
                Credentials = new Credentials() { Username = "tslabadmin", Domain = "", Password = "1234AbCd", HaveBeenPersisted = false }
            };

            _navigationService.NavigateToView("TestsView", connectionInformation);
        }

        private void AddDesktopButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // new dektop, static user
            AddOrEditDesktopViewModelArgs args = new AddOrEditDesktopViewModelArgs(
                null,
                new Credentials() { Username = "tslabadmin", Domain = "", Password = "1234AbCd", HaveBeenPersisted = false },
                true);

            _navigationService.PushModalView("AddOrEditDesktopView", args);
        }

        private void EditDesktopButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // edit existing desktop, static user
            AddOrEditDesktopViewModelArgs args = new AddOrEditDesktopViewModelArgs(
                new Desktop() { HostName = "a3-w81" },
                new Credentials() { Username = "tslabadmin", Domain = "", Password = "1234AbCd", HaveBeenPersisted = false },
                false);

            _navigationService.PushModalView("AddOrEditDesktopView", args);
        }
    }
}
