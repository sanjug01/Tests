namespace RdClient.Shared.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.UI.Core;
    using Windows.UI.ViewManagement;

    public class BackButtonHandler
    {
        private readonly INavigationService _nav;
        private readonly SystemNavigationManager _navManager;

        public BackButtonHandler(SystemNavigationManager navManager, INavigationService navigationService)
        {
            _nav = navigationService;
            _navManager = navManager;
            _navManager.IsShellChromeBackVisible = true;
            _navManager.BackRequested += BackButtonHandler_BackRequested;           
        }

        private void BackButtonHandler_BackRequested(object sender, BackRequestedEventArgs e)
        {
            IBackCommandArgs backArgs = new BackRequestedEventArgsWrapper(e);
            _nav.BackCommand.Execute(backArgs);
        }
    }
}
