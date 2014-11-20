using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace RdClient.Shared.Navigation
{
    public class NavigationServiceException : Exception
    {
        public NavigationServiceException(string msg) : base(msg)
        {
        }
    }

    public class NavigationService : INavigationService
    {
        private readonly List<IPresentableView> _modalStack;
        private NavigationExtensionList _extensions;
        private IViewPresenter _presenter;
        private IPresentableViewFactory _viewFactory;
        private IPresentableView _currentView;

        public event EventHandler PushingFirstModalView;
        public event EventHandler DismissingLastModalView;

        public NavigationExtensionList Extensions
        {
            get
            {
                if (null == _extensions)
                    _extensions = new NavigationExtensionList();
                return _extensions;
            }
            set { _extensions = value; }
        }

        public IViewPresenter Presenter { set { _presenter = value; } }

        public IPresentableViewFactory ViewFactory { set { _viewFactory = value; } }

        public NavigationService()
        {
            Contract.Ensures(null != _modalStack);
            _modalStack = new List<IPresentableView>();
        }

        private void EmitPushingFirstModalView()
        {
            if (PushingFirstModalView != null)
            {
                PushingFirstModalView(this, EventArgs.Empty);
            }
        }

        public void EmitDismissingLastModalView()
        {
            if (DismissingLastModalView != null)
            {
                DismissingLastModalView(this, EventArgs.Empty);
            }
        }

        public virtual void NavigateToView(string viewName, object activationParameter)
        {
            IPresentableView view = _viewFactory.CreateView(viewName, activationParameter);

            if (view == null)
            {
                throw new NavigationServiceException("Tried to create unknown view: " + viewName);
            }

            if (_currentView != null && !object.ReferenceEquals(_currentView, view))
            {
                CallDismissing(_currentView);
            }

            CallPresenting(view, activationParameter);

            if (!object.ReferenceEquals(_currentView, view))
            {
                _presenter.PresentView(view);
            }

            _currentView = view;
        }

        public virtual void PushModalView(string viewName, object activationParameter)
        {
            Contract.Requires(viewName != null);

            IPresentableView view = _viewFactory.CreateView(viewName, activationParameter);

            if (object.ReferenceEquals(view, _currentView) || _modalStack.Contains(view))
            {
                throw new NavigationServiceException("trying to modally display a view which is already shown.");
            }

            if(_modalStack.Count == 0)
            {
                EmitPushingFirstModalView();
            }

            _modalStack.Add(view);
            CallPresenting(view, activationParameter);

            _presenter.PushModalView(view);
        }


        public virtual void DismissModalView(IPresentableView modalView)
        {
            Contract.Requires(modalView != null);

            List<IPresentableView> toDismiss = new List<IPresentableView>();
            int toDismissIndex = _modalStack.IndexOf(modalView);

            if (toDismissIndex < 0)
            {
                throw new NavigationServiceException("trying to dismiss a modal view that is not presented");
            }

            toDismiss = _modalStack.GetRange(toDismissIndex, _modalStack.Count - toDismissIndex);

            _modalStack.RemoveRange(toDismissIndex, _modalStack.Count - toDismissIndex);

            toDismiss.Reverse();

            foreach (IPresentableView view in toDismiss)
            {
                CallDismissing(view);

                _presenter.DismissModalView(view);
            }

            if(_modalStack.Count == 0)
            {
                EmitDismissingLastModalView();
            }
        }

        private void CallPresenting(IPresentableView view, object activationParameter)
        {
            IViewModel vm = view.ViewModel;

            if (null != vm)
            {
                foreach(INavigationExtension extension in Extensions)
                {
                    extension.Presenting(vm);
                }

                vm.Presenting(this, activationParameter);
            }
            view.Presenting(this, activationParameter);
        }


        private void CallDismissing(IPresentableView view)
        {
            IViewModel vm = view.ViewModel;

            if (null != vm)
            {
                vm.Dismissing();
            }
            view.Dismissing();
        }
    }
}
