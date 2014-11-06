using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
namespace RdClient.Navigation
{
    public class NavigationServiceException : Exception
    {
        public NavigationServiceException(string msg) : base(msg)
        {
        }
    }

    public sealed class NavigationService : INavigationService
    {
        public event EventHandler PushingFirstModalView;
        public event EventHandler DismissingLastModalView;

        private IViewPresenter _presenter;
        private IPresentableViewFactory _viewFactory;
        private IPresentableView _currentView;

        private List<IPresentableView> _modalStack = new List<IPresentableView>();

        public NavigationService(IViewPresenter presenter, IPresentableViewFactory viewFactory)
        {
            Contract.Requires(presenter != null);
            Contract.Requires(viewFactory != null);

            _presenter = presenter;
            _viewFactory = viewFactory;
        }

        public void NavigateToView(string viewName, object activationParameter)
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
            
            view.Presenting(this, activationParameter);

            if (!object.ReferenceEquals(_currentView, view))
            {
                _presenter.PresentView(view);
            }

            _currentView = view;
        }

        public void PushModalView(string viewName, object activationParameter)
        {
            Contract.Requires(viewName != null);

            IPresentableView view = _viewFactory.CreateView(viewName, activationParameter);

            if (object.ReferenceEquals(view, _currentView) || _modalStack.Contains(view))
            {
                throw new NavigationServiceException("trying to modally display a view which is already shown.");
            }

            if(_modalStack.Count == 0 && PushingFirstModalView != null)
            {
                PushingFirstModalView(this, null);
            }

            _modalStack.Add(view);
            view.Presenting(this, activationParameter);
            _presenter.PushModalView(view);
        }

        public void DismissModalView(IPresentableView modalView)
        {
            Contract.Requires(modalView != null);

            List<IPresentableView> toDismiss = new List<IPresentableView>();
            int toDismissIndex = _modalStack.IndexOf(modalView);

            if (toDismissIndex < 0)
            {
                throw new NavigationServiceException("trying to dismiss a modal view which is not presented");
            }

            toDismiss = _modalStack.GetRange(toDismissIndex, _modalStack.Count - toDismissIndex);

            _modalStack.RemoveRange(toDismissIndex, _modalStack.Count - toDismissIndex);

            toDismiss.Reverse();

            foreach (IPresentableView view in toDismiss)
            {
                view.Dismissing();
                _presenter.DismissModalView(view);
            }

            if(DismissingLastModalView != null)
            {
                DismissingLastModalView(this, null);
            }
        }

    }
}
