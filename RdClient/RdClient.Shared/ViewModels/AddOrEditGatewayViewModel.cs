using RdClient.Shared.Data;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.ValidationRules;
using RdClient.Shared.ViewModels.EditCredentialsTasks;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class AddGatewayViewModelArgs
    {
        public AddGatewayViewModelArgs()
        {
        }    
    }

    public class EditGatewayViewModelArgs
    {
        private readonly GatewayModel _gateway;

        public GatewayModel Gateway { get { return _gateway; } }

        public EditGatewayViewModelArgs(GatewayModel gateway)
        {
            _gateway = gateway;
        }
    }

    public class AddOrEditGatewayViewModel : ViewModelBase
    {
        private string _host;
        private bool _isHostValid;
        private bool _isAddingGateway;

        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _cancelCommand;
        private GatewayModel _gateway;
        private int _selectedUserOptionsIndex;

        public AddOrEditGatewayViewModel()
        {
            _saveCommand = new RelayCommand(SaveCommandExecute,
                o =>
                {
                    return (string.IsNullOrEmpty(this.Host) == false);
                });
            _cancelCommand = new RelayCommand(CancelCommandExecute);

            IsHostValid = true;

            this.UserOptions = new ObservableCollection<UserComboBoxElement>();
        }

        public bool IsAddingGateway
        {
            get { return _isAddingGateway; }
            private set 
            {
                this.SetProperty(ref _isAddingGateway, value, "IsAddingGateway");
            }
        }

        public ObservableCollection<UserComboBoxElement> UserOptions { get; set; }

        public int SelectedUserOptionsIndex 
        { 
            get { return _selectedUserOptionsIndex; }

            set
            {
                if (SetProperty(ref _selectedUserOptionsIndex, value))
                {

                    if (value >= 0)
                    {
                        switch (this.UserOptions[value].UserComboBoxType)
                        {
                            case UserComboBoxType.AddNew:
                                //
                                // Push the EditCredentialsView with a task to create new credentials
                                // and save it in the data model.
                                //
                                NewCredentialsTask.Present(this.NavigationService, this.ApplicationDataModel,
                                    //
                                    // Resource name in this case is the host name of the gateway object
                                    //
                                    this.Gateway.HostName,
                                    //
                                    // The editor has saved new credentials and reported their ID;
                                    // Update the gateway object and reload the list od credentials.
                                    //
                                    credentialsId =>
                                    {
                                        this.Gateway.CredentialsId = credentialsId;
                                        this.Update();
                                    },
                                    //
                                    // The editor has been cancelled; reload the list of credentials and restore the
                                    // selected item representing the current choice in the gateway object; if this is not done,
                                    // the selection in the list will stay at the "Add new" item.
                                    //
                                    () => this.Update());
                                break;

                            case UserComboBoxType.AskEveryTime:
                                this.Gateway.CredentialsId = Guid.Empty;
                                break;
                        }
                    }
                }
            }
        }

        public IPresentableView PresentableView { private get; set; }

        public ICommand SaveCommand { get { return _saveCommand; } }

        public ICommand CancelCommand { get { return _cancelCommand; } }

        public GatewayModel Gateway
        {
            get { return _gateway; }
            private set { this.SetProperty(ref _gateway, value); }
        }

        public string Host
        {
            get { return _host; }
            set 
            { 
                SetProperty(ref _host, value); 
                _saveCommand.EmitCanExecuteChanged();
            }
        }

        public bool IsHostValid
        {
            get { return _isHostValid; }
            private set { SetProperty(ref _isHostValid, value); }
        }

        private void SaveCommandExecute(object o)
        {
            if (this.Validate())
            {
                this.Gateway.HostName = this.Host;

                this.Gateway.HostName = this.Host;

                if (null != this.UserOptions[this.SelectedUserOptionsIndex].Credentials)
                {
                    this.Gateway.CredentialsId = this.UserOptions[this.SelectedUserOptionsIndex].Credentials.Id;
                }


                if (this.IsAddingGateway)
                {
                    this.ApplicationDataModel.Gateways.AddNewModel(this.Gateway);
                }

                NavigationService.DismissModalView(PresentableView);
            }
        }

        private void CancelCommandExecute(object o)
        {
            NavigationService.DismissModalView(PresentableView);
        }

        /// <summary>
        /// This method performs all validation tests.
        ///     Currently the validation is performed only on Save command
        /// </summary>
        /// <returns>true, if all validations pass</returns>
        private bool Validate()
        {
            HostNameValidationRule rule = new HostNameValidationRule();
            bool isValid = true;
            if (!(this.IsHostValid = rule.Validate(this.Host, System.Globalization.CultureInfo.CurrentCulture)))
            {
                isValid = isValid && this.IsHostValid;
            }

            return isValid;
        }

        private void Update()
        {
            this.UserOptions.Clear();
            this.UserOptions.Add(new UserComboBoxElement(UserComboBoxType.AskEveryTime));
            this.UserOptions.Add(new UserComboBoxElement(UserComboBoxType.AddNew));

            foreach (IModelContainer<CredentialsModel> credentials in this.ApplicationDataModel.LocalWorkspace.Credentials.Models)
            {
                this.UserOptions.Add(new UserComboBoxElement(UserComboBoxType.Credentials, credentials));
            }

            int idx = 0;
            for (idx = 0; idx < this.UserOptions.Count; idx++)
            {
                if (this.Gateway.HasCredentials &&
                    this.UserOptions[idx].UserComboBoxType == UserComboBoxType.Credentials &&
                    this.UserOptions[idx].Credentials.Id == this.Gateway.CredentialsId)
                    break;
            }

            if (idx == this.UserOptions.Count)
            {
                idx = 0;
            }

            this.SelectedUserOptionsIndex = idx;            
        }


        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(null != activationParameter);

            AddGatewayViewModelArgs addArgs = activationParameter as AddGatewayViewModelArgs;
            EditGatewayViewModelArgs editArgs = activationParameter as EditGatewayViewModelArgs;

            if (editArgs != null)
            {
                this.Gateway = editArgs.Gateway;
                this.Host = this.Gateway.HostName;

                this.IsAddingGateway = false;
            }
            else if(addArgs != null)
            {
                this.Gateway = new GatewayModel();

                this.IsAddingGateway = true;
            }

            Update();
        }
    }
}
