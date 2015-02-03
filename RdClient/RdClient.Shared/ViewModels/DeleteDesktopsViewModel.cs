namespace RdClient.Shared.ViewModels
{
    ﻿﻿using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;


    public class DeleteDesktopsArgs
    {
        public IList<IModelContainer<DesktopModel>> SelectedDesktops { get; private set; }

        public DeleteDesktopsArgs(IList<IModelContainer<DesktopModel>> selectedDesktops)
        {
            SelectedDesktops = selectedDesktops;
        }

        public DeleteDesktopsArgs(IModelContainer<DesktopModel> desktop)
        {
            // create a single object list
            SelectedDesktops = new List<IModelContainer<DesktopModel>>() { desktop };
        }
    }

    public class DeleteDesktopsViewModel : ViewModelBase
    {
        private readonly ICommand _deleteCommand;
        private readonly ICommand _cancelCommand;
        private IList<IModelContainer<DesktopModel>> _selectedDesktops;
        private int _desktopsCount;

        public ICommand DeleteCommand { get { return _deleteCommand; } }
        public ICommand CancelCommand { get { return _cancelCommand; } }
        public IPresentableView DialogView { private get; set; }

        public IList<IModelContainer<DesktopModel>> SelectedDesktops 
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
                SetProperty(ref _desktopsCount, value);
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
            Contract.Requires(null != this.ApplicationDataModel);

            if (null != SelectedDesktops)
            {
                foreach(IModelContainer<DesktopModel> container in _selectedDesktops)
                {
                    this.ApplicationDataModel.LocalWorkspace.Connections.RemoveModel(container.Id);
                }

                SelectedDesktops.Clear();
                this.DesktopsCount = 0;
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

