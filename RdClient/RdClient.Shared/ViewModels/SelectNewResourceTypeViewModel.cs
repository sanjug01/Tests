﻿namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Navigation;
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public sealed class SelectNewResourceTypeViewModel : ViewModelBase
    {
        private readonly RelayCommand _addDesktop;
        private readonly RelayCommand _addOnPremiseWorkspace;
        private readonly RelayCommand _addCloudWorkspace;

        public ICommand AddDesktop
        {
            get { return _addDesktop; }
        }

        public ICommand AddOnPremiseWorkspace
        {
            get { return _addOnPremiseWorkspace; }
        }

        public ICommand AddCloudWorkspace
        {
            get { return _addCloudWorkspace; }
        }

        public SelectNewResourceTypeViewModel()
        {
            _addDesktop = new RelayCommand(this.ExecuteAddDesktop);
            _addOnPremiseWorkspace = new RelayCommand(this.ExecuteAddOnPremiseWorkspace);
            _addCloudWorkspace = new RelayCommand(this.ExecuteAddCloudWorkspace);
        }

        private void ExecuteAddDesktop(object parameter)
        {
            Contract.Assert(null != this.NavigationService);
            INavigationService nav = this.NavigationService;            
            //
            // First the view must dismiss self, then it must push the next view on the stack;
            // otherwise, the next view will go on top of the current one and they both will get dismissed.
            //
            DismissModal(null);
            nav.PushAccessoryView("DesktopEditorView", null);
        }

        private void ExecuteAddOnPremiseWorkspace(object parameter)
        {
            Contract.Assert(null != this.NavigationService);
            INavigationService nav = this.NavigationService;
            //
            // First the view must dismiss self, then it must push the next view on the stack;
            // otherwise, the next view will go on top of the current one and they both will get dismissed.
            //
            DismissModal(null);
            nav.PushAccessoryView("DesktopEditorView", null);
        }

        private void ExecuteAddCloudWorkspace(object parameter)
        {
            Contract.Assert(null != this.NavigationService);
            INavigationService nav = this.NavigationService;
            //
            // First the view must dismiss self, then it must push the next view on the stack;
            // otherwise, the next view will go on top of the current one and they both will get dismissed.
            //
            DismissModal(null);
            nav.PushAccessoryView("DesktopEditorView", null);
        }
    }
}
