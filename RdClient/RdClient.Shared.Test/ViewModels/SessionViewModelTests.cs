﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class SessionViewModelTests
    {
        [TestMethod]
        public void SessionViewModel_ShouldConnect()
        {
            using(Mock.NavigationService navigation = new Mock.NavigationService())
            using(Mock.SessionModel sessionModel = new Mock.SessionModel())
            {
                SessionViewModel svm = new SessionViewModel();
                svm.SessionModel = sessionModel;

                ConnectionInformation connectionInformation = new ConnectionInformation()
                {
                    Desktop = new Desktop() { hostName = "narf" },
                    Credentials = new Credentials() { username = "don pedro", domain = "Spain", password = "Chorizo" }
                };
                
                sessionModel.Expect("Connect", new List<object>() { connectionInformation }, 0);

                svm.Presenting(navigation, connectionInformation);
                svm.ConnectCommand.Execute(null);
            }
        }

        [TestMethod]
        public void SessionViewModel_ShouldDisconnect_ShouldNavigateHome()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            {
                SessionViewModel svm = new SessionViewModel();
                svm.SessionModel = sessionModel;

                ConnectionInformation connectionInformation = new ConnectionInformation()
                {
                    Desktop = new Desktop() { hostName = "narf" },
                    Credentials = new Credentials() { username = "don pedro", domain = "Spain", password = "Chorizo" }
                };

                sessionModel.Expect("Disconnect", new List<object>() { }, 0);
                navigation.Expect("NavigateToView", new List<object>() { "view1", null }, 0);

                svm.Presenting(navigation, connectionInformation);
                svm.DisconnectCommand.Execute(null);
            }
        }
    }
}
