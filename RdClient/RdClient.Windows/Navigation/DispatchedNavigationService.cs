using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace RdClient.Shared.Navigation
{
    public class DispatchedNavigationService : NavigationService
    {
        private readonly CoreDispatcher _dispatcher;

        public DispatchedNavigationService()
        {
            _dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
        }

        public override async void NavigateToView(string viewName, object activationParameter)
        {                        
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                base.NavigateToView(viewName, activationParameter);
            });
        }

        public override async void PushModalView(string viewName, object activationParameter)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                base.PushModalView(viewName, activationParameter);
            });
        }

        public override async void DismissModalView(IPresentableView modalView)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                base.DismissModalView(modalView);
            });
        }
    }
}
