using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.ValidationRules;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public delegate void AddUserViewResultHandler(Credentials credentials, bool store);

    public class AddUserViewArgs
    {
        private readonly AddUserViewResultHandler _resultHandler;
        public AddUserViewResultHandler ResultHandler { get { return _resultHandler; } }

        private readonly Desktop _desktop;
        public Desktop Desktop {get { return _desktop; } }

        private readonly bool _showSave;
        public bool ShowSave { get { return _showSave; } }

        public AddUserViewArgs(Desktop desktop, AddUserViewResultHandler resultHandler, bool showSave)
        {
            _desktop = desktop;
            _resultHandler = resultHandler;
            _showSave = showSave;
        }
    }

    public class AddUserViewModel : ViewModelBase, IViewModelWithData
    {
        private AddUserViewArgs _args;

        public IPresentableView PresentableView { private get; set; }

        private bool _storeCredentials;
        public bool StoreCredentials { 
            get { return _storeCredentials; }
            set { SetProperty(ref _storeCredentials, value, "StoreCredentials"); } 
        }

        public bool IsUsernameValid
        {
            get {             
                UsernameValidationRule rule = new UsernameValidationRule();
                return rule.Validate(this.User, CultureInfo.CurrentCulture); 
            }
        }

        public bool ShowSave { get; private set; }

        private string _user;
        public string User
        {
            get { return _user; }
            set
            {
                SetProperty(ref _user, value, "User");
                this.EmitPropertyChanged("IsUsernameValid");
                _okCommand.EmitCanExecuteChanged();
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value, "Password");
                _okCommand.EmitCanExecuteChanged();
            }
        }

        private readonly RelayCommand _okCommand;
        public ICommand OkCommand { get { return _okCommand; } }

        private readonly RelayCommand _cancelCommand;
        public ICommand CancelCommand { get { return _cancelCommand; } }

        public AddUserViewModel()
        {
            _okCommand = new RelayCommand(new Action<object>(OkCommandHandler), (o) => {
                return 
                    (this.User != null) && 
                    (this.Password != null) && 
                    (this.IsUsernameValid) && 
                    (this.Password.Length > 0);
            } );
            _cancelCommand = new RelayCommand(new Action<object>(CancelCommandHandler));
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(null != activationParameter as AddUserViewArgs);
            _args = activationParameter as AddUserViewArgs;
            this.ShowSave = _args.ShowSave;
        }

        private void OkCommandHandler(object o)
        {
            Credentials credentials = new Credentials()
            {
                Username = this.User,
                Password = this.Password,
                Domain = ""
            };

            this.NavigationService.DismissModalView(this.PresentableView);
            _args.ResultHandler(credentials, this.StoreCredentials);
        }

        private void CancelCommandHandler(object o)
        {
            this.NavigationService.DismissModalView(this.PresentableView);
        }
    }
}
