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
    public delegate void AddUserViewCancelledHandler(Credentials credentials);

    public class AddUserViewArgs
    {
        private readonly Credentials _cred;
        private readonly AddUserViewResultHandler _resultHandler;
        private readonly AddUserViewCancelledHandler _cancelledHandler;
        private readonly bool _showSave;

        public AddUserViewArgs(Credentials credential, AddUserViewResultHandler resultHandler, AddUserViewCancelledHandler cancelledHandler, bool showSave)
        {
            _cred = credential;
            _resultHandler = resultHandler;
            _cancelledHandler = cancelledHandler;
            _showSave = showSave;
        }

        public AddUserViewArgs(AddUserViewResultHandler resultHandler, AddUserViewCancelledHandler cancelledHandler, bool showSave)
            : this(new Credentials(), resultHandler, cancelledHandler, showSave)
        {
            _cred.Domain = "";
        }

        public AddUserViewArgs(AddUserViewResultHandler resultHandler, bool showSave)
            : this(resultHandler, null, showSave)
        { }

        public AddUserViewResultHandler ResultHandler { get { return _resultHandler; } }

        public AddUserViewCancelledHandler CancelledHandler { get { return _cancelledHandler; } }

        public bool ShowSave { get { return _showSave; } }

        public Credentials User { get { return _cred; } }
    }

    public class AddUserViewModel : ViewModelBase, IViewModelWithData
    {
        private AddUserViewArgs _args;
        private bool _storeCredentials;
        private string _user;
        private string _password;
        private readonly RelayCommand _okCommand;
        private readonly RelayCommand _cancelCommand;

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

        public IPresentableView PresentableView { private get; set; }

        public bool ShowSave { get; private set; }

        public ICommand OkCommand { get { return _okCommand; } }

        public ICommand CancelCommand { get { return _cancelCommand; } }

        public bool StoreCredentials
        {
            get { return _storeCredentials; }
            set { SetProperty(ref _storeCredentials, value, "StoreCredentials"); }
        }

        public bool IsUsernameValid
        {
            get
            {
                UsernameValidationRule rule = new UsernameValidationRule();
                return rule.Validate(this.User, CultureInfo.CurrentCulture);
            }
        }

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

        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value, "Password");
                _okCommand.EmitCanExecuteChanged();
            }
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
            if (_args.CancelledHandler != null)
            {
                _args.CancelledHandler(_args.User);
            }
        }
    }
}
