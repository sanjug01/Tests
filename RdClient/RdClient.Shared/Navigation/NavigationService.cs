using System;
using System.Collections.Generic;
namespace RdClient.Navigation
{
    public class NavigationServiceException : Exception
    {
        private string p;

        public NavigationServiceException(string msg) : base(msg)
        {
        }

    }

    public sealed class NavigationService : INavigationService
    {
        private IViewPresenter _presenter;
        private IPresentableViewFactory _viewFactory;
        private IPresentableView _currentView;

        private List<IPresentableView> modalStack = new List<IPresentableView>();

        public static INavigationService Create(IViewPresenter presenter, IPresentableViewFactory viewFactory)
        {
            return new NavigationService(presenter, viewFactory);
        }

        private NavigationService(IViewPresenter presenter, IPresentableViewFactory viewFactory)
        {
            _presenter = presenter;
            _viewFactory = viewFactory;
        }

        void INavigationService.NavigateToView(string viewName, object activationParameter)
        {
            IPresentableView view = _viewFactory.CreateView(viewName, activationParameter);

            if (view == null)
            {
                throw new NavigationServiceException("Tried to create unknown view: " + viewName);
            }

            if (_currentView != null && !object.ReferenceEquals(_currentView, view))
            {
                _currentView.Dismissing();
            }
            
            _currentView = view;
            _currentView.Presenting(this, activationParameter);
            _presenter.PresentView(view);
        }

        void INavigationService.PushModalView(string viewName, object activationParameter)
        {
            IPresentableView view = _viewFactory.CreateView(viewName, activationParameter);
            modalStack.Add(view);
            view.Presenting(this, activationParameter);
            _presenter.PushModalView(view);
        }

        public void DismissModalView(IPresentableView modalView)
        {
            List<IPresentableView> toDismiss = new List<IPresentableView>();
            bool foundToDismiss = false;

            foreach(IPresentableView view in modalStack)
            {
                if(object.ReferenceEquals(view, modalView))
                {
                    foundToDismiss = true;
                }

                if (foundToDismiss)
                {
                    toDismiss.Add(view);
                }
            }

            if (foundToDismiss == false)
            {
                throw new NavigationServiceException("trying to dismiss a modal view which is not presented");
            }

            modalStack.RemoveRange(modalStack.Count - toDismiss.Count, toDismiss.Count); ;

            foreach (IPresentableView view in toDismiss)
            {
                view.Dismissing();
                _presenter.DismissModalView(view);
            }
        }
    }
}
