namespace FadeTest.Navigation
{
    public sealed class NavigationService : INavigationService
    {
        public static INavigationService Create()
        {
            return new NavigationService();
        }

        private NavigationService()
        {
        }

        void INavigationService.NavigateToView(string viewName, object activationParameter)
        {
        }

        void INavigationService.PushModalView(string viewName, object activationParameter)
        {
        }
    }
}
