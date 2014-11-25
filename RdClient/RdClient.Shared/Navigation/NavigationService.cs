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
        private readonly List<PresentedModalView> _modalStack;
        private NavigationExtensionList _extensions;
        private IViewPresenter _presenter;
        private IPresentableViewFactory _viewFactory;
        private IPresentableView _currentView;

        private sealed class PresentedModalView : IPresentationResult
        {
            private readonly IPresentableView _view;
            private readonly IPresentationCompletion _completion;
            private object _result;

            public IPresentableView View { get { return _view; } }

            public PresentedModalView(IPresentableView view, IPresentationCompletion completion)
            {
                Contract.Requires(null != view);
                Contract.Ensures(null != _view);
                _view = view;
                _completion = completion;
            }

            public bool HasView(IPresentableView view)
            {
                return object.ReferenceEquals(view, _view);
            }

            public void ReportCompletion()
            {
                if (null != _completion)
                    _completion.EmitCompleted(_view, _result);
            }

            void IPresentationResult.SetResult(object result)
            {
                _result = result;
            }
        }

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
            _modalStack = new List<PresentedModalView>();
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

            CallPresenting(view, activationParameter, null);

            if (!object.ReferenceEquals(_currentView, view))
            {
                _presenter.PresentView(view);
            }

            _currentView = view;
        }

        public virtual void PushModalView(string viewName, object activationParameter, IPresentationCompletion presentationCompletion)
        {
            Contract.Requires(viewName != null);

            IPresentableView view = _viewFactory.CreateView(viewName, activationParameter);

            if (object.ReferenceEquals(view, _currentView) || null != _modalStack.Find(pmv => pmv.HasView(view)))
            {
                throw new NavigationServiceException("trying to modally display a view which is already shown.");
            }

            if(_modalStack.Count == 0)
            {
                EmitPushingFirstModalView();
            }

            PresentedModalView presentedView = new PresentedModalView(view, presentationCompletion);
            _modalStack.Add(presentedView);
            CallPresenting(view, activationParameter, presentedView);

            _presenter.PushModalView(view);
        }


        public virtual void DismissModalView(IPresentableView modalView)
        {
            Contract.Requires(modalView != null);

            List<PresentedModalView> toDismiss = new List<PresentedModalView>();

            int toDismissIndex = _modalStack.FindIndex(pmv => pmv.HasView(modalView));

            if (toDismissIndex < 0)
            {
                throw new NavigationServiceException("trying to dismiss a modal view that is not presented");
            }

            toDismiss = _modalStack.GetRange(toDismissIndex, _modalStack.Count - toDismissIndex);
            _modalStack.RemoveRange(toDismissIndex, _modalStack.Count - toDismissIndex);
            toDismiss.Reverse();

            foreach (PresentedModalView presentedView in toDismiss)
            {
                CallDismissing(presentedView.View);
                _presenter.DismissModalView(presentedView.View);
                presentedView.ReportCompletion();
            }
            //
            // The view that has become the new active view - either the currently presented view,
            // of the view at the top of the modal stack.
            //
            IPresentableView newActiveView = null;
            int topModalViewIndex = _modalStack.Count - 1;

            if (topModalViewIndex < 0)
            {
                EmitDismissingLastModalView();
                newActiveView = _currentView;
            }
            else
            {
                newActiveView = _modalStack[topModalViewIndex].View;
            }

            if( null != newActiveView )
            {
                newActiveView.ViewModel.CastAndCall<IViewModel>(vm =>
                {
                    foreach (INavigationExtension extension in this.Extensions)
                        extension.Presenting(vm);
                });
            }
        }

        private void CallPresenting(IPresentableView view, object activationParameter, IPresentationResult presentationResult)
        {
            view.ViewModel.CastAndCall<IViewModel>( vm =>
            {
                foreach (INavigationExtension extension in Extensions)
                {
                    extension.Presenting(vm);
                }

                vm.Presenting(this, activationParameter, presentationResult);
            });

            view.Presenting(this, activationParameter);
        }


        private void CallDismissing(IPresentableView view)
        {
            view.ViewModel.CastAndCall<IViewModel>(vm =>
            {
                vm.Dismissing();

                foreach (INavigationExtension extension in Extensions)
                {
                    extension.Dismissed(vm);
                }
            });

            view.Dismissing();
        }
    }
}
