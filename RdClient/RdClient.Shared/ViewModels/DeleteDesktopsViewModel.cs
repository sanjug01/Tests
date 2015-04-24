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

    public class DeleteDesktopsViewModel : ViewModelBase, IDialogViewModel
    {
        private readonly ICommand _deleteCommand;
        private readonly ICommand _cancelCommand;
        private IList<IModelContainer<DesktopModel>> _selectedDesktops;
        private int _desktopsCount;

        public DeleteDesktopsViewModel()
        {
            _selectedDesktops = null;
            _deleteCommand = new RelayCommand(o => DeleteDesktops());
            _cancelCommand = new RelayCommand(o => CancelCommandExecute());
            this.DesktopsCount = 0;
        }

        public ICommand DefaultAction { get { return _deleteCommand; } }
        public ICommand Cancel { get { return _cancelCommand; } }

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

        private void DeleteDesktops()
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

            this.DismissModal(null);            
        }

        private void CancelCommandExecute()
        {
            this.DismissModal(null);
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

