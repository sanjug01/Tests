using RdClient.Shared.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{

    public class DeleteDesktopsArgs
    {
        public IList<object> SelectedDesktops { get; private set; }

        public DeleteDesktopsArgs(IList<object> selectedDesktops)
        {
            SelectedDesktops = selectedDesktops;
        }
    }

    public class DeleteDesktopsViewModel : ViewModelBase
    {
        private readonly ICommand _deleteCommand;
        public ICommand DeleteCommand { get { return _deleteCommand; } }

        private readonly ICommand _cancelCommand;
        public ICommand CancelCommand { get { return _cancelCommand; } }

        public IPresentableView DialogView { private get; set; }

        
        private IList<object> SelectedDesktops 
        { 
            get; 
            set; 
        }

        public bool IsSingleSelection
        {
            get { return true; } 
        }

        public int DesktopsCount
        {
            get 
            {
                int count = 0;
                return count; 
            }
        }


        public DeleteDesktopsViewModel()
        {
        }

        private void DeleteDesktops(object o)
        {
            NavigationService.DismissModalView(DialogView);
            // TBD
        }

        private void Cancel(object o)
        {
            NavigationService.DismissModalView(DialogView);
            // TBD
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Requires(null != activationParameter as DeleteDesktopsArgs);
            Contract.Assert(activationParameter is DeleteDesktopsArgs);
            DeleteDesktopsArgs args = (DeleteDesktopsArgs)activationParameter;

            this.SelectedDesktops = args.SelectedDesktops;
        }
    }
}

