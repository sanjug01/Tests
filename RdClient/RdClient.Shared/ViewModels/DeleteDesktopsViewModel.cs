﻿using RdClient.Shared.Models;
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
        private readonly ICommand _cancelCommand;
        private IList<object> _selectedDesktops;
        private int _desktopsCount;
        private string _selectionLabel;

        public ICommand DeleteCommand { get { return _deleteCommand; } }
        public ICommand CancelCommand { get { return _cancelCommand; } }
        public IPresentableView DialogView { private get; set; }

        private IList<object> SelectedDesktops 
        {
            get { return _selectedDesktops; }
            set
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

        public string SelectionLabel
        {
            get { return _selectionLabel; }
            private set
            {
                SetProperty(ref _selectionLabel, value, "SelectionLabel");
            }
        }


        public DeleteDesktopsViewModel()
        {
            _selectedDesktops = null;
            _deleteCommand = new RelayCommand(new Action<object>(DeleteDesktops));
            _cancelCommand = new RelayCommand(new Action<object>(Cancel));

            this.DesktopsCount = 0;
            this.SelectionLabel = "No desktop selected";
        }

        private void DeleteDesktops(object o)
        {
            Contract.Requires(null != this.DataModel);

            if (null != SelectedDesktops)
            {
                int c = SelectedDesktops.Count;
                while (c > 0)
                {
                    this.DataModel.Desktops.Remove(SelectedDesktops[c - 1] as Desktop);
                    c--;
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
            if (null != _selectedDesktops)
            {
                count = _selectedDesktops.Count;
            }
            this.DesktopsCount =  count; 

            // build a string list from all desktops
            string label = "";
            if (this.DesktopsCount > 0)
            {
                label = label + (_selectedDesktops[0] as Desktop).HostName;
                for (int i = 1; i < this.DesktopsCount; i++)
                {
                    label = label + "," + (_selectedDesktops[i] as Desktop).HostName;
                }
            }
            this.SelectionLabel = label;
        }
    }
}

