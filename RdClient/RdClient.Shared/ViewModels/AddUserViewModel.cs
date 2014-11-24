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
    public class AddUserViewModel : ViewModelBase, IViewModelWithData
    {
        private Desktop _desktop;

        public IPresentableView PresentableView { private get; set; }

        private bool _isUsernameValid;
        public bool IsUsernameValid
        {
            get {             
                UsernameValidationRule rule = new UsernameValidationRule();
                return rule.Validate(this.User, CultureInfo.CurrentCulture); 
            }
        }

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
                    (this.User.Length > 0) && 
                    (this.Password.Length > 0);
            } );
            _cancelCommand = new RelayCommand(new Action<object>(CancelCommandHandler));
        }

        protected override void OnPresenting(object activationParameter)
        {
//            Contract.Assert(null != activationParameter as Desktop);
//            _desktop = activationParameter as Desktop;
        }

        private void OkCommandHandler(object o)
        {

        }

        private void CancelCommandHandler(object o)
        {
            this.NavigationService.DismissModalView(this.PresentableView);
        }
    }
}
