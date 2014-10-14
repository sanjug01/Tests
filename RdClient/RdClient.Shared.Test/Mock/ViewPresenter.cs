using FadeTest.Navigation;

namespace Test.RdClient.Shared.Mock
{
    class ViewPresenter : IViewPresenter
    {
        private IPresentableView _presentedView;

        public ViewPresenter()
        {
        }

        public void PresentView(IPresentableView view, INavigationService navigationService, object activationParameter)
        {
            if (!object.ReferenceEquals(view, _presentedView))
            {
                if (null != _presentedView)
                    _presentedView.Dismissing();
            }

            if (null != view)
            {
                view.Activating(navigationService, activationParameter);
                if (!object.ReferenceEquals(view, _presentedView))
                    view.Presenting(null);
            }
            _presentedView = view;
        }

        public void PushModalView(IPresentableView view, INavigationService navigationService, object activationParameter)
        {
        }
    }
}
