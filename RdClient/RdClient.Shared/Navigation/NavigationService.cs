using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
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

        private readonly IViewPresenter _presenter;
        private readonly IPresentableViewFactory _viewFactory;
        private readonly IApplicationBarViewModel _appBarViewModel;
        private IPresentableView _currentView;

        private List<IPresentableView> _modalStack = new List<IPresentableView>();

        public NavigationService(IViewPresenter presenter, IPresentableViewFactory viewFactory, IApplicationBarViewModel appBarViewModel)
        {
            Contract.Requires(presenter != null);
            Contract.Requires(viewFactory != null);
            //
            // appBarViewModel one is asserted because in the application the parameter comes from the view model which may
            // accidentally get replaced with one that doesn't implement the interface.
            //
            Contract.Assert(null != appBarViewModel);
            Contract.Ensures(null != _presenter);
            Contract.Ensures(null != _viewFactory);
            Contract.Ensures(null != _appBarViewModel);

            _presenter = presenter;
            _viewFactory = viewFactory;
            _appBarViewModel = appBarViewModel;
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

        public void NavigateToView(string viewName, object activationParameter)
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
            UpdateApplicationBar();
        }

        public void PushModalView(string viewName, object activationParameter)
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
                _appBarViewModel.BarItems = null;
            }

            _modalStack.Add(view);
            CallPresenting(view, activationParameter);

            _presenter.PushModalView(view);
        }


        public void DismissModalView(IPresentableView modalView)
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
                UpdateApplicationBar();
            }
        }

        private void CallPresenting(IPresentableView view, object activationParameter)
        {
            IViewModel vm = view.ViewModel;

            if (null != vm)
            {
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

        private void UpdateApplicationBar()
        {
            //
            // If the new view model is there and implements the IApplicationBarItemsSource interface,
            // create a bar site object for the view, request the collection of application bar items,
            // and upate the application bar with the new 
            //
            IEnumerable<BarItemModel> barItems = QueryApplicationBarItems();

            _appBarViewModel.IsBarSticky = false;
            _appBarViewModel.BarItems = barItems;
        }

        private IEnumerable<BarItemModel> QueryApplicationBarItems()
        {
            IEnumerable<BarItemModel> barItems = null;

            if (null != _currentView)
            {
                IApplicationBarItemsSource itemSource = _currentView.ViewModel as IApplicationBarItemsSource;

                if (null != itemSource)
                {
                    IApplicationBarSite site = ApplicationBarSite.Create(_appBarViewModel,
                        //
                        // Predicate that the site object calls before making any changes to the view model -
                        // check if the currently presented view is the one for that the site was created,
                        // and that there are no modally presented views on the stack.
                        //
                        o => this.IsCurrentView(o) && 0 == _modalStack.Count, _currentView,
                        _appBarViewModel.ShowBar,
                        () => _appBarViewModel.IsBarVisible = false);
                    barItems = itemSource.GetItems(site);
                }
            }

            return barItems;
        }

        private bool IsCurrentView(object obj)
        {
            return object.ReferenceEquals(obj, _currentView);
        }
    }
}
