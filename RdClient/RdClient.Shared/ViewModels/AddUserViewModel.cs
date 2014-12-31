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
        private readonly Credentials _cred;
        private readonly AddUserViewResultHandler _resultHandler;
        private readonly bool _showSave;

        public AddUserViewArgs(Credentials credential, AddUserViewResultHandler resultHandler, bool showSave)
        {
            _cred = credential;
            _resultHandler = resultHandler;
            _showSave = showSave;
        }

        public AddUserViewArgs(AddUserViewResultHandler resultHandler, bool showSave)
            : this(new Credentials(), resultHandler, showSave)
        { }

        public AddUserViewResultHandler ResultHandler { get { return _resultHandler; } }

        public bool ShowSave { get { return _showSave; } }

        public Credentials User { get { return _cred; } }
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
            this.User = _args.User.Username;
            this.Password = _args.User.Password;
        }

        private void OkCommandHandler(object o)
        {
            _args.User.Username = this.User;
            _args.User.Password = this.Password;

            this.NavigationService.DismissModalView(this.PresentableView);
            if (_args.ResultHandler != null)
            {
                _args.ResultHandler(_args.User, this.StoreCredentials);
            }
        }

        private void CancelCommandHandler(object o)
        {
            this.NavigationService.DismissModalView(this.PresentableView);
        }
    }
}
