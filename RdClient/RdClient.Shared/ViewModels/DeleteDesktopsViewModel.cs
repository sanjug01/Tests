﻿﻿using RdClient.Shared.Models;
using RdClient.Shared.Navigation;

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{

    public class DeleteDesktopsArgs
    {
        public IList<Desktop> SelectedDesktops { get; private set; }

        public DeleteDesktopsArgs(IList<Desktop> selectedDesktops)
        {
            SelectedDesktops = selectedDesktops;
        }

        public DeleteDesktopsArgs(Desktop desktop)
        {
            // create a single object list
            SelectedDesktops =  new List<Desktop>();
            SelectedDesktops.Add(desktop);            
        }
    }

    public class DeleteDesktopsViewModel : ViewModelBase
    {
        private readonly ICommand _deleteCommand;
        private readonly ICommand _cancelCommand;
        private IList<Desktop> _selectedDesktops;
        private int _desktopsCount;

        public ICommand DeleteCommand { get { return _deleteCommand; } }
        public ICommand CancelCommand { get { return _cancelCommand; } }
        public IPresentableView DialogView { private get; set; }

        public IList<Desktop> SelectedDesktops 
        {
            get { return _selectedDesktops; }
            private set
            {
                SetProperty(ref _selectedDesktops, value, "SelectedDesktops");
                this.EmitPropertyChanged("DesktopsCount");
                this.EmitPropertyChanged("IsSingleSelection");
            }
        }

        public bool IsSingleSelection
        {
            get { return (1==this.DesktopsCount); } 
        }

        public int DesktopsCount
        {
            get { return _desktopsCount; }
            private set
            {
                SetProperty(ref _desktopsCount, value, "DesktopsCount");
                this.EmitPropertyChanged("IsSingleSelection");
            }
        }

        public DeleteDesktopsViewModel()
        {
            _selectedDesktops = null;
            _deleteCommand = new RelayCommand(new Action<object>(DeleteDesktops));
            _cancelCommand = new RelayCommand(new Action<object>(Cancel));
            this.DesktopsCount = 0;
        }

        private void DeleteDesktops(object o)
        {
            Contract.Requires(null != this.DataModel);

            if (null != SelectedDesktops)
            {
                for(int i=0; i< SelectedDesktops.Count; i++)
                {
                    this.DataModel.LocalWorkspace.Connections.Remove(SelectedDesktops[i]);
                }

                SelectedDesktops.Clear();
            }

            NavigationService.DismissModalView(DialogView);
        }

        private void Cancel(object o)
        {
            NavigationService.DismissModalView(DialogView);
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Requires(null != activationParameter as DeleteDesktopsArgs);
            Contract.Assert(activationParameter is DeleteDesktopsArgs);

            DeleteDesktopsArgs args = (DeleteDesktopsArgs)activationParameter;
            this.SelectedDesktops = args.SelectedDesktops;
            int count = 0;
            if (null != this.SelectedDesktops)
            {
                count = this.SelectedDesktops.Count;
            }
            this.DesktopsCount = count; 
        }
    }
}

