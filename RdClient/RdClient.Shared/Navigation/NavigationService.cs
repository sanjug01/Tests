using RdClient.Shared.Helpers;
using RdClient.Shared.Telemetry;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows.Input;

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
        private readonly ITelemetryClient _telemetryClient;
        private readonly List<PresentedStackedView> _modalStack;
        private readonly List<PresentedStackedView> _accessoryStack;
        private NavigationExtensionList _extensions;
        private IViewPresenter _presenter;
        private IStackedViewPresenter _modalPresenter;
        private IPresentableViewFactory _viewFactory;
        private IPresentableView _currentView;
        private readonly ICommand _backCommand;
        private readonly ICommand _dismissAccessoryViewsCommand;

        public NavigationService(ITelemetryClient telemetryClient = null)
        {
            Contract.Ensures(null != _modalStack);
            Contract.Ensures(null != _accessoryStack);

            _telemetryClient = telemetryClient;
            _modalStack = new List<PresentedStackedView>();
            _accessoryStack = new List<PresentedStackedView>();
            _dismissAccessoryViewsCommand = new RelayCommand(o => DismissAccessoryViewsCommandExecute());
            _backCommand = new RelayCommand(BackCommandExecute);
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

        public IViewPresenter Presenter
        {
            set
            {
                _presenter = value;
                _modalPresenter = _presenter as IStackedViewPresenter;

                if (null != value)
                    Contract.Assert(null != _modalPresenter);
            }
        }

        public IPresentableViewFactory ViewFactory { set { _viewFactory = value; } }

        public ICommand BackCommand { get { return _backCommand; } }

        public ICommand DismissAccessoryViewsCommand { get { return _dismissAccessoryViewsCommand; } }

        public void EmitDismissingLastModalView()
        {
            if (DismissingLastModalView != null)
            {
                DismissingLastModalView(this, EventArgs.Empty);
            }
        }

        public virtual void NavigateToView(string viewName, object activationParameter)
        {
            if (null != _telemetryClient)
            {
                ITelemetryEvent telemetryEvent = _telemetryClient.MakeEvent("NavigateToView");
                telemetryEvent.AddTag("view", viewName);
                telemetryEvent.Report();
            }

            IPresentableView view = _viewFactory.CreateView(viewName, activationParameter);

            if (view == null)
            {
                throw new NavigationServiceException("Tried to create unknown view: " + viewName);
            }

            if (_currentView != null && !object.ReferenceEquals(_currentView, view))
            {
                DismissAllAccessoryViews();
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
            if (null != _telemetryClient)
            {
                ITelemetryEvent telemetryEvent = _telemetryClient.MakeEvent("PushModalView");
                telemetryEvent.AddTag("view", viewName);
                telemetryEvent.Report();
            }

            Contract.Requires(viewName != null);

            IPresentableView view = _viewFactory.CreateView(viewName, activationParameter);

            if (object.ReferenceEquals(view, _currentView)
                || null != _modalStack.Find(pmv1 => pmv1.HasView(view)
                || null != _accessoryStack.Find(pmv2 => pmv2.HasView(view))))
            {
                throw new NavigationServiceException("trying to modally display a view which is already shown.");
            }

            if(_modalStack.Count == 0)
            {
                EmitPushingFirstModalView();
            }
            else
            {
                //
                // Tell the top view on the stack that is is no longer the top view.
                //
                _modalStack[_modalStack.Count - 1].View.CastAndCall<IStackedView>(sv => sv.Deactivate());
            }

            PresentedStackedView presentedView = new PresentedModalView(this, view, presentationCompletion, _telemetryClient);
            _modalStack.Add(presentedView);
            CallPresenting(view, activationParameter, presentedView);

            _modalPresenter.PushView(view, true);
            //
            // Tell the view that it is now the top of the stack.
            //
            view.CastAndCall<IStackedView>(sv => sv.Activate());
        }

        public virtual void DismissModalView(IPresentableView modalView)
        {
            DismissStackedView(_modalStack, modalView, _modalPresenter);

            if (0 == _modalStack.Count)
            {
                EmitDismissingLastModalView();
            }
        }

        void INavigationService.PushAccessoryView(string viewName, object activationParameter, IPresentationCompletion presentationCompletion)
        {
            if (null != _telemetryClient)
            {
                ITelemetryEvent telemetryEvent = _telemetryClient.MakeEvent("PushAccessoryView");
                telemetryEvent.AddTag("view", viewName);
                telemetryEvent.Report();
            }

            IStackedViewPresenter accessoryPresenter = _currentView as IStackedViewPresenter;

            if(null == accessoryPresenter)
            {
                this.PushModalView(viewName, activationParameter, null);
            }
            else
            {
                //
                // Push the view onto the accessory stack and show in in the accessory presenter.
                //
                Contract.Requires(viewName != null);

                IPresentableView view = _viewFactory.CreateView(viewName, activationParameter);

                if (object.ReferenceEquals(view, _currentView)
                    || null != _modalStack.Find(pmv => pmv.HasView(view))
                    || null != _accessoryStack.Find(pmv => pmv.HasView(view)))
                {
                    throw new NavigationServiceException("Trying to present an already presented accessory view.");
                }

                if (0 != _accessoryStack.Count)
                {
                    //
                    // Tell the top view on the stack that is is no longer the top view.
                    //
                    _accessoryStack[_accessoryStack.Count - 1].View.CastAndCall<IStackedView>(sv => sv.Deactivate());
                }

                PresentedStackedView presentedView = new PresentedAccessoryView(this, view, presentationCompletion, _telemetryClient);
                _accessoryStack.Add(presentedView);
                CallPresenting(view, activationParameter, presentedView);

                accessoryPresenter.PushView(view, true);
                //
                // Tell the view that it is now the top of the stack.
                //
                view.CastAndCall<IStackedView>(sv => sv.Activate());
            }
        }
        void INavigationService.DismissAccessoryView(IPresentableView accessoryView)
        {
            IStackedViewPresenter accessoryPresenter = _currentView as IStackedViewPresenter;

            if (null == accessoryPresenter)
            {
                this.DismissModalView(accessoryView);
            }
            else
            {
                DismissStackedView(_accessoryStack, accessoryView, accessoryPresenter);
            }
        }

        private void DismissStackedView(IList<PresentedStackedView> viewStack, IPresentableView stackedView, IStackedViewPresenter presenter)
        {
            Contract.Assert(stackedView != null);

            bool presented = false, dismissed = false;
            //
            // Check if the view is on the stack
            //
            foreach (PresentedStackedView psv in viewStack)
            {
                if (psv.HasView(stackedView))
                {
                    presented = true;
                    break;
                }
            }

            if(!presented)
                throw new NavigationServiceException("Dismissing a stacked view that is not presented");
            //
            // From the top of the stack dismiss all the views down to the requested one.
            //
            int index = viewStack.Count;
            IList<PresentedStackedView> reportCompletion = new List<PresentedStackedView>();

            do
            {
                PresentedStackedView psv = viewStack[--index];

                if(psv.HasView(stackedView))
                    dismissed = true;
                //
                // Tell the view that it is not at the top of the stack anymore.
                //
                psv.View.CastAndCall<IStackedView>(sv => sv.Deactivate());
                //
                // Properly dismiss the view.
                //
                CallDismissing(psv.View);
                presenter.DismissView(psv.View, true);
                reportCompletion.Add(psv);
                //
                // Remove the view from the list that represents the stack.
                //
                viewStack.RemoveAt(index);
            } while (!dismissed);

            Contract.Assert(dismissed);
            IPresentableView newActiveView = null;

            if (0 == viewStack.Count)
            {
                //
                // The last view from the stack has been dismissed, the new active view is the currently
                // presented main view.
                //
                Contract.Assert(0 == index);
                newActiveView = _currentView;
            }
            else
            {
                Contract.Assert(index > 0);
                newActiveView = viewStack[index-1].View;
                //
                // Tell the view that it is now the top of the stack.
                //
                newActiveView.CastAndCall<IStackedView>(sv => sv.Activate());
            }

            if (null != newActiveView)
            {
                newActiveView.ViewModel.CastAndCall<IViewModel>(vm =>
                {
                    foreach (INavigationExtension extension in this.Extensions)
                        extension.Presenting(vm);
                });
            }

            foreach (PresentedStackedView psv in reportCompletion)
                psv.ReportCompletion();
        }

        private void BackCommandExecute(object param)
        {
            Contract.Assert(param is IBackCommandArgs || param == null);

            IBackCommandArgs args = param as IBackCommandArgs ?? new BackCommandArgs();

            if (args.Handled == false)
            {
                if (_modalStack.Count != 0)
                {
                    TelemetryEvent("NavigateBack:ModalView");

                    IPresentableView topModalView = _modalStack[_modalStack.Count - 1].View;

                    topModalView.ViewModel.NavigatingBack(args);
                    if (!args.Handled)
                    {
                        DismissModalView(topModalView);
                        args.Handled = true;
                    }
                }
                else if (_accessoryStack.Count != 0)
                {
                    TelemetryEvent("NavigateBack:AccessoryView");

                    IPresentableView topAccessoryView = _accessoryStack[_accessoryStack.Count - 1].View;

                    topAccessoryView.ViewModel.NavigatingBack(args);

                    if (!args.Handled)
                    {
                        Contract.Assert(_currentView is IStackedViewPresenter);
                        IStackedViewPresenter accessoryPresenter = (IStackedViewPresenter)_currentView;

                        DismissStackedView(_accessoryStack, topAccessoryView, accessoryPresenter);
                        args.Handled = true;
                    }
                }
                else
                {
                    TelemetryEvent("NavigateBack:PresentedView");
                    _currentView.ViewModel.NavigatingBack(args);
                }
            }
        }

        private void EmitPushingFirstModalView()
        {
            if (PushingFirstModalView != null)
            {
                PushingFirstModalView(this, EventArgs.Empty);
            }
        }

        private void CallPresenting(IPresentableView view, object activationParameter, IStackedPresentationContext presentationResult)
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

        private void DismissAllAccessoryViews()
        {
            var presenter = _currentView as IStackedViewPresenter;
            var stack = _accessoryStack;

            if (presenter == null) //must be using the modal stack instead
            {
                presenter = _modalPresenter;
                stack = _modalStack;
            }

            // Dismiss the botton of the stack will dismiss the whole stack
            //    double de-activation is avoided per fix for bug# 2537012
            if (stack.Count > 0)
            {
                DismissStackedView(stack, stack[0].View, presenter);
            }
        }

        private void CallDismissing(IPresentableView view)
        {
            view.ViewModel.CastAndCall<IViewModel>(vm =>
            {
                vm.Dismissing();

                if(null != _extensions)
                {
                    foreach (INavigationExtension extension in _extensions)
                        extension.Dismissed(vm);
                }
            });

            view.Dismissing();
        }

        private void DismissAccessoryViewsCommandExecute()
        {
            this.DismissAllAccessoryViews();
        }

        private void TelemetryEvent(string eventName)
        {
            if (null != _telemetryClient)
                _telemetryClient.Event(eventName);
        }

        private abstract class PresentedStackedView : IStackedPresentationContext
        {
            private readonly INavigationService _navigationService;
            private readonly IPresentableView _view;
            private readonly IPresentationCompletion _completion;
            private readonly ITelemetryStopwatch _presentationDuration;
            private object _result;

            public IPresentableView View { get { return _view; } }

            protected PresentedStackedView(INavigationService navigationService, IPresentableView view, IPresentationCompletion completion,
                ITelemetryClient telemetryClient)
            {
                Contract.Requires(null != navigationService);
                Contract.Requires(null != view);
                Contract.Ensures(null != _navigationService);
                Contract.Ensures(null != _view);
                _navigationService = navigationService;
                _view = view;
                _completion = completion;
                if (null != telemetryClient)
                    _presentationDuration = telemetryClient.StartStopwatch();
           }

            public bool HasView(IPresentableView view)
            {
                return object.ReferenceEquals(view, _view);
            }

            public void ReportCompletion()
            {
                if(null != _presentationDuration)
                    _presentationDuration.Stop(string.Format("StackedViewPresentationTime:{0}", _view.GetType().Name));

                if (null != _completion)
                    _completion.Completed(_view, _result);
            }

            protected abstract void DismissView(IPresentableView view, INavigationService navigationService);

            void IStackedPresentationContext.Dismiss(object result)
            {
                _result = result;
                this.DismissView(_view, _navigationService);
            }
        }

        private sealed class PresentedModalView : PresentedStackedView
        {
            public PresentedModalView(INavigationService navigationService, IPresentableView view, IPresentationCompletion completion,
                ITelemetryClient telemetryClient)
                : base(navigationService, view, completion, telemetryClient)
            {
            }

            protected override void DismissView(IPresentableView view, INavigationService navigationService)
            {
                navigationService.DismissModalView(view);
            }
        }

        private sealed class PresentedAccessoryView : PresentedStackedView
        {
            public PresentedAccessoryView(INavigationService navigationService, IPresentableView view, IPresentationCompletion completion,
                ITelemetryClient telemetryClient)
                : base(navigationService, view, completion, telemetryClient)
            {
            }

            protected override void DismissView(IPresentableView view, INavigationService navigationService)
            {
                navigationService.DismissAccessoryView(view);
            }
        }
    }
}
